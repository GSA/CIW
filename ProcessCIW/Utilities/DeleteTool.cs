using ProcessCIW.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessCIW.Utilities
{
    public class DeleteTool : IDeleteTool
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void deleteOldCsvFiles()
        {
            foreach (string oldCSVFiles in Directory.EnumerateFiles(ConfigurationManager.AppSettings["TEMPFOLDER"], "*.csv"))
            {
                try
                {
                    log.Info(string.Format("Deleting old CSV file {0}.", oldCSVFiles));
                    File.Delete(oldCSVFiles);
                }
                catch (IOException e)
                {
                    log.Error(e.Message + " - " + e.InnerException);
                }
            }
        }

        public void deleteTempCsvFile(string filePath)
        {
            try
            {
                log.Info(string.Format("Deleting Temp CSV File {0}.", filePath));
                File.Delete(filePath);
            }
            catch (IOException e)
            {
                log.Error("Unable to delete temp file" + e.Message);
            }
        }


        /// <summary>
        /// Deletes all files in the list of files passed in
        /// </summary>
        /// <param name="filesToDelete"></param>
        public void DeleteFiles(List<string> filesToDelete)
        {
            foreach (var file in filesToDelete)
            {
                try
                {
                    log.Info(string.Format("Deleting CIW file {0}.", file));
                    File.Delete(file);
                }
                catch (Exception)
                {
                    log.Warn(string.Format("Unable to delete file {0}", file));
                }
                
            }
        }

    }
}
