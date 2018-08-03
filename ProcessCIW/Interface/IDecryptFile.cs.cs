using ProcessCIW.Models;

namespace ProcessCIW.Interface
{
    interface IDecryptFile
    {
        string Decrypt(string filePath, UnprocessedFiles ciwFile);
    }
}
