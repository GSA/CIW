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

namespace ProcessCIW.Test
{
    public class Class1
    {
       

        [Fact]
        public void test1()
        {
            var mockFt = new Mock<IFileTool>();
            mockFt.Setup(ft => ft.CreateTempFile(It.IsAny<List<CIWData>>())).Returns("c://file.file");
            mockFt.Setup(ft => ft.GetFileData<CIW, CIWMapping>("", It.IsAny<CsvConfiguration>())).Returns(new List<CIW>());

            var mockDa = new Mock<IDataAccess>();
            mockDa.Setup(db => db.BeAValidBuilding(It.IsAny<string>())).Returns(true);

            var mockPd = new Mock<IProcessDocuments>();
            int outvar = 0;
            mockPd.Setup(pd => pd.GetCIWInformation(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), out outvar)).Returns("string");
            

            Assert.True(true);
        }



        //arrange
        readonly DataAccess db = DataAccess.GetInstance(new MySqlConnection(ConfigurationManager.ConnectionStrings["GCIMS"].ToString()));

        [Fact]
        public void NotBeDuplicateSSN_Test()
        {
            //act
            var result = db.NotBeADuplicateSSN("000");

            //assert
            Assert.True(result);
        }
        [Fact]
        public void NotBeDuplicateUser_Test()
        {
            var result = db.NotBeADuplicateUser("123", "2050-12-12", "000");
            Assert.True(result);
        }
        [Fact]
        public void ValidateState_Posative_Test()
        {
            var result = db.ValidateState("on", "ca");
            Assert.True(result);
        }
        [Fact]
        public void ValidateState_Negative_Test()
        {
            var result = db.ValidateState("on", "mx");
            Assert.False(result);
        }
        [Fact]
        public void GetUploaderInformation_Test()
        {
            var result = db.GetUploaderInformation(47084);
            Assert.NotNull(result);
        }
        [Fact]
        public void GetUnprocessedFiles_Test()
        {
            var result = db.GetUnprocessedFiles();
            Assert.NotNull(result);
        }
    }
}
