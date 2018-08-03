using MySql.Data.MySqlClient;
using ProcessCIW.Interface;
using ProcessCIW.Models;
using ProcessCIW.Utilities;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace ProcessCIW.Process
{
    public class ProcessFiles :IProcessFiles
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly IDataAccess da = DataAccess.GetInstance(new MySqlConnection(ConfigurationManager.ConnectionStrings["GCIMS"].ToString()));
        private readonly IUtilities U;        
        private readonly IProcessDocuments pd;
        private readonly IValidateCIW vc;
        private readonly IDecryptFile df;
        private readonly IDeleteTool dt;

        public ProcessFiles()
        {
            IFileTool ft = new FileTool();
            U = new Utilities.Utilities();            
            pd = new ProcessDocuments(da, ft);
            vc = new Validation.ValidateCIW();
            df = new DecryptFile();
            dt = new DeleteTool();
        }

        /// <summary>
        /// Gets unprocessed files, deletes old CSV files, calls process function based on debug mode boolean
        /// </summary>
        public void PreProcessFiles()
        {
            List<UnprocessedFiles> uf;

            log.Info("Getting unprocessed files");

            uf = da.GetUnprocessedFiles();

            dt.deleteOldCsvFiles();

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
        }

        /// <summary>
        /// Processes files while in debug mode, note that in debug mode the files are not encrypted
        /// </summary>
        /// <param name="filesForProcessing"></param>
        private void ProcessDebugFiles(List<UnprocessedFiles> filesForProcessing)
        {
            int processedResult;

            foreach (var ciwFile in filesForProcessing)
            {
                string filePath = ConfigurationManager.AppSettings["CIWDEBUGFILELOCATION"] + ciwFile.FileName;

                int errorCode;

                log.Info(string.Format("Processing file {0}", filePath));

                //Get data from CIW
                string tempFile = pd.GetCIWInformation(ciwFile.PersID, filePath, ciwFile.FileName, out errorCode);

                if (tempFile != null)
                {
                    log.Info(string.Format("GetCIWInformation returned with temp file {0}.", tempFile));

                    //Process the data retrieved from the CIW
                    processedResult = pd.ProcessCIWInformation(ciwFile.PersID, tempFile, true, vc);

                    log.Info(string.Format("ProcessCIWInformation returned with result: {0}", GetErrorMessage(processedResult)));
                    //Update the status of processing the file in the database
                    da.UpdateProcessed(ciwFile.ID, processedResult);
                }
                else
                {
                    //Mark the file as failed in the database
                    da.UpdateProcessed(ciwFile.ID, errorCode);
                }

                try
                {
                    //Delete the original file
                    dt.DeleteFiles(new List<string> { filePath });
                }
                catch (IOException e)
                {
                    log.Error(e.Message + " - " + e.InnerException);
                }
            }
        }

        /// <summary>
        /// Processes files uploaded when not in debug mode
        /// </summary>
        /// <param name="filesForProcessing"></param>
        private void ProcessProdFiles(List<UnprocessedFiles> filesForProcessing)
        {
            int processedResult;

            foreach (var ciwFile in filesForProcessing)
            {
                string filePath = ConfigurationManager.AppSettings["CIWPRODUCTIONFILELOCATION"] + ciwFile.FileName;
                int errorCode;
                log.Info(string.Format("Processing file {0}", filePath));

                string decryptedFile = df.Decrypt(filePath, ciwFile);

                //Gets data from CIW
                string tempFile = pd.GetCIWInformation(ciwFile.PersID, decryptedFile, ciwFile.FileName, out errorCode);

                if (tempFile != null)
                {
                    log.Info(string.Format("GetCIWInformation returned with temp file {0}.", tempFile));

                    //Processes data retrieved from CIW
                    processedResult = pd.ProcessCIWInformation(ciwFile.PersID, tempFile, true, vc);

                    log.Info(string.Format("ProcessCIWInformation returned with result: {0}", processedResult == 1 ? "File processed successfully" : processedResult == 0 ? "File remains unprocessed" : "File failed processing"));

                    //Mark status of processed file in the database
                    da.UpdateProcessed(ciwFile.ID, processedResult);
                }
                else
                    //Mark the file as failed in the database
                    da.UpdateProcessed(ciwFile.ID, errorCode);

                try
                {
                    //Delete the original and decrypted file
                    dt.DeleteFiles(new List<string> { filePath, decryptedFile });
                }
                catch (IOException e)
                {
                    log.Error(e.Message + " - " + e.InnerException);
                }
            }
        }

        private static string GetErrorMessage(int e)
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
    }
}
