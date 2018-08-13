using ProcessCIW.Utilities;
using System.Configuration;
using MySql.Data.MySqlClient;
using ProcessCIW.Process;

namespace ProcessCIW.Test.dll
{
    public partial class CiwTest
    {
        static XmlTool xmlTool;
        static readonly DataAccess db;
        static readonly DeleteTool dt;
        static readonly Utilities.Utilities util;
        static readonly CiwEmails ce;
        static readonly LogTool log;
        static readonly MockTool mt;

        static CiwTest()
        {
            mt = new MockTool();
            var dataAccessMock = mt.createDataAccessMock();
            var decryptMock = mt.createDecryptFileMock();
            var fileToolMock = mt.createFileToolMock();
            var xmlToolMock = mt.createXmlToolMock();
            var ciwEmailmock = mt.createCiwEmailMock();
            var logMock = mt.createLogMock();
            var utilMock = mt.createUtilMock();
            var deleteMock = mt.createDeleteMock();
            var validationmock = mt.createValidationMock();

            log = LogTool.GetInstance();
            util = new Utilities.Utilities();
            dt = new DeleteTool(logMock.Object);
            db = DataAccess.GetInstance(new MySqlConnection(ConfigurationManager.ConnectionStrings["GCIMS"].ToString()), utilMock.Object, logMock.Object);

            ce = new CiwEmails(db, logMock.Object);            
            
            xmlTool = new XmlTool(utilMock.Object, ciwEmailmock.Object, /*db,*/ logMock.Object);

        }








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
