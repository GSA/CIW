using ProcessCIW.Interface;
using System;
using System.Net.Mail;

namespace ProcessCIW.Utilities
{
    /// <summary>
    /// Used for sending emails through a smtp server
    /// </summary>
    class EMail : IDisposable
    {
        private readonly ILogTool log;

        public EMail(ILogTool log)
        {
            this.log = log;
        }

        protected string _strSmtpServer = string.Empty;
        protected string _strEmailFrom = string.Empty;
        protected string _strEmailTo = string.Empty;
        protected string _strEmailCc = string.Empty;
        protected string _strEmailBcc = string.Empty;
        protected string _strEmailSubject = string.Empty;
        protected string _strEmailMessageBody = string.Empty;
        protected string _strEmailAttachments = string.Empty;
        protected bool _IsBodyHtml = false;

        MailMessage message = new MailMessage();
        SmtpClient SmtpMail = new SmtpClient();

        

        /// <summary>
        /// Prepare a message for sending through the smtp server
        /// </summary>
        /// <param name="strEmailFrom"></param>
        /// <param name="strEmailTo"></param>
        /// <param name="strEmailCc"></param>
        /// <param name="strEmailBcc"></param>
        /// <param name="strEmailSubject"></param>
        /// <param name="strEmailMessageBody"></param>
        /// <param name="strEmailAttachments"></param>
        /// <param name="strSmtpServer"></param>
        /// <param name="IsBodyHtml"></param>
        public void Send(string strEmailFrom, string strEmailTo, string strEmailCc, string strEmailBcc, string strEmailSubject,
                         string strEmailMessageBody, string strEmailAttachments, string strSmtpServer, bool IsBodyHtml = false)
        {
            _strEmailFrom = strEmailFrom;
            _strEmailTo = strEmailTo;
            _strEmailCc = strEmailCc;
            _strEmailBcc = strEmailBcc;
            _strEmailSubject = strEmailSubject;
            _strEmailMessageBody = strEmailMessageBody;
            _strEmailAttachments = strEmailAttachments;
            _strSmtpServer = strSmtpServer;
            _IsBodyHtml = IsBodyHtml;

            SendEmail();
        }

        /// <summary>
        /// Sends the email prepared in the Send function
        /// </summary>
        private void SendEmail()
        {
            // Don't attempt an email if there is no smtp server
            if ((!string.IsNullOrEmpty(_strSmtpServer)) && (_strSmtpServer != null))
            {
                try
                {
                    // Create Mail object
                    message = new MailMessage();

                    // Set properties needed for the email
                    MailAddress mail_from = new MailAddress(_strEmailFrom);

                    message.From = mail_from;

                    //message.To.Add(_strEmailTo);
                    if (_strEmailTo.Trim().Length > 0)

                        message.To.Add(_strEmailTo);

                    if (_strEmailCc.Trim().Length > 0)

                        message.CC.Add(_strEmailCc);

                    if (_strEmailBcc.Trim().Length > 0)

                        message.Bcc.Add(_strEmailBcc);

                    message.Subject = _strEmailSubject;
                    message.Body = _strEmailMessageBody;
                    message.IsBodyHtml = _IsBodyHtml;

                    // TO-DO: Include additional validation for all parameter fields (RegEx)
                    if (_strEmailAttachments.IndexOf(";", StringComparison.CurrentCulture) > 0)
                    {
                        // Split multiple attachments into a string array
                        Array a = _strEmailAttachments.Split(';');

                        // Loop through attachments list and add to objMail.Attachments one at a time
                        for (int i = 0; i < a.Length; i++)
                        {
                            message.Attachments.Add(new Attachment(a.GetValue(i).ToString().Trim()));
                        }

                    }
                    else if (_strEmailAttachments.Length > 0) // Single attachment without trailing separator
                    {
                        message.Attachments.Add(new Attachment(_strEmailAttachments.ToString().Trim()));
                    }

                    // Set the mail object's smtp server property
                    SmtpMail = new SmtpClient(_strSmtpServer);

                    //-->SmtpMail.Credentials = (ICredentialsByHost)CredentialCache.DefaultNetworkCredentials;
                    SmtpMail.Send(message);

                    SmtpMail.Dispose();
                    message.Dispose();
                }
                catch (SmtpFailedRecipientsException ex)
                {
                    string smtpfailedrecipients_msg = string.Empty;

                    for (int i = 0; i < ex.InnerExceptions.Length; i++)
                    {
                        SmtpStatusCode status = ex.InnerExceptions[i].StatusCode;

                        if (status == SmtpStatusCode.MailboxBusy ||
                            status == SmtpStatusCode.MailboxUnavailable)
                        {
                            // do nothing
                        }
                        else
                        {
                            log.Error(String.Format("Failed to deliver message to {0}", ex.InnerExceptions[i].FailedRecipient));
                            smtpfailedrecipients_msg += String.Format("Failed to deliver message to {0}\n",
                                ex.InnerExceptions[i].FailedRecipient);
                        }
                    }
                    throw new SmtpException(smtpfailedrecipients_msg, ex);
                }
                finally
                {
                    SmtpMail.Dispose();
                    message.Dispose();
                }
            }
        }

        /// <summary>
        /// Disposes of object
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose managed resources
                SmtpMail.Dispose();
                message.Dispose();
            }
            // free native resources
        }

        /// <summary>
        /// Disposes of object
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}