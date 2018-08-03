using CsvHelper.Configuration;
using ProcessCIW.Mapping;
using ProcessCIW.Models;
using ProcessCIW.Utilities;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace ProcessCIW.Test.dll
{
    public partial class CiwTest
    {
        #region FileTool Tests

        readonly FileTool ft = new FileTool();

        [Fact]
        public void CreateTempFile_Test()
        {
            var l = new List<CIWData>();
            var file = ft.CreateTempFile(l);
            var result = File.Exists(file);
            Assert.True(result);
            File.Delete(file);
        }

        [Fact]
        public void GetFileData_Test()
        {
            var config = new CsvConfiguration();
            config.Delimiter = "||";
            config.HasHeaderRecord = true;
            config.WillThrowOnMissingField = false;
            config.IsHeaderCaseSensitive = false;
            config.TrimFields = false;
            var l = new List<CIWData>();
            l.Add(new CIWData() { InnerText = "V1", TagName = "Version-Num" });
            l.Add(new CIWData() { InnerText = "ciweqiptest", TagName = "Employee-LastName" });
            l.Add(new CIWData() { InnerText = "Unites States", TagName = "Employee-PlaceOfBirthCountry" });
            var file = ft.CreateTempFile(l);
            var result = ft.GetFileData<CIW, CIWMapping>(file, config);
            Assert.NotNull(result);
            Assert.IsType<List<CIW>>(result);
            File.Delete(file);
        }

        #endregion
    }
}
