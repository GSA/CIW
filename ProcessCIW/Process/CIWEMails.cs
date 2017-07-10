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
    class CIWEMails
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static Suitability.SendNotification sendNotification;

        EMail email = new EMail();

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

        /// <summary>
        /// Pass in the uploader ID
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

        private void SendEMail([CallerMemberName] string memberName = "")
        {
            GenerateEMailBody();

            string sendTo = SendTo();

            log.Info(String.Format("Sending Email to {0} with subject:{1} called from function:{2}", sendTo, subject, memberName));

            email.Send(defaultEMail, sendTo, "", "terry.saunders@gsa.gov", subject, emailBody, "", ConfigurationManager.AppSettings["SMTPSERVER"], true);
        }

        private bool IncludeZonalEMail()
        {
            return uploaderMajorOrg.ToLower().Equals("p");
        }

        private bool IncludeFASEMail()
        {
            return uploaderMajorOrg.ToLower().Equals("q");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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

        private void GenerateEMailBody()
        {
            emailBody = emailBody.Replace("[UploaderPrefixLastName]", prefixedName);
        }

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

        public void SendChildCareWorker()
        {
            emailBody = File.ReadAllText(@ConfigurationManager.AppSettings["EMAILTEMPLATESLOCATION"] + "Tier1CError.html");

            log.Info(string.Format("Sending child care worker E-Mail"));

            SendEMail(FormatSubject());
        }

        public void SendWrongVersion()
        {
            emailBody = File.ReadAllText(@ConfigurationManager.AppSettings["EMAILTEMPLATESLOCATION"] + "VersionError.html");

            log.Info(string.Format("Sending wrong version E-Mail"));

            SendEMail("Invalid CIW");
        }

        public void SendPasswordProtection()
        {
            emailBody = File.ReadAllText(@ConfigurationManager.AppSettings["EMAILTEMPLATESLOCATION"] + "PasswordError.html");

            log.Info(string.Format("Sending password protection E-Mail"));

            SendEMail("Invalid CIW");
        }

        public void SendDuplicateUser()
        {
            emailBody = File.ReadAllText(@ConfigurationManager.AppSettings["EMAILTEMPLATESLOCATION"] + "DuplicateUserError.html");

            log.Info(string.Format("Sending duplicate user E-Mail"));

            SendEMail("Invalid CIW");
        }

        public void SendARRA()
        {
            emailBody = File.ReadAllText(@ConfigurationManager.AppSettings["EMAILTEMPLATESLOCATION"] + "ARRAError.html");

            log.Info(string.Format("Sending ARRA E-Mail"));

            SendEMail("Invalid CIW");
        }

        public void SendErrors(ValidationResult s1, ValidationResult s2, ValidationResult s3, ValidationResult s4, ValidationResult s5, ValidationResult s6, ValidationResult nested_Err, List<CIWData> nested_List)
        {
            log.Info(string.Format("Preparing to send errors - generating email body"));

            emailBody = File.ReadAllText(@ConfigurationManager.AppSettings["EMAILTEMPLATESLOCATION"] + "Errors.html");

            emailBody = emailBody.Replace("[GENERAL]", AddNestedErrors(nested_Err.Errors, nested_List));
            emailBody = emailBody.Replace("[SECTION1]", AddErrors(s1.Errors));
            emailBody = emailBody.Replace("[SECTION2]", AddErrors(s2.Errors));
            emailBody = emailBody.Replace("[SECTION3]", AddErrors(s3.Errors));
            emailBody = emailBody.Replace("[SECTION4]", AddErrors(s4.Errors));
            emailBody = emailBody.Replace("[SECTION5]", AddErrors(s5.Errors));
            emailBody = emailBody.Replace("[SECTION6]", AddErrors(s6.Errors));

            log.Info(string.Format("Sending error E-Mail"));

            SendEMail("Invalid CIW");
        }

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

        public void SendSponsorshipEMail(int id)
        {
            try
            {
                log.Info("Begin Sponsorship E-Mail");                

                sendNotification = new Suitability.SendNotification(
                                    ConfigurationManager.AppSettings["DEFAULTEMAIL"],
                                    id,
                                    ConfigurationManager.ConnectionStrings["GCIMS"].ToString(),
                                    ConfigurationManager.AppSettings["SMTPSERVER"],
                                    ConfigurationManager.AppSettings["ONBOARDINGLOCATION"]);

                sendNotification.SendSponsorshipNotification();
            }
            catch (Exception ex)
            {
                log.Error("E-Mailing: " + ex.Message + " - " + ex.InnerException);
            }
        }
    }
}