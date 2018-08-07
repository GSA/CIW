using ProcessCIW.Models;

namespace ProcessCIW.Interface
{
    public interface IDecryptFile
    {
        string Decrypt(string filePath, UnprocessedFiles ciwFile);
    }
}
