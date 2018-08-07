using Gsa.Sftp.Libraries.Utilities.Encryption;
using ProcessCIW.Interface;
using ProcessCIW.Models;
using System.Configuration;
using System.IO;

namespace ProcessCIW.Utilities
{
    class DecryptFile : IDecryptFile
    {
        private readonly ILogTool log;
        private readonly IUtilities U;

        public DecryptFile(IUtilities U, ILogTool log)
        {
            this.U = U;
            this.log = log;
        }

        public string Decrypt(string filePath, UnprocessedFiles ciwFile)
        {
            //Decrypt unprocessed production files
            byte[] buffer;

            string decryptedFile = string.Empty;

            decryptedFile = ConfigurationManager.AppSettings["CIWPRODUCTIONFILELOCATION"] + U.GenerateDecryptedFilename(Path.GetFileNameWithoutExtension(ciwFile.FileName));
            
            log.Info(string.Format("Decrypting file {0}.", ciwFile));

            buffer = File.ReadAllBytes(filePath);

            buffer.WriteToFile(decryptedFile, Cryptography.Security.Decrypt, true);

            return decryptedFile;
        }        
    }
}
