using ProcessCIW.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessCIW.Interface
{
    public interface IXmlTool
    {
        bool isPasswordProtected(string filePath);
        List<CIWData> parseCiwDocument(string filePath, int uploaderID, string fileName, out int errorCode);

    }
}
