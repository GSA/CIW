
namespace ProcessCIW.Models
{
    /// <summary>
    /// When checking for files that need to be processed, if files are found, UnprocessedFiles objects are created and returned.
    /// </summary>
    class UnprocessedFiles
    {
        public int ID { get; set; }
        public int PersID { get; set; }
        public string FileName { get; set; }        
    }
}
