using CsvHelper;
using CsvHelper.Configuration;
using ProcessCIW.Interface;
using ProcessCIW.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessCIW.Utilities
{
    public class FileTool : IFileTool
    {
        private readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        /// <summary>
        /// Generates temp CSV file separated by ||
        /// </summary>
        /// <param name="ciwData"></param>
        public string CreateTempFile(List<CIWData> ciwData)
        {
            Guid guid = Guid.NewGuid();
            string csvFileName = guid + ".csv";

            log.Info("CIW Info Count: " + ciwData.Count);

            string fileName = ConfigurationManager.AppSettings["TEMPFOLDER"] + csvFileName;

            try
            {
                using (StreamWriter writer = new StreamWriter(fileName, false))
                {
                    writer.WriteLine(string.Join("||", ciwData.Select(item => item.TagName)));
                    writer.WriteLine(string.Join("||", ciwData.Select(item => item.InnerText)));
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message + " - " + ex.InnerException);
            }

            log.Info(string.Format("Temp csv file {0} created", fileName));

            return fileName;
        }

        /// <summary>
        /// Loads the CIW information
        /// </summary>
        /// <typeparam name="TClass"></typeparam>
        /// <typeparam name="TMap"></typeparam>
        /// <param name="filePath"></param>
        /// <param name="config"></param>
        /// <returns>List of CIW's</returns>
        public List<TClass> GetFileData<TClass, TMap>(string filePath, CsvConfiguration config)
            where TClass : class
            where TMap : CsvClassMap<TClass>
        {
            log.Info(string.Format("Parsing CSV file {0} and mapping to CIW object", filePath));

            using (CsvParser csvParser = new CsvParser(new StreamReader(filePath), config))
            {
                using (CsvReader csvReader = new CsvReader(csvParser))
                {
                    csvReader.Configuration.RegisterClassMap<TMap>();

                    return csvReader.GetRecords<TClass>().ToList();
                }
            }
        }
    }
}
