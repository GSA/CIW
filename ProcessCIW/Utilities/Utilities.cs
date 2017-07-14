﻿using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ProcessCIW.Utilities
{
    sealed class Utilities
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// SHA256 Hash of the SSN, pass in the full 9 or the last 4
        /// </summary>
        /// <param name="ssn"></param>
        /// <returns>Hashed version of SSN passed in</returns>
        public byte[] HashSSN(string ssn)
        {
            byte[] hashedSSN = null;

            SHA256 shaM = new SHA256Managed();

            ssn = ssn.Replace("-", string.Empty).Trim();

            //Using UTF8 because this only contains ASCII text            
            hashedSSN = shaM.ComputeHash(Encoding.UTF8.GetBytes(ssn));

            shaM.Dispose();

            return hashedSSN;
        }

        /// <summary>
        /// Adds -d.docx to end of filename
        /// </summary>
        /// <param name="encryptedFilename"></param>
        /// <returns></returns>
        public string GenerateDecryptedFilename(string encryptedFilename)
        {
            return string.Concat(encryptedFilename, "-d.docx");
        }

        /// <summary>
        /// Deletes all files in the list of files passed in
        /// </summary>
        /// <param name="filesToDelete"></param>
        public static void DeleteFiles(List<string> filesToDelete)
        {
            foreach (var file in filesToDelete)
            {
                log.Info(string.Format("Deleting CIW file {0}.", file));
                File.Delete(file);
            }
        }
    }
}