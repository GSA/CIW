using ProcessCIW.Utilities;
using ProcessCIW.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.IO;
using System.Configuration;

namespace ProcessCIW.Test.dll
{
    public partial class CiwTest
    {
        [Theory]
        [MemberData(nameof(DeleteFiles_True))]
        public void DeleteFiles_True_Test(List<string> s)
        {
            foreach (var item in s)
            {
                if (!File.Exists(item))
                {
                    using (var fs = File.Create(item))
                    {
                        fs.Dispose();
                    }

                    dt.DeleteFiles(new List<string>() { item });
                    var result = File.Exists(item);
                    Assert.False(result);
                }
                else
                {
                    Assert.True(false);
                }
            }
        }

        [Theory]
        [MemberData(nameof(DeleteOldFiles_True))]
        public void DeleteOldFiles_True_Test(List<string> s)
        {
            string folder = ConfigurationManager.AppSettings["TEMPFOLDER"];
            foreach (var item in s)
            {
                if (!File.Exists(folder + item))
                {
                    using (var fs = File.Create(folder + item))
                    {
                        fs.Close();
                    }
                                                              
                }                
            }

            dt.deleteOldCsvFiles();
            var result = !Directory.EnumerateFileSystemEntries(folder).Any();
            Assert.True(result);
        }      

        [Theory]
        [MemberData(nameof(DeleteTempCsvFile_True))]
        public void DeleteTempCsvFile_True_Test(string s)
        {
            string folder = ConfigurationManager.AppSettings["TEMPFOLDER"];
            string filePath = folder + s;

            if (!File.Exists(filePath))
            {
                using (var fs = File.Create(filePath))
                {
                    fs.Close();
                }
            }

            dt.deleteTempCsvFile(filePath);
            var result = File.Exists(filePath);
            Assert.False(result);
        }
        public static TheoryData<string> DeleteTempCsvFile_True =>
            new TheoryData<string>
            {
                { "file1.csv" }
            };

        public static TheoryData<List<string>> DeleteOldFiles_True =>
            new TheoryData<List<string>>
            {
                { new List<string>() { "file1.csv", "file2.csv", "file3.csv" } }
            };

        public static TheoryData<List<string>> DeleteFiles_True =>
           new TheoryData<List<string>>
           {
                { new List<string>() { "file.txt", "file2.txt" } }
           };
    }
}
