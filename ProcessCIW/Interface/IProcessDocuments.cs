using CsvHelper.Configuration;
using FluentValidation.Results;
using ProcessCIW.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace ProcessCIW.Interface
{
    interface IProcessDocuments
    {
        string GetCIWInformation(int uploaderID, string filePath, string fileName, out int errorCode);
        int ProcessCIWInformation(int uploaderID, string filePath, bool isDebug);
    }
}
