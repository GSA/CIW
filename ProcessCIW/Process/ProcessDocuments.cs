using CsvHelper.Configuration;
using FluentValidation.Results;
using ProcessCIW.Interface;
using ProcessCIW.Mapping;
using ProcessCIW.Models;
using ProcessCIW.Process;
using ProcessCIW.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using ProcessCIW.Enum;

namespace ProcessCIW
{
    /// <summary>
    /// Class that controls processing of CIW forms
    /// </summary>
    public class ProcessDocuments : IProcessDocuments
    {
        private readonly ILogTool log;
        private static CsvConfiguration config;
        private readonly IDataAccess dataAccess;
        private readonly IFileTool fileTool;
        private readonly IXmlTool xmlTool;
        private readonly IUtilities U;
        private readonly IDeleteTool dt;

        /// <summary>
        /// Constructor that sets up CsvConfiguration which is part of CsvHelper
        /// </summary>
        public ProcessDocuments(IDataAccess dataAccess, IFileTool fileTool, IXmlTool xt, IUtilities U, IDeleteTool dt, ILogTool log)
        {
            this.dt = dt;
            xmlTool = xt;
            this.U = U;
            this.dataAccess = dataAccess;
            this.fileTool = fileTool;
            this.log = log;

            config = new CsvConfiguration();

            config.Delimiter = "||";
            config.HasHeaderRecord = true;
            config.WillThrowOnMissingField = false;
            config.IsHeaderCaseSensitive = false;
            config.TrimFields = false;
        }

        /// <summary>
        /// Get all the CIW information, create temp csv file then load that and then filter it down to the different objects
        /// </summary>
        /// <param name="fileName"></param>
        public string GetCIWInformation(int uploaderID, string filePath, string fileName, out int errorCode)
        {
            List<CIWData> ciwInformation;
            log.Info(string.Format("Getting information from file {0}", filePath));

            //Check for password protection
            if (xmlTool.isPasswordProtected(filePath))
            {
                sendPasswordProtection(uploaderID, U.fileNameHelper(fileName));
                errorCode = (int)ErrorCodes.password_protected;
                log.Warn(string.Format("Inserting error code {0}:{1} into upload table", ErrorCodes.password_protected, (int)ErrorCodes.password_protected));
                return null;
            }

            ciwInformation = xmlTool.parseCiwDocument(filePath, uploaderID, fileName, out errorCode);

            //used in log
            string lastFirst = (ciwInformation.FirstOrDefault(c => c.TagName == "Employee-LastName").InnerText ?? "null") + ", " + (ciwInformation.FirstOrDefault(c => c.TagName == "Employee-FirstName").InnerText ?? "null");

            log.Info(string.Format("CiwInformation obtained for {0}", lastFirst));
            log.Info(string.Format("Creating temp file for {0}", lastFirst));

            //Create a temp csv file of the information within the form
            string tempFile = fileTool.CreateTempFile(ciwInformation);
            errorCode = (int)ErrorCodes.successfully_processed;

            return tempFile;

        }



        /// <summary>
        /// Function that is called if password protection detected
        /// Calls sendEmail constructor and function to send email for password protection.
        /// </summary>
        /// <param name="uploaderID"></param>
        /// <param name="fileName"></param>
        private void sendPasswordProtection(int uploaderID, string fileName)
        {
            CiwEmails sendEmails = new CiwEmails(dataAccess, log);
            sendEmails.Setup(uploaderID, "", "", "", "", fileName);

            sendEmails.SendPasswordProtection();
        }        

        

        /// <summary>
        /// Helper function to check for child care applicant on CIW
        /// </summary>
        /// <param name="ciwInformation"></param>
        /// <returns></returns>
        private bool CheckIfChildCare(List<CIW> ciwInformation)
        {
            return ciwInformation.First().ContractorType == "Child Care" || ciwInformation.First().InvestigationTypeRequested == "Tier 1C";
        }

        private void ApplyFipsCodes(ref List<CIW> ciw, DataSet ds)
        {
            ciw[0].PlaceOfBirthCountry = ds.Tables[0].Rows.Count > 0 ? ds.Tables[0].Rows[0].ItemArray[0].ToString() : string.Empty;
            ciw[0].HomeAddressCountry = ds.Tables[1].Rows.Count > 0 ? ds.Tables[1].Rows[0].ItemArray[0].ToString() : string.Empty;
            ciw[0].CitzenshipCountry = ds.Tables[2].Rows.Count > 0 ? ds.Tables[2].Rows[0].ItemArray[0].ToString() : string.Empty;
        }

        /// <summary>
        /// Processes data after CIW converted to CSV
        /// </summary>
        /// <param name="uploaderID"></param>
        /// <param name="filePath"></param>
        /// <param name="isDebug"></param>
        /// <returns>Int success code</returns>
        public int ProcessCIWInformation(int uploaderID, string filePath, bool isDebug, IValidateCiw validateCiw)
        {
            log.Info("Processing CIW");

            //Create validation object
            IValidateCiw validate = validateCiw;

            List<CIW> ciwInformation;

            log.Info("Getting file data from temp csv file.");

            //Gets list of CIW's after mapping from csv files
            ciwInformation = fileTool.GetFileData<CIW, CIWMapping>(filePath, config);

            DataSet fipsCodes = dataAccess.GetFipsCodeFromCountryName(ciwInformation[0].PlaceOfBirthCountryName, ciwInformation[0].HomeCountryName, ciwInformation[0].CitizenCountryName);

            ApplyFipsCodes(ref ciwInformation, fipsCodes);

            CiwEmails sendEmails = new CiwEmails(dataAccess, log);
            sendEmails.Setup(uploaderID, ciwInformation.First().FirstName, ciwInformation.First().MiddleName,
                                                 ciwInformation.First().LastName, ciwInformation.First().Suffix, Path.GetFileName(filePath),
                                                 CheckIfChildCare(ciwInformation));

            //Delete temp csv file before proceeding
            dt.deleteTempCsvFile(filePath);

            log.Info("Processing " + ciwInformation.First().FullNameForLog);

            log.Info(string.Format("Checking version number. Current version is {0}", ciwInformation.First().VersionNumber));

            //Check version and begin exit if wrong version
            if (ciwInformation.First().VersionNumber != ConfigurationManager.AppSettings["VERSION"])
            {
                log.Warn("Sending Wrong Version Number E-Mail");
                sendEmails.SendWrongVersion();
                log.Warn(string.Format("Inserting error code {0}:{1} into upload table", ErrorCodes.wrong_version, (int)ErrorCodes.wrong_version));
                return (int)ErrorCodes.wrong_version;

            }
            else
                log.Info("Version OK");

            log.Info(string.Format("Checking if ARRA. ARRA selected is: {0}", ciwInformation.First().ArraLongTermContractor));

            //Check if ARRA contractor and begin exit if ARRA
            if (ciwInformation.First().ArraLongTermContractor == "Yes")
            {
                log.Warn("Sending ARRA E-Mail");
                sendEmails.SendARRA();
                log.Warn(string.Format("Inserting error code {0}:{1} into upload table", ErrorCodes.arra, (int)ErrorCodes.arra));
                return (int)ErrorCodes.arra;
            }
            else
                log.Info("ARRA is OK");

            log.Info(String.Format("Checking if {0} is a duplicate user", ciwInformation.First().FullNameForLog));

            //Check if duplicate and begin exit if duplicate exists
            if (!validate.IsDuplicate(ciwInformation))
            {
                log.Warn(String.Format("Duplicate user found for {0}", ciwInformation.First().FullNameForLog));
                sendEmails.SendDuplicateUser();
                log.Warn(string.Format("Inserting error code {0}:{1} into upload table", ErrorCodes.duplicate_user, (int)ErrorCodes.duplicate_user));
                return (int)ErrorCodes.duplicate_user;
            }

            log.Info(String.Format("No existing user found for {0}", ciwInformation.First().FullNameForLog));

            log.Info(String.Format("Company Name Primary is : {0}", !string.IsNullOrWhiteSpace(ciwInformation.FirstOrDefault().CompanyName) ? ciwInformation.FirstOrDefault().CompanyName : "No Company Name Primary"));
            log.Info(String.Format("Company Name Sub is : {0}", !string.IsNullOrWhiteSpace(ciwInformation.FirstOrDefault().CompanyNameSub) ? ciwInformation.FirstOrDefault().CompanyNameSub : "No Company Name Sub"));
            log.Info(String.Format("Checking if form is valid for user {0}", ciwInformation.First().FullNameForLog));



            //Validation is called inside if statement
            if (validate.IsFormValid(ciwInformation))
            {
                log.Info(string.Format("Form is valid for user {0}", ciwInformation.First().FullNameForLog));

                int persID = 0;

                //Save the data
                log.Info(string.Format("Begin inserting CIW for {0}", ciwInformation.First().FullNameForLog));
                persID = dataAccess.InsertCIW(ciwInformation.First(), uploaderID);

                //Begin sponsorship if successful
                if (persID > 0)
                    sendEmails.SendSponsorshipEMail(persID);
                log.Info(string.Format("Inserting error code {0}:{1} into upload table", ErrorCodes.successfully_processed, (int)ErrorCodes.successfully_processed));
                return (int)ErrorCodes.successfully_processed;
            }
            else
            {
                log.Warn(string.Format("Form failed validation for user {0}", ciwInformation.First().FullNameForLog));

                //E-Mail Failure Template
                //Send error email
                Tuple<ValidationResult, ValidationResult, ValidationResult, ValidationResult, ValidationResult, ValidationResult> ValidationErrors;
                log.Info("Getting errors");

                ValidationErrors = validate.GetErrors();

                log.Info(string.Format("{0} errors returned", CountErrors(ValidationErrors)));

                //send error email which contains a list of each sections errors
                sendEmails.SendErrors(ValidationErrors.Item1, ValidationErrors.Item2, ValidationErrors.Item3,
                                       ValidationErrors.Item4, ValidationErrors.Item5, ValidationErrors.Item6);
                log.Warn(string.Format("Inserting error code {0}:{1} into upload table", ErrorCodes.failed_validation, (int)ErrorCodes.failed_validation));

                return (int)ErrorCodes.failed_validation;
            }
        }

        /// <summary>
        /// Counts the total number of errors
        /// </summary>
        /// <param name="t"></param>
        /// <returns>Count of errors</returns>
        private int CountErrors(Tuple<ValidationResult, ValidationResult, ValidationResult, ValidationResult, ValidationResult, ValidationResult> t)
        {
            var count = t.Item1.Errors.Count + t.Item2.Errors.Count + t.Item3.Errors.Count + t.Item4.Errors.Count + t.Item5.Errors.Count + t.Item6.Errors.Count;
            return count;
        }

        

        
    }
}