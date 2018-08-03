using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using ProcessCIW.Models;
using ProcessCIW.Utilities;
using ProcessCIW.Interface;
using ProcessCIW.Mapping;
using ProcessCIW;
using Moq;
using CsvHelper.Configuration;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.IO;

namespace ProcessCIW.Test
{
    public class CiwTest
    {
        

        //[Fact]
        //public void test1()
        //{
        //    var mockFt = new Mock<IFileTool>();
        //    mockFt.Setup(ft => ft.CreateTempFile(It.IsAny<List<CIWData>>())).Returns("c://file.file");
        //    mockFt.Setup(ft => ft.GetFileData<CIW, CIWMapping>("", It.IsAny<CsvConfiguration>())).Returns(new List<CIW>());

        //    var mockDa = new Mock<IDataAccess>();
        //    mockDa.Setup(db => db.BeAValidBuilding(It.IsAny<string>())).Returns(true);

        //    var mockPd = new Mock<IProcessDocuments>();
        //    int outvar = 0;
        //    mockPd.Setup(pd => pd.GetCIWInformation(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), out outvar)).Returns("string");

        //    Assert.True(true);
        //}




    }
}
