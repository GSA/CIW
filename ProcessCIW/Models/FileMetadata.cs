
namespace ProcessCIW.Models
{
    /// <summary>
    /// When checking for files that need to be processed, if files are found, UnprocessedFiles objects are created and returned.
    /// </summary>
    class FileMetadata
    {
        public int ID { get; set; }
        public int UploaderPersID { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string TempFilePath { get; set; }
        public string DecryptedFilePath { get; set; }
    }
}
