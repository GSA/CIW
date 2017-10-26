using Gsa.Sftp.Libraries.Utilities.Encryption;
using ProcessCIW.Models;
using ProcessCIW.Utilities;
using ProcessCIW.Validation;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;

namespace ProcessCIW
{
    class Program
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static Stopwatch stopWatch = new Stopwatch();

        private static ProcessDocuments pd = new ProcessDocuments();

        //Need better naming namespace and convention here
        private static Utilities.Utilities u = new Utilities.Utilities();

        static void Main(string[] args)
        {
            //Define unhandled exception delegate
            System.AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;
            throw new Exception();

            //used during logging
            stopWatch.Start();

            log.Info("Application Started");

            //All action takes place here
            ProcessFiles();

            log.Info(string.Format("Processed Adjudications in {0} milliseconds", stopWatch.ElapsedMilliseconds));

            stopWatch.Stop();

            log.Info("Application Done");

            Console.WriteLine("Done! " + stopWatch.ElapsedMilliseconds);

            //End of program
            return;
        }

        /// <summary>
        /// Gets unprocessed files, deletes old CSV files, calls process function based on debug mode boolean
        /// </summary>
        private static void ProcessFiles()
        {
            List<UnprocessedFiles> uf = new List<UnprocessedFiles>();

			log.Info(string.Format("Getting unprocessed files"));
            uf = pd.GetUnprocessedFiles();

            foreach (string oldCSVFiles in Directory.EnumerateFiles(ConfigurationManager.AppSettings["TEMPFOLDER"], "*.csv"))
            {
                try
                {
                    log.Info(string.Format("Deleting old CSV file (0).", oldCSVFiles));
                    File.Delete(oldCSVFiles);
                }
                catch (IOException e)
                {
                    log.Error(e.Message + " - " + e.InnerException);
                }
            }

            if (uf.Count == 0)
            {
                log.Info("No Files Found For Processing");
                return;
            }
            else
                log.Info(string.Format("Found {0} unprocessed files.", uf.Count));

            if (ConfigurationManager.AppSettings["DEBUGMODE"] == "true")
            {
                log.Info("Processing Debug Files");
                ProcessDebugFiles(uf);
            }
            else
            {
                log.Info("Processing Prod Files");
                ProcessProdFiles(uf);
            }

            return;
        }

        /// <summary>
        /// Processes files while in debug mode, note that in debug mode the files are not encrypted
        /// </summary>
        /// <param name="filesForProcessing"></param>
        private static void ProcessDebugFiles(List<UnprocessedFiles> filesForProcessing)
        {
            int processedResult;

            foreach (var ciwFile in filesForProcessing)
            {
				List<CIWData> dupes = new List<CIWData>();
                string filePath = ConfigurationManager.AppSettings["CIWDEBUGFILELOCATION"] + ciwFile.FileName;

                int errorCode;

                log.Info(string.Format("Processing file {0}", filePath));

                //Get data from CIW
                string tempFile = pd.GetCIWInformation(ciwFile.PersID, filePath, ciwFile.FileName, out dupes, out errorCode);

                if (tempFile != null)
                {
                    log.Info(string.Format("GetCIWInformation returned with temp file {0} and had {1} nested field(s).", tempFile, dupes.Count));

                    //Process the data retrieved from the CIW
                    processedResult = pd.ProcessCIWInformation(ciwFile.PersID, tempFile, true, dupes);

                    log.Info(string.Format("ProcessCIWInformation returned with result: {0}", GetErrorMessage(processedResult)));
                    //Update the status of processing the file in the database
                    pd.UpdateProcessed(ciwFile.ID, processedResult);
                }
                else
                {
                    //Mark the file as failed in the database
                    pd.UpdateProcessed(ciwFile.ID, errorCode);
                }

                try
                {
                    //Delete the original file
                    Utilities.Utilities.DeleteFiles(new List<string> { filePath });
                }
                catch (IOException e)
                {
                    log.Error(e.Message + " - " + e.InnerException);
                }
            }
        }

        public static string GetErrorMessage(int e)
        {
            switch (e)
            {
                case -1:
                    return "An unknown error has occurred";
                case 0:
                    return "The file is unprocessed";
                case 1:
                    return "The file was processed successfully";
                case -2:
                    return "The file is password protected";
                case -3:
                    return "The file is the wrong version";
                case -4:
                    return "The file is ARRA";
                case -5:
                    return "The file contains a duplicate user";
                case -6:
                    return "The file failed validation";
                default:
                    return "The error code was not found in the list";
            }
        }

        /// <summary>
        /// Processes files uploaded when not in debug mode
        /// </summary>
        /// <param name="filesForProcessing"></param>
        private static void ProcessProdFiles(List<UnprocessedFiles> filesForProcessing)
        {
            int processedResult;

            foreach (var ciwFile in filesForProcessing)
            {
				List<CIWData> dupes;
                string filePath = ConfigurationManager.AppSettings["CIWPRODUCTIONFILELOCATION"] + ciwFile.FileName;
                int errorCode;
                log.Info(string.Format("Processing file {0}", filePath));

                //Decrypt unprocessed production files
                byte[] buffer = new byte[] { };

                string decryptedFile = string.Empty;

                decryptedFile = ConfigurationManager.AppSettings["CIWPRODUCTIONFILELOCATION"] + u.GenerateDecryptedFilename(Path.GetFileNameWithoutExtension(ciwFile.FileName));

                log.Info(string.Format("Decrypting file {0}.", ciwFile));

                buffer = File.ReadAllBytes(filePath);

                buffer.WriteToFile(decryptedFile, Cryptography.Security.Decrypt, true);

                //Gets data from CIW
                string tempFile = pd.GetCIWInformation(ciwFile.PersID, decryptedFile, ciwFile.FileName, out dupes, out errorCode);

                if (tempFile != null)
                {

                    log.Info(string.Format("GetCIWInformation returned with temp file {0} and had {1} nested field(s).", tempFile, dupes.Count));

                    //Processes data retrieved from CIW
                    processedResult = pd.ProcessCIWInformation(ciwFile.PersID, tempFile, true, dupes);

                    log.Info(string.Format("ProcessCIWInformation returned with result: {0}", processedResult == 1 ? "File processed successfully" : processedResult == 0 ? "File remains unprocessed" : "File failed processing"));

                    //Mark status of processed file in the database
                    pd.UpdateProcessed(ciwFile.ID, processedResult);
                }
                else
                    //Mark the file as failed in the database
                    pd.UpdateProcessed(ciwFile.ID, errorCode);

                try
                {
                    //Delete the original and decrypted file
                    Utilities.Utilities.DeleteFiles(new List<string> { filePath, decryptedFile });
                }
                catch (IOException e)
                {
                    log.Error(e.Message + " - " + e.InnerException);
                }
            }
        }

        static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            log.Fatal("Fatal Error has occurred!");
            log.Fatal(e.ExceptionObject.ToString());
            log.Fatal("Terminating!");

            EMail email = new EMail();
            try
            {
                email.Send
                (
                    ConfigurationManager.AppSettings["DEFAULTEMAIL"].ToString(),        //from
                    ConfigurationManager.AppSettings["FATALEMAIL"].ToString(),          //to
                    "",                                                                 //cc
                    "",                                                                 //bcc
                    "FATAL ERROR IN CIW",                                               //subject
                    e.ExceptionObject.ToString(),                                       //body
                    "",                                                                 //attachments
                    ConfigurationManager.AppSettings["SMTPSERVER"],                     //smtp
                    false                                                               //isbodyhtml
                );
            }
            catch (Exception ex)
            {
                log.Fatal("Fatal Email not sent due to :" + ex.Message + ex.StackTrace);
            }            
            
            Environment.Exit(1);
        }
    }
}