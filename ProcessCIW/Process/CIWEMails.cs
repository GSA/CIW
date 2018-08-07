using FluentValidation.Results;
using ProcessCIW.Interface;
using ProcessCIW.Models;
using ProcessCIW.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace ProcessCIW.Process
{
    /// <summary>
    /// All processes that involve email during processing the CIW
    /// </summary>
    public class CiwEmails : ICiwEmails
    {
        private readonly ILogTool log;

        //Reference to basic Email class in Utilities
        readonly EMail email;

        readonly IDataAccess da;

        //Variable declaration and default values
        UploaderInformation uploaderInfo;
        readonly string defaultEMail = ConfigurationManager.AppSettings["DEFAULTEMAIL"].ToString();
        string emailBody = string.Empty;
        string firstName;
        string middleName;
        string lastName;
        string suffix;
        string fileName;
        string subject;
        bool isChildCareWorker;

        public CiwEmails(IDataAccess da, ILogTool log)
        {
            this.da = da;
            this.log = log;
            email = new EMail(log);
        }

        /// <summary>
        /// Constructor to retrieve uploader ID, full name, suffix, filename, and isChildCareWorker
        /// Will then create an email subject line of either name or filename and append a date and time to the end
        /// </summary>
        /// <param name="uID"></param>
        public void Setup(int uID, string firstName, string middleName, string lastName, string suffix, string fileName, bool isChildCareWorker = false)
        {
            int uploaderId = uID;
            this.firstName = firstName;
            this.middleName = middleName;
            this.lastName = lastName;
            this.suffix = suffix;
            this.fileName = fileName;
            this.isChildCareWorker = isChildCareWorker;
            this.subject = FormatSubject();

            uploaderInfo = da.GetUploaderInformation(uploaderId);
            

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
            StringBuilder sbSubject = new StringBuilder();

            sbSubject.Append("Invalid CIW - ");

            //Append file name if first and last name not known
            if (lastName == "" && firstName == "")
                sbSubject.Append(fileName);
            else
            {
                sbSubject.Append(lastName);
                sbSubject.Append(" ");

                if (!suffix.Equals("N/A"))
                {
                    sbSubject.Append(suffix);
                }


                sbSubject.Append(",");
                sbSubject.Append(" ");
                sbSubject.Append(firstName);
                sbSubject.Append(" ");

                if (!middleName.Equals("NMN"))
                    sbSubject.Append(middleName);
            }

            //Append date and time to end
            sbSubject.Append(" - ");

            sbSubject.Append(DateTime.Now.ToShortDateString());
            sbSubject.Append(" - ");
            sbSubject.Append(DateTime.Now.ToShortTimeString());

            return sbSubject.ToString();
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
            return uploaderInfo.UploaderMajorOrg.ToLower().Equals("p") && !isChildCareWorker;

        }

        /// <summary>
        /// Checks if uploaderMajorOrg is "q"
        /// </summary>
        /// <returns>Bool</returns>
        private bool IncludeFASEMail()
        {
            return uploaderInfo.UploaderMajorOrg.ToLower().Equals("q");
        }

        /// <summary>
        /// Function utilizing stringbuilder to generate SendTo string
        /// </summary>
        /// <returns>String of email addresses</returns>
        private string SendTo()
        {
            StringBuilder to = new StringBuilder();

            to.Append(uploaderInfo.UploaderWorkEmail);

            if (IncludeZonalEMail())
            {
                to.Append(",");
                to.Append(uploaderInfo.ZoneEmail);
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
            emailBody = emailBody.Replace("[UploaderPrefixLastName]", uploaderInfo.PrefixedName);
        }

       

        /// <summary>
        /// Called when CIW is Wrong version
        /// </summary>
        public void SendWrongVersion()
        {
            emailBody = File.ReadAllText(@ConfigurationManager.AppSettings["EMAILTEMPLATESLOCATION"] + "VersionError.html");

            log.Info("Sending wrong version E-Mail");

            SendEMail("Invalid CIW");
        }

        /// <summary>
        /// Called when CIW is password protected
        /// </summary>
        public void SendPasswordProtection()
        {
            emailBody = File.ReadAllText(@ConfigurationManager.AppSettings["EMAILTEMPLATESLOCATION"] + "PasswordError.html");

            log.Info("Sending password protection E-Mail");

            SendEMail("Invalid CIW");
        }

        /// <summary>
        /// Called when CIW is a duplicate user
        /// </summary>
        public void SendDuplicateUser()
        {
            emailBody = File.ReadAllText(@ConfigurationManager.AppSettings["EMAILTEMPLATESLOCATION"] + "DuplicateUserError.html");

            log.Info("Sending duplicate user E-Mail");

            SendEMail("Invalid CIW");
        }

        /// <summary>
        /// Called when CIW is ARRA
        /// </summary>
        public void SendARRA()
        {
            emailBody = File.ReadAllText(@ConfigurationManager.AppSettings["EMAILTEMPLATESLOCATION"] + "ARRAError.html");

            log.Info("Sending ARRA E-Mail");

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
            log.Info("Preparing to send errors - generating email body");

            emailBody = File.ReadAllText(@ConfigurationManager.AppSettings["EMAILTEMPLATESLOCATION"] + "Errors.html");

            emailBody = emailBody.Replace("[GENERAL]", "");
            emailBody = emailBody.Replace("[SECTION1]", AddErrors(s1.Errors));
            emailBody = emailBody.Replace("[SECTION2]", AddErrors(s2.Errors));
            emailBody = emailBody.Replace("[SECTION3]", AddErrors(s3.Errors));
            emailBody = emailBody.Replace("[SECTION4]", AddErrors(s4.Errors));
            emailBody = emailBody.Replace("[SECTION5]", AddErrors(s5.Errors));
            emailBody = emailBody.Replace("[SECTION6]", AddErrors(s6.Errors));

            log.Info("Sending error E-Mail");

            SendEMail("Invalid CIW");
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
            Suitability.SendNotification sendNotification;
            try
            {
                log.Info("Begin Sponsorship E-Mail");
                log.Info(string.Format("Sending Sponsorship E-Mail using ID: {0}", id));
                log.Info(string.Format("Using Default Email: {0}", ConfigurationManager.AppSettings["DEFAULTEMAIL"] ));
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
