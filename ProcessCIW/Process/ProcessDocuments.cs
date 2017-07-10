using CsvHelper;
using CsvHelper.Configuration;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using FluentValidation.Results;
using MySql.Data.MySqlClient;
using ProcessCIW.Mapping;
using ProcessCIW.Models;
using ProcessCIW.Process;
using ProcessCIW.Validation;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml;

namespace ProcessCIW
{
    class ProcessDocuments
    {
        private static CsvConfiguration config;
        private readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ProcessDocuments()
        {
            config = new CsvConfiguration();

            config.Delimiter = "||";
            config.HasHeaderRecord = true;
            config.WillThrowOnMissingField = false;
            config.IsHeaderCaseSensitive = false;
            config.TrimFields = false;
        }

        public List<UnprocessedFiles> GetUnprocessedFiles()
        {
            MySqlCommand cmd = new MySqlCommand();

            List<UnprocessedFiles> uf = new List<UnprocessedFiles>();

            using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["GCIMS"].ToString()))
            {
                conn.Open();

                using (cmd)
                {
                    MySqlDataReader unprocessedFiles;

                    cmd.Connection = conn;
                    cmd.CommandText = "CIW_Unprocessed";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("SQLExceptionWarning", MySqlDbType.VarChar, 4000);

                    unprocessedFiles = cmd.ExecuteReader();

                    while (unprocessedFiles.Read())
                    {
                        uf.Add(
                                new UnprocessedFiles
                                {
                                    ID = (int)unprocessedFiles[0],
                                    PersID = (int)unprocessedFiles[1],
                                    FileName = unprocessedFiles[2].ToString()
                                }
                              );
                    }
                }
            }

            log.Info(string.Format("CIW_Unprocessed returned with {0} unprocessed files and SQLExceptionWarning:{1}", uf.Count, cmd.Parameters["SQLExceptionWarning"].Value));

            return uf;
        }

        public void UpdateProcessed(int documentID, int processedResult)
        {
            MySqlCommand cmd = new MySqlCommand();

            log.Info(string.Format("Updating processed document {0} with result {1}", documentID, processedResult));

            using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["GCIMS"].ToString()))
            {
                conn.Open();

                using (cmd)
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "CIW_UpdateProcessed";
                    cmd.CommandType = CommandType.StoredProcedure;

                    MySqlParameter[] ProcessedParameters = new MySqlParameter[]
                    {
                        new MySqlParameter { ParameterName = "documentID", Value = documentID, MySqlDbType = MySqlDbType.Int32, Direction = ParameterDirection.Input },
                        new MySqlParameter { ParameterName = "processedResult", Value = processedResult, MySqlDbType = MySqlDbType.Int32, Direction = ParameterDirection.Input },
                        new MySqlParameter { ParameterName = "SQLExceptionWarning", MySqlDbType=MySqlDbType.VarChar, Size=4000, Direction = ParameterDirection.Output },
                    };

                    cmd.Parameters.AddRange(ProcessedParameters);

                    cmd.ExecuteNonQuery();

                    log.Info(string.Format("CIW_UpdateProcessed completed and error message: {0}", cmd.Parameters["SQLExceptionWarning"].Value));
                }
            }
        }

        /// <summary>
        /// Get all the CIW inforamtion, create temp csv file then load that and then filter it down to the different objects!!!!!
        /// </summary>
        /// <param name="fileName"></param>
        public string GetCIWInformation(int uploaderID, string filePath, string fileName, out List<CIWData> dupes)
        {
            List<CIWData> ciwInformation = new List<CIWData>();

            log.Info(String.Format("Getting information from file {0}", filePath));
            
            try
            {
                using (WordprocessingDocument wd = WordprocessingDocument.Open(filePath, false))
                {
                    DocumentProtection dp = wd.MainDocumentPart.DocumentSettingsPart.Settings.GetFirstChild<DocumentProtection>();
                }
            }
            catch (FileFormatException e)
            {
                log.Error(string.Format("Locked Document - {0} with innner exception:{1}", e.Message, e.InnerException));                
                sendPasswordProtection(uploaderID, fileNameHelper(fileName));
                dupes = null;
                return null;
            }

            using (var document = WordprocessingDocument.Open(filePath, true))
            {
                XmlDocument xml = new XmlDocument();
                MainDocumentPart docPart = document.MainDocumentPart;
                xml.InnerXml = docPart.Document.FirstChild.OuterXml;
                XmlNamespaceManager nameSpaceManager = new XmlNamespaceManager(xml.NameTable);
                nameSpaceManager.AddNamespace("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");


                var node = xml.SelectSingleNode(string.Format("w:body/w:tbl/w:tr/w:tc/w:tbl/w:tr/w:tc/w:sdt/w:sdtContent/w:p/w:r/w:t"), nameSpaceManager);

                if (node != null)
                {
                    if (node.InnerText != "V1")
                    {
                        sendWrongVersion(uploaderID, fileNameHelper(fileName));
                        dupes = null;
                        return null;
                    }
                }
                else
                {
                    sendWrongVersion(uploaderID, fileNameHelper(fileName));
                    dupes = null;
                    return null;
                }
                
                try
                {
                    //Gets all data on the form via tags
                    log.Info(string.Format("Parsing XML and searching for nested fields."));
                    ciwInformation = docPart.Document.Descendants<SdtBlock>()
                                        .Select
                                            (
                                                s =>
                                                    new CIWData
                                                    {
                                                        TagName = s.GetFirstChild<SdtProperties>().GetFirstChild<Tag>().Val,
                                                        InnerText = ParseXML(s.InnerText, s.OuterXml),
                                                        Child = getNestedChild(s.OuterXml)

                                                    }
                                            ).ToList();
                }
                catch (Exception e)
                {
                    log.Error(string.Format("XML Parsing Failed - {0} with inner exception: {1}", e.Message, e.InnerException));
                    sendWrongVersion(uploaderID, fileNameHelper(fileName));

                    dupes = null;

                    return null;
                }

                dupes = ciwInformation.Where(c => c.Child != String.Empty).ToList();

                string lastFirst = ciwInformation.FirstOrDefault(c => c.TagName == "Employee-LastName").InnerText ?? "null" + ", " + ciwInformation.FirstOrDefault(c => c.TagName == "Employee-FirstName").InnerText ?? "null";

                log.Info(String.Format("CiwInformation obtained for {0}", lastFirst));

                //Create a temp csv file of the informaiton within the form
                log.Info(String.Format("Creating temp file for {0}", lastFirst));
                string tempFile = CreateTempFile(ciwInformation);

                return tempFile;
            }
        }

        private void sendWrongVersion(int uploaderID, string fileName)
        {
            CIWEMails sendEmails = new CIWEMails(uploaderID, "", "", "", "", fileName);

            sendEmails.SendWrongVersion();
        }

        private void sendPasswordProtection(int uploaderID, string fileName)
        {
            CIWEMails sendEmails = new CIWEMails(uploaderID, "", "", "", "", fileName);

            sendEmails.SendPasswordProtection();
        }

        private string fileNameHelper(string fileName)
        {

            int _ = fileName.LastIndexOf("_");
            string _name = fileName.Remove(_, fileName.Length - _ - 5);

            return _name;
        }

        public string getNestedChild(string outerXML)
        {
            XmlDocument xml = new XmlDocument();
            if (!String.IsNullOrEmpty(outerXML))
            {
                xml.InnerXml = outerXML;
            }
            XmlNamespaceManager nameSpaceManager = new XmlNamespaceManager(xml.NameTable);
            nameSpaceManager.AddNamespace("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
            var node = xml.SelectSingleNode(string.Format("w:sdt/w:sdtContent/w:p/w:sdt/w:sdtPr/w:tag"), nameSpaceManager);
            if (node == null)
            {
                node = xml.SelectSingleNode(string.Format("w:sdt/w:sdtContent/w:sdt/w:sdtPr/w:tag"), nameSpaceManager);

            }
            return node != null ? node.Attributes[0].Value : string.Empty;
        }


        private string ParseXML(string innerText, string outerXML)
        {
            //if xml contains dropdownlist then parse and return value otherwise return innerxml
            XmlDocument xml = new XmlDocument();

            if (!String.IsNullOrEmpty(outerXML))
            {
                xml.InnerXml = outerXML;
            }

            // Add the namespace.  
            XmlNamespaceManager nameSpaceManager = new XmlNamespaceManager(xml.NameTable);

            nameSpaceManager.AddNamespace("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");

            //check if it is a list
            XmlNodeList elemList = xml.GetElementsByTagName("w:listItem");

            if (elemList.Count == 0)
                return innerText;
            else
            {
                if (!String.IsNullOrEmpty(innerText))
                {
                    XmlNode a = xml.SelectSingleNode(string.Format("w:sdt/w:sdtPr/w:dropDownList/w:listItem[@w:displayText='{0}']", innerText), nameSpaceManager);

                    if (a.Attributes.Count > 1)
                    {
                        if (a.Attributes[1].Value != null)
                        {
                            return a.Attributes[1].Value;
                        }
                    }
                    else return a.Attributes[0].Value;
                }
            }

            return innerText;
        }

        private bool CheckIfChildCare(List<CIW> ciwInformation)
        {
            return ciwInformation.First().ContractorType == "Child Care" || ciwInformation.First().InvestigationTypeRequested == "Tier 1C";
        }

        public int ProcessCIWInformation(int uploaderID, string filePath, bool isDebug, List<CIWData> dupes)
        {
            log.Info("Processing CIW");           

            ValidateCIW validate = new Validation.ValidateCIW();

            List<CIW> ciwInformation = new List<CIW>();

            log.Info(string.Format("Getting file data from temp csv file."));

            ciwInformation = GetFileData<CIW, CIWMapping>(filePath, config);

            CIWEMails sendEmails = new CIWEMails(uploaderID, ciwInformation.First().FirstName, ciwInformation.First().MiddleName,
                                                 ciwInformation.First().LastName, ciwInformation.First().Suffix, Path.GetFileName(filePath),
                                                 CheckIfChildCare(ciwInformation));

            try
            {
                log.Info(string.Format("Deleting Temp CSV File {0}.", filePath));
                File.Delete(filePath); 
            }
            catch (IOException e)
            {
                log.Error("Unable to delete temp file" + e.Message);
                return 0;
            }

            log.Info("Processing " + ciwInformation.First().FullNameForLog);

            log.Info(string.Format("Checking version number. Current version is {0}", ciwInformation.First().VersionNumber));
            if (ciwInformation.First().VersionNumber != ConfigurationManager.AppSettings["VERSION"])
            {
                log.Error("Sending Wrong Version Number E-Mail");
                sendEmails.SendWrongVersion();
                return -1;
            }
            else
                log.Info(string.Format("Version OK"));

            log.Info(string.Format("Checking if ARRA. ARRA selected is: {0}", ciwInformation.First().ArraLongTermContractor));
            if (ciwInformation.First().ArraLongTermContractor == "Yes")
            {
                log.Error("Sending ARRA E-Mail");
                sendEmails.SendARRA();
                return -1;
            }
            else
                log.Info(string.Format("ARRA is OK"));

            //E-Mail Duplicate Template
            log.Info(String.Format("Checking if {0} is a duplicate user", ciwInformation.First().FullNameForLog));
            if (!validate.IsDuplicate(ciwInformation))
            {
                log.Error(String.Format("Duplicate user found for {0}", ciwInformation.First().FullNameForLog));
                sendEmails.SendDuplicateUser();

                return -1;
            }

            log.Info(String.Format("No existing user found for {0}", ciwInformation.First().FullNameForLog));

            log.Info(String.Format("Checking if form is valid for user {0}", ciwInformation.First().FullNameForLog));

            ciwInformation.First().Dupes = dupes;

            //Short circuit evaluation of If statement removed so we can always get list of all errors
            if (validate.IsFormValid(ciwInformation) & validate.IsNested(ciwInformation))
            {
                log.Info(String.Format("Form is valid for user {0}", ciwInformation.First().FullNameForLog));


                InsertCIW sd = new InsertCIW(ciwInformation.First(), uploaderID);

                int persID = 0;

                //Save Data
                log.Info(String.Format("Begin inserting CIW for {0}", ciwInformation.First().FullNameForLog));
                persID = sd.SaveCIW();

                if (persID > 0)
                    sendEmails.SendSponsorshipEMail(persID);

                return 1;
            }
            else
            {
                log.Error(String.Format("Form failed validation for user {0}", ciwInformation.First().FullNameForLog));

                //E-Mail Failure Template
                //Send error email                    
                Tuple<ValidationResult, ValidationResult, ValidationResult,
                        ValidationResult, ValidationResult, ValidationResult, ValidationResult> ValidationErrors = new Tuple<ValidationResult, ValidationResult, ValidationResult,
                                                                                                            ValidationResult, ValidationResult, ValidationResult, ValidationResult>(null, null, null,
                                                                                                                                                                null, null, null, null);

                log.Info(string.Format("Getting errors"));

                ValidationErrors = validate.GetErrors();

                log.Info(string.Format("{0} errors returned", CountErrors(ValidationErrors)));

                sendEmails.SendErrors(ValidationErrors.Item1, ValidationErrors.Item2, ValidationErrors.Item3,
                                        ValidationErrors.Item4, ValidationErrors.Item5, ValidationErrors.Item6, ValidationErrors.Item7, ciwInformation.First().Dupes);

                return -1;
            }
        }

        private int CountErrors(Tuple<ValidationResult, ValidationResult, ValidationResult, ValidationResult, ValidationResult, ValidationResult, ValidationResult> t)
        {
            var count = t.Item1.Errors.Count + t.Item2.Errors.Count + t.Item3.Errors.Count + t.Item4.Errors.Count + t.Item5.Errors.Count + t.Item6.Errors.Count + t.Item7.Errors.Count;
            return count;
        }

        /// <summary>
        /// Generates temp CSV file separated by ~
        /// </summary>
        /// <param name="ciwData"></param>
        private string CreateTempFile(List<CIWData> ciwData)
        {
             //Get first and last name
            string first = ciwData.First(c => c.TagName == "Employee-FirstName").InnerText;
            string last = ciwData.First(c => c.TagName == "Employee-LastName").InnerText;

            //If either is null or empty then use p;aceholder name
            first = (first == null ? "FirstNameNull" : (first == "" ? "FirstNameEmpty" : first));            
            last = (last == null ? "LastNameNull" : (last == "" ? "LastNameEmpty" : last));

            //uses first 20 characters of first and last name and adds timestamp to end and then .csv
            string csvFileName = first.Length >= 20 ? first.Substring(0, 20) : first.Substring(0, first.Length) + "_" + (last.Length >= 20 ? last.Substring(0, 20) : last.Substring(0, last.Length)) + "_" + DateTime.Now.ToString("MMddyyyy_HHmmss") + ".csv";
            
            log.Info("CIW Info Count: " + ciwData.Count);

            string fileName = ConfigurationManager.AppSettings["TEMPFOLDER"] + csvFileName; 
                        
            try
            {
                using (StreamWriter writer = new StreamWriter(fileName, false))
                {
                    writer.WriteLine(string.Join("||", ciwData.Select(item => item.TagName)));
                    writer.WriteLine(string.Join("||", ciwData.Select(item => item.InnerText)));
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message + " - " + ex.InnerException);
            }

            log.Info(string.Format("Temp csv file {0} created", fileName));

            return fileName;
        }        

        /// <summary>
        /// Loads the CIW information
        /// </summary>
        /// <typeparam name="TClass"></typeparam>
        /// <typeparam name="TMap"></typeparam>
        /// <param name="filePath"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        private List<TClass> GetFileData<TClass, TMap>(string filePath, CsvConfiguration config)
            where TClass : class
            where TMap : CsvClassMap<TClass>
        {
            log.Info(string.Format("Parsing CSV file {0} and mapping to CIW object", filePath));

            using (CsvParser csvParser = new CsvParser(new StreamReader(filePath), config))
            {
                using (CsvReader csvReader = new CsvReader(csvParser))
                {
                    csvReader.Configuration.RegisterClassMap<TMap>();

                    return csvReader.GetRecords<TClass>().ToList();
                }
            }
        }
    }
}