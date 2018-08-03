using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessCIW.Interface
{
    interface IDeleteTool
    {
        void deleteOldCsvFiles();
        void deleteTempCsvFile(string filePath);
        void DeleteFiles(List<string> filesToDelete);
    }
}
