using ProcessCIW.Interface;
using System;
using System.Configuration;
using System.IO;

namespace ProcessCIW.Utilities
{
    class UnhandledException
    {
        private static readonly ILogTool log = LogTool.GetInstance();

        private static string GetErrorRecursive(Exception e)
        {
            if (e.InnerException == null)
                return e.Message + '\n' + e.StackTrace;
            else return e.Message + '\n' + e.StackTrace + '\n' + GetErrorRecursive(e.InnerException);
        }

        private static string PrepareFatalErrorBody(string error)
        {
            string body = File.ReadAllText(ConfigurationManager.AppSettings["EMAILTEMPLATESLOCATION"].ToString() + "FatalError.html");
            body = body.Replace("[ERRORMESSAGE]", error);
            body = body.Replace("[DATETIME]", DateTime.Now.ToString());
            return body;
        }

        public static void UnhandledExceptionTrap(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            log.Fatal("Fatal Error has occurred!");
            log.Fatal(e.ExceptionObject.ToString());
            log.Fatal("Terminating!");

            EMail email = new EMail(log);
            try
            {
                email.Send
                (
                    ConfigurationManager.AppSettings["DEFAULTEMAIL"].ToString(),        //from
                    ConfigurationManager.AppSettings["FATALEMAIL"].ToString(),          //to
                    "",                                                                 //cc
                    "",                                                                 //bcc
                    "FATAL ERROR IN CIW",                                               //subject
                    PrepareFatalErrorBody(GetErrorRecursive(ex)),                       //body
                    "",                                                                 //attachments
                    ConfigurationManager.AppSettings["SMTPSERVER"],                     //smtp
                    true                                                                //isbodyhtml
                );
            }
            catch (Exception x)
            {
                log.Fatal("Fatal Email not sent due to :" + x.Message + x.StackTrace);
            }

            Environment.Exit(1);
        }
    }
}
