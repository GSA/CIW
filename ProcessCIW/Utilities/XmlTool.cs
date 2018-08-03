using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using ProcessCIW.Enum;
using ProcessCIW.Interface;
using ProcessCIW.Models;
using ProcessCIW.Process;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace ProcessCIW.Utilities
{
    public class XmlTool :IXmlTool
    {
        private readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IUtilities U;

        public XmlTool()
        {
            U = new Utilities();
        }

        public bool isPasswordProtected(string filePath)
        {
            try
            {
                using (WordprocessingDocument wd = WordprocessingDocument.Open(filePath, false))
                {
                    DocumentProtection dp = wd.MainDocumentPart.DocumentSettingsPart.Settings.GetFirstChild<DocumentProtection>();
                    return false;
                }
            }
            catch (FileFormatException e)
            {
                log.Warn(string.Format("Locked Document - {0} with inner exception:{1}", e.Message, e.InnerException));

                return true;
            }            
        }

        public List<CIWData> parseCiwDocument(string filePath, int uploaderID, string fileName, out int errorCode)
        {
            List<CIWData> ciwInformation = new List<CIWData>();

            //Begin parsing XML from CIW document
            using (var document = WordprocessingDocument.Open(filePath, true))
            {
                XmlDocument xml = new XmlDocument();
                MainDocumentPart docPart = document.MainDocumentPart;
                xml.InnerXml = docPart.Document.FirstChild.OuterXml;
                XmlNamespaceManager nameSpaceManager = new XmlNamespaceManager(xml.NameTable);
                nameSpaceManager.AddNamespace("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");

                //Get Version number node
                var node = xml.SelectSingleNode(string.Format("w:body/w:tbl/w:tr/w:tc/w:tbl/w:tr/w:tc/w:sdt/w:sdtContent/w:p/w:r/w:t"), nameSpaceManager);

                if (node != null)
                {
                    if (node.InnerText != "V1")
                    {
                        //Begin exiting if wrong version
                        sendWrongVersion(uploaderID, U.fileNameHelper(fileName));
                        errorCode = (int)ErrorCodes.wrong_version;
                        log.Warn(string.Format("Inserting error code {0}:{1} into upload table", ErrorCodes.wrong_version, (int)ErrorCodes.wrong_version));
                        return null;
                    }
                }
                else
                {
                    //Begin exiting if no version on form
                    sendWrongVersion(uploaderID, U.fileNameHelper(fileName));
                    errorCode = (int)ErrorCodes.wrong_version;
                    log.Warn(string.Format("Inserting error code {0}:{1} into upload table", ErrorCodes.wrong_version, (int)ErrorCodes.wrong_version));
                    return null;
                }

                try
                {
                    //Gets all data on the form via tags
                    log.Info(string.Format("Parsing XML."));
                    //docpart.document.firstchild is the entire xml document
                    //if we get the child elements we get a list of first children which should be 9
                    //we select the 3rd child which is the main table that gets filled out
                    var docTable = docPart.Document.FirstChild.ChildElements[2];
                    //the first 2 children of this table are grid settings and properties which we dont care about right now
                    docTable.RemoveAllChildren<TableGrid>();
                    docTable.RemoveAllChildren<TableProperties>();
                    //now we have 29 children of type w:tr which are the rows of the table
                    //select all the table cells inside the current table that arent a section header. 
                    //currently headers start with a number so excluded those
                    var tableCells = docTable.Descendants<TableCell>().Except(docTable.Descendants<TableCell>().Where(x => "0123456789".Contains(x.InnerText.Trim().Substring(0, 1))));

                    //Grab the version cell and add it to ciwInformation
                    var versionNode = xml.SelectSingleNode(string.Format("w:body/w:tbl/w:tr/w:tc/w:tbl/w:tr/w:tc"), nameSpaceManager).NextSibling;
                    ciwInformation.Add(new CIWData { InnerText = versionNode.InnerText, TagName = versionNode.ChildNodes[1].ChildNodes[0].ChildNodes[1].Attributes[0].Value });

                    //get pob country name
                    var placeOfBirthCountryNode = xml.FirstChild.ChildNodes[2].ChildNodes[4].ChildNodes[4];
                    var pobTagname = placeOfBirthCountryNode.ChildNodes[2].FirstChild.ChildNodes[1].Attributes[0].Value;
                    ciwInformation.Add(new CIWData { InnerText = placeOfBirthCountryNode.LastChild.InnerText, TagName = pobTagname + "2" });

                    //get home country name
                    var homeCountry = xml.FirstChild.ChildNodes[2].ChildNodes[6].ChildNodes[2].LastChild.InnerText;
                    var homeTag = xml.FirstChild.ChildNodes[2].ChildNodes[6].ChildNodes[2].ChildNodes[2].FirstChild.ChildNodes[1].Attributes[0].Value;
                    ciwInformation.Add(new CIWData { InnerText = homeCountry, TagName = homeTag + "2" });

                    //get citizenship country
                    var citizenCountry = xml.FirstChild.ChildNodes[2].ChildNodes[9].ChildNodes[4].ChildNodes[2].InnerText;
                    var citizenTag = xml.FirstChild.ChildNodes[2].ChildNodes[9].ChildNodes[4].ChildNodes[2].FirstChild.ChildNodes[1].Attributes[0].Value;
                    ciwInformation.Add(new CIWData { InnerText = citizenCountry, TagName = citizenTag + "2" });

                    //get all table cells and add them after the version in ciwInformation
                    ciwInformation.AddRange(tableCells
                                        .Select
                                            (
                                                s =>
                                                    new CIWData
                                                    {
                                                        TagName = s.ChildElements.OfType<SdtBlock>().FirstOrDefault().GetFirstChild<SdtProperties>().GetFirstChild<Tag>().Val,
                                                        InnerText = ParseXML(s.ChildElements.OfType<SdtBlock>().FirstOrDefault().InnerText, s.OuterXml),
                                                    }
                                            ).ToList());

                    errorCode = (int)ErrorCodes.successfully_processed;
                    return ciwInformation;

                }
                catch (Exception e)
                {
                    log.Warn(string.Format("XML Parsing Failed - {0} with inner exception: {1}", e.Message, e.InnerException));
                    sendWrongVersion(uploaderID, U.fileNameHelper(fileName));
                    errorCode = (int)ErrorCodes.wrong_version;
                    log.Warn(string.Format("Inserting error code {0}:{1} into upload table", ErrorCodes.wrong_version, (int)ErrorCodes.wrong_version));
                    return null;
                }
            }
        }

        /// <summary>
        /// Retrieves the node
        /// </summary>
        /// <param name="innerText"></param>
        /// <param name="outerXML"></param>
        /// <returns>The text object in a field or the selected list item value</returns>
        private string ParseXML(string innerText, string outerXML)
        {
            //if xml contains dropdown list then parse and return value otherwise return inner xml
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

            //Properly retrieve selected value of dropdown items
            else
            {
                if (!String.IsNullOrEmpty(innerText))
                {
                    XmlNode a = xml.SelectSingleNode(string.Format("w:tc/w:sdt/w:sdtPr/w:dropDownList/w:listItem[@w:displayText=\"{0}\"]", innerText), nameSpaceManager);

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

        /// <summary>
        /// Function that is called if wrong version detected.
        /// Calls sendEmail constructor and function to send email for wrong version.
        /// </summary>
        /// <param name="uploaderID"></param>
        /// <param name="fileName"></param>
        private void sendWrongVersion(int uploaderID, string fileName)
        {
            CIWEMails sendEmails = new CIWEMails(uploaderID, "", "", "", "", fileName);

            sendEmails.SendWrongVersion();
        }
    }
}
