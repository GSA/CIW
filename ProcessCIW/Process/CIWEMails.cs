using FluentValidation.Results;
using MySql.Data.MySqlClient;
using ProcessCIW.Models;
using ProcessCIW.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace ProcessCIW.Process
{
    /// <summary>
    /// All processes that involve email during processing the CIW
    /// </summary>
    class CIWEMails
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Object reference to Suitability.DLL
        private static Suitability.SendNotification sendNotification;

        //Reference to basic Email class in Utilities
        EMail email = new EMail();

        //Variable declaration and default values
        int uploaderID = 0;
        string prefixedName = string.Empty;
        string zonalEMail = string.Empty;
        string uploaderWorkEMail = string.Empty;
        string uploaderMajorOrg = string.Empty;
        string defaultEMail = ConfigurationManager.AppSettings["DEFAULTEMAIL"].ToString();
        string emailBody = string.Empty;
        string firstName = string.Empty;
        string middleName = string.Empty;
        string lastName = string.Empty;
        string suffix = string.Empty;
        string fileName = string.Empty;
        string subject = string.Empty;
        bool isChildCareWorker;
        bool isInvalid = false;

        /// <summary>
        /// Constructor to retrieve uploader ID, full name, suffix, filename, and isChildCareWorker
        /// Will then create an email subject line of either name or filename and append a date and time to the end
        /// </summary>
        /// <param name="uID"></param>
        public CIWEMails(int uID, string firstName, string middleName, string lastName, string suffix, string fileName, bool isChildCareWorker = false)
        {
            this.uploaderID = uID;
            this.firstName = firstName;
            this.middleName = middleName;
            this.lastName = lastName;
            this.suffix = suffix;
            this.fileName = fileName;
            this.isChildCareWorker = isChildCareWorker;

            this.subject = FormatSubject();

            GetUploaderInformation();
        }

        /// <summary>
        /// Generates subject line string
        /// Starts with "Invalid CIW - "
        /// Then chooses last and first name if the exist
        /// Else it chooses the file name
        /// Then appends date and time to end
        /// </summary>
        /// <returns></returns>
        private string FormatSubject()
        {
            StringBuilder subject = new StringBuilder();

            subject.Append("Invalid CIW - ");

            //Append file name if first and last name not known
            if (lastName == "" && firstName == "")
                subject.Append(fileName);
            else
            {
                subject.Append(lastName);
                subject.Append(" ");

                if (!suffix.Equals("N/A"))
                {
                    subject.Append(suffix);
                }


                subject.Append(",");
                subject.Append(" ");
                subject.Append(firstName);
                subject.Append(" ");

                if (!middleName.Equals("NMN"))
                    subject.Append(middleName);
            }

            //Append date and time to end
            subject.Append(" - ");

            subject.Append(DateTime.Now.ToShortDateString());
            subject.Append(" - ");
            subject.Append(DateTime.Now.ToShortTimeString());

            return subject.ToString();
        }

        /// <summary>
        /// The final function of the CIWEmail class before emails are sent through the smtp server
        /// Has many references so using CallerMemberName for logging to get source of request to SendEmail
        /// </summary>
        /// <param name="memberName"></param>
        private void SendEMail([CallerMemberName] string memberName = "")
        {
            GenerateEMailBody();

            string sendTo = SendTo();

            log.Info(String.Format("Sending Email to {0} with subject:{1} called from function:{2}", sendTo, subject, memberName));
            email.Send(defaultEMail, sendTo, "", ConfigurationManager.AppSettings["BCCEMAIL"], subject, emailBody, "", ConfigurationManager.AppSettings["SMTPSERVER"], true);
        }

        /// <summary>
        /// Checks if uploaderMajorOrg is "p" and not Child care worker
        /// </summary>
        /// <returns>Bool</returns>
        private bool IncludeZonalEMail()
        {
            return (uploaderMajorOrg.ToLower().Equals("p") && !isChildCareWorker) || isInvalid;

        }

        /// <summary>
        /// Checks if uploaderMajorOrg is "q"
        /// </summary>
        /// <returns>Bool</returns>
        private bool IncludeFASEMail()
        {
            return uploaderMajorOrg.ToLower().Equals("q");
        }

        /// <summary>
        /// Function utilizing stringbuilder to generate SendTo string
        /// </summary>
        /// <returns>String of email addresses</returns>
        private string SendTo()
        {
            StringBuilder to = new StringBuilder();

            to.Append(uploaderWorkEMail);

            if (IncludeZonalEMail())
            {
                to.Append(",");
                to.Append(zonalEMail);
            }

            if (IncludeFASEMail())
            {
                to.Append(",");
                to.Append(ConfigurationManager.AppSettings["FASEMAIL"].ToString());
            }

            if (isChildCareWorker)
            {
                to.Append(",");
                to.Append(ConfigurationManager.AppSettings["CHILDCAREEMAIL"].ToString());
            }

            return to.ToString().TrimEnd(',');
        }

        /// <summary>
        /// Replace UploaderPrefixLastName with prefixedName
        /// </summary>
        private void GenerateEMailBody()
        {
            emailBody = emailBody.Replace("[UploaderPrefixLastName]", prefixedName);
        }

        /// <summary>
        /// Function that calls stored procedure to retrieve uploader information based on uploader ID.
        /// Values converted to strings and stored in previously declared variables.
        /// </summary>
        private void GetUploaderInformation()
        {
            MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["GCIMS"].ToString());
            MySqlCommand cmd = new MySqlCommand();

            using (conn)
            {
                conn.Open();

                using (cmd)
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "CIW_GetUploaderInfo";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Clear();

                    MySqlParameter[] ContractHeaderParameters = new MySqlParameter[]
                    {
                        new MySqlParameter { ParameterName = "UploaderId", Value = uploaderID, MySqlDbType = MySqlDbType.Int32, Direction = ParameterDirection.Input },

                        new MySqlParameter { ParameterName = "PrefixedName", MySqlDbType=MySqlDbType.VarChar, Size=64, Direction = ParameterDirection.Output },
                        new MySqlParameter { ParameterName = "ZoneLetter", MySqlDbType=MySqlDbType.VarChar, Size=2, Direction = ParameterDirection.Output },
                        new MySqlParameter { ParameterName = "ZoneEmail", MySqlDbType=MySqlDbType.VarChar, Size=64, Direction = ParameterDirection.Output },
                        new MySqlParameter { ParameterName = "ZonePhone", MySqlDbType=MySqlDbType.VarChar, Size=22, Direction = ParameterDirection.Output },
                        new MySqlParameter { ParameterName = "UploaderWorkEmail", MySqlDbType=MySqlDbType.VarChar, Size=64, Direction = ParameterDirection.Output },
                        new MySqlParameter { ParameterName = "UploaderMajorOrg", MySqlDbType=MySqlDbType.VarChar, Size=2, Direction = ParameterDirection.Output },
                    };

                    cmd.Parameters.AddRange(ContractHeaderParameters);

                    cmd.ExecuteNonQuery();

                    prefixedName = (string)cmd.Parameters["PrefixedName"].Value;
                    zonalEMail = (string)cmd.Parameters["ZoneEmail"].Value;
                    uploaderWorkEMail = (string)cmd.Parameters["UploaderWorkEmail"].Value;
                    uploaderMajorOrg = (string)cmd.Parameters["UploaderMajorOrg"].Value;

                    log.Info(String.Format("GetUploaderInformation completed with PrefixedName:{0} UploaderWorkEmail:{1} UploaderMajorOrg:{2}", prefixedName, uploaderWorkEMail, uploaderMajorOrg));
                }
            }
        }        

        /// <summary>
        /// Called when CIW is Wrong version
        /// </summary>
        public void SendWrongVersion()
        {
            emailBody = File.ReadAllText(@ConfigurationManager.AppSettings["EMAILTEMPLATESLOCATION"] + "VersionError.html");
            isInvalid = true;
            log.Info(string.Format("Sending wrong version E-Mail"));

            SendEMail("Invalid CIW");
        }

        /// <summary>
        /// Called when CIW is password protected
        /// </summary>
        public void SendPasswordProtection()
        {
            emailBody = File.ReadAllText(@ConfigurationManager.AppSettings["EMAILTEMPLATESLOCATION"] + "PasswordError.html");
            isInvalid = true;
            log.Info(string.Format("Sending password protection E-Mail"));

            SendEMail("Invalid CIW");
        }

        /// <summary>
        /// Called when CIW is a duplicate user
        /// </summary>
        public void SendDuplicateUser()
        {
            emailBody = File.ReadAllText(@ConfigurationManager.AppSettings["EMAILTEMPLATESLOCATION"] + "DuplicateUserError.html");
            isInvalid = true;
            log.Info(string.Format("Sending duplicate user E-Mail"));

            SendEMail("Invalid CIW");
        }

        /// <summary>
        /// Called when CIW is ARRA
        /// </summary>
        public void SendARRA()
        {
            emailBody = File.ReadAllText(@ConfigurationManager.AppSettings["EMAILTEMPLATESLOCATION"] + "ARRAError.html");
            isInvalid = true;
            log.Info(string.Format("Sending ARRA E-Mail"));

            SendEMail("Invalid CIW");
        }

        /// <summary>
        /// Function to replace placeholder text in email template with actual error messages.
        /// Email template is divided by section, each sections errors are passed in as separate objects
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <param name="s3"></param>
        /// <param name="s4"></param>
        /// <param name="s5"></param>
        /// <param name="s6"></param>
        /// <param name="nested_Err"></param>
        /// <param name="nested_List"></param>
        public void SendErrors(ValidationResult s1, ValidationResult s2, ValidationResult s3, ValidationResult s4, ValidationResult s5, ValidationResult s6)
        {
            log.Info(string.Format("Preparing to send errors - generating email body"));
            isInvalid = true;
            emailBody = File.ReadAllText(@ConfigurationManager.AppSettings["EMAILTEMPLATESLOCATION"] + "Errors.html");

            emailBody = emailBody.Replace("[GENERAL]", "");
            emailBody = emailBody.Replace("[SECTION1]", AddErrors(s1.Errors));
            emailBody = emailBody.Replace("[SECTION2]", AddErrors(s2.Errors));
            emailBody = emailBody.Replace("[SECTION3]", AddErrors(s3.Errors));
            emailBody = emailBody.Replace("[SECTION4]", AddErrors(s4.Errors));
            emailBody = emailBody.Replace("[SECTION5]", AddErrors(s5.Errors));
            emailBody = emailBody.Replace("[SECTION6]", AddErrors(s6.Errors));

            log.Info(string.Format("Sending error E-Mail"));

            SendEMail("Invalid CIW");
        }

        /// <summary>
        /// Generates list of nested errors to be added to email template
        /// </summary>
        /// <param name="failures"></param>
        /// <param name="nestedErrors"></param>
        /// <returns>String of nested errors</returns>
        private string AddNestedErrors(IList<ValidationFailure> failures, List<CIWData> nestedErrors)
        {
            StringBuilder errors = new StringBuilder();

            if (failures.Count != 0)
                errors.Append("The following fields have unspecified errors. Please delete all data from these fields and re-input the data manually.");

            errors.Append("<ul>");

            if (failures.Count == 0)
            {
                errors.Append("<li>");
                errors.Append("No Errors Found");
                errors.Append("</li>");

                return errors.ToString();
            }

            foreach (var nested in nestedErrors)
            {
                errors.Append("<li>");
                errors.Append(nested.TagName);
                errors.Append("</li>");
            }

            errors.Append("</ul>");

            return errors.ToString();
        }

        /// <summary>
        /// Helper function that generates the actual error list for each section
        /// </summary>
        /// <param name="failures"></param>
        /// <returns>String of errors in HTML format in an unordered list</returns>
        private string AddErrors(IList<ValidationFailure> failures)
        {
            StringBuilder errors = new StringBuilder();

            errors.Append("<ul>");

            if (failures.Count == 0)
            {
                errors.Append("<li>");
                errors.Append("No Errors Found");
                errors.Append("</li>");

                return errors.ToString();
            }

            foreach (var rule in failures)
            {
                errors.Append("<li>");
                errors.Append(rule.ErrorMessage);
                errors.Append("</li>");
            }

            errors.Append("</ul>");

            return errors.ToString();
        }

        /// <summary>
        /// If able to process CIW, sends email that sponsorship process has been initiated
        /// </summary>
        /// <param name="id"></param>
        public void SendSponsorshipEMail(int id)
        {
            
            try
            {
                log.Info("Begin Sponsorship E-Mail");
                log.Info(string.Format("Sending Sponsorship E-Mail using ID: {0}", id));
                log.Info(string.Format("Using Default Email: {0}", ConfigurationManager.AppSettings["DEFAULTEMAIL"] ));
                //log.Info(string.Format("Subject: {0}", subject));
                //log.Info(string.Format("Zonal email is: {0}",zonalEMail));
                log.Info(string.Format("isChildCareWorker: {0}", isChildCareWorker));

                sendNotification = new Suitability.SendNotification(
                                    ConfigurationManager.AppSettings["DEFAULTEMAIL"],
                                    id,
                                    ConfigurationManager.ConnectionStrings["GCIMS"].ToString(),
                                    ConfigurationManager.AppSettings["SMTPSERVER"],
                                    ConfigurationManager.AppSettings["ONBOARDINGLOCATION"]);

                sendNotification.SendSponsorshipNotification();

                log.Info("Finished sending sponsorship notification");
            }
            catch (Exception ex)
            {
                log.Error("Error E-Mailing Sponsorship: " + ex.Message + " - " + ex.InnerException);
            }
        }
    }
}
