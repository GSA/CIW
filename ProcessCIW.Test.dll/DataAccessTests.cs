using MySql.Data.MySqlClient;
using ProcessCIW.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ProcessCIW.Test.dll
{
    public partial class CiwTest
    {
        #region DataAccess Tests
        
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

        [Theory]
        [MemberData(nameof(Buildings_True))]
        public void BeAValidBuilding_True_Test(string s1)
        {
            var result = db.BeAValidBuilding(s1);
            Assert.True(result);
        }
        [Theory]
        [MemberData(nameof(Building_False))]
        public void BeAValidBuilding_False_Test(string s1)
        {
            var result = db.BeAValidBuilding(s1);
            Assert.False(result);
        }
        [Theory]
        [MemberData(nameof(Email_True))]
        public void BeAValidEmail_True_Test(string s1)
        {
            var result = db.BeAValidEMail(s1);
            Assert.True(result);
        }
        [Theory]
        [MemberData(nameof(FipsCodes_True))]
        public void GetFipsCodesFromCountryName_True_Test(string s1, string s2, string s3, string s4)
        {
            var result = db.GetFipsCodeFromCountryName(s1, s2, s3);
            Assert.NotNull(result);
            Assert.NotEmpty(result.Tables);
            Assert.NotEmpty(result.Tables[0].Rows);
            Assert.NotEmpty(result.Tables[1].Rows);
            Assert.NotEmpty(result.Tables[2].Rows);
            Assert.NotNull(result.Tables[0].Rows[0]);
            Assert.NotNull(result.Tables[1].Rows[0]);
            Assert.NotNull(result.Tables[2].Rows[0]);
            Assert.Equal(s4, result.Tables[0].Rows[0].ItemArray[0].ToString());
            Assert.Equal(s4, result.Tables[1].Rows[0].ItemArray[0].ToString());
            Assert.Equal(s4, result.Tables[2].Rows[0].ItemArray[0].ToString());
        }

        [Theory]
        [MemberData(nameof(FipsCodes_False))]
        public void GetFipsCodesFromCountryName_False_Test(string s1, string s2, string s3, string s4)
        {
            var result = db.GetFipsCodeFromCountryName(s1, s2, s3);
            Assert.NotNull(result);
            Assert.NotEmpty(result.Tables);
            Assert.NotEmpty(result.Tables[0].Rows);
            Assert.NotEmpty(result.Tables[1].Rows);
            Assert.NotEmpty(result.Tables[2].Rows);
            Assert.NotNull(result.Tables[0].Rows[0]);
            Assert.NotNull(result.Tables[1].Rows[0]);
            Assert.NotNull(result.Tables[2].Rows[0]);
            Assert.NotEqual(s4, result.Tables[0].Rows[0].ItemArray[0].ToString());
            Assert.NotEqual(s4, result.Tables[1].Rows[0].ItemArray[0].ToString());
            Assert.NotEqual(s4, result.Tables[2].Rows[0].ItemArray[0].ToString());
        }

        [Theory]
        [MemberData(nameof(NotBeDuplicateSSN_True))]
        public void NotBeADuplicateSSN_True_Test(string s1)
        {
            var result = db.NotBeADuplicateSSN(s1);
            Assert.True(result);
        }

        [Theory]
        [MemberData(nameof(ValidateState_True))]
        public void ValidateState_Posative_Test(string state, string country)
        {
            var result = db.ValidateState(state, country);
            Assert.True(result);
        }
        [Theory]
        [MemberData(nameof(ValidateState_False))]
        public void ValidateState_Negative_Test(string state, string country)
        {
            var result = db.ValidateState(state, country);
            Assert.False(result);
        }

        public static TheoryData<string, string> ValidateState_True =>
           new TheoryData<string, string>
           {
                { "ON", "CA"}, { "BA", "MX"}, { "HL", "MX"}, { "NF", "CA"}
           };

        public static TheoryData<string, string> ValidateState_False =>
            new TheoryData<string, string>
            {
                 { "WV", "US" }, { "US", "MX" }, { "CA", "NF"}, { "", "US"}, { "KS", "US"}
            };

        public static TheoryData<string> NotBeDuplicateSSN_True =>
            new TheoryData<string>
            {
                { "0000000000" }, { "12345678" }, { "ABCDEFGHIJK" }, { "" }
            };

        public static TheoryData<string, string, string, string> FipsCodes_False =>
            new TheoryData<string, string, string, string>
            {
                { "Canada", "Canada", "Canada", "ca" },
                { "United States", "United States", "United States", "us" },
                { "United States", "United States", "United States", "Us" },
                { "United States", "United States", "United States", "uS" },
                { "United States", "United States", "United States", "USA" }
            };

        public static TheoryData<string, string, string, string> FipsCodes_True =>
            new TheoryData<string, string, string, string>
            {
                { "Canada","Canada","Canada","CA" },
                { "United States", "United States", "United States", "US" },
                { "Mexico", "Mexico", "Mexico", "MX"},
                { "France", "France", "France", "FR"}
            };
        public static TheoryData<string> Email_True =>
            new TheoryData<string>
            {
                { "jason.goodell@gsa.gov" }, { "terry.saunders@gsa.gov" }
            };
        public static TheoryData<string> Building_False =>
            new TheoryData<string>
            {
                { "building1" }, { "12345" }, { "AA0000" }
            };

        //Small subsute of total number of buildings. One from each state
        public static TheoryData<string> Buildings_True =>
            new TheoryData<string>
            {
                { "AK0001" }, {"AL0003" }, {"AQ6148" }, {"AR0016" }, {"AZ0011" },
                { "CA0041" }, {"CO0006" }, {"CT0013" }, {"DC0007" }, {"DE0016" },
                { "FL0002" }, {"GA0005" }, {"GU6839" }, {"HI0001" }, {"IA0013" },
                { "ID0004" }, {"IL0017" }, {"IN0031" }, {"KS0067" }, {"KY0006" },
                { "LA0002" }, {"MA0011" }, {"MD0003" }, {"ME0006" }, {"MI0005" },
                { "MN0015" }, {"MO0033" }, {"MS0006" }, {"MT0002" }, {"NC0002" },
                { "ND0002" }, {"NE0018" }, {"NH0010" }, {"NJ0015" }, {"NM0015" },
                { "NV0002" }, {"NY0002" }, {"OH0023" }, {"OK0022" }, {"OR0002" },
                { "PA0064" }, {"PR0002" }, {"RI0004" }, {"SC0002" }, {"SD0021" },
                { "TN0004" }, {"TQ6401" }, {"TX0001" }, {"UT0010" }, {"VA0001" },
                { "VI0007" }, {"VT0002" }, {"WA0019" }, {"WI0016" }, {"WV0003" },
                { "WY0003"}
            };
        #endregion
    }
}
