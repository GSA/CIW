using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ProcessCIW.Test.dll
{
    public partial class CiwTest
    {
        #region Utility Tests


        [Theory]
        [MemberData(nameof(BeAValidBirthDate_True))]
        public void BeAValidBirthDate_True_Test(string date)
        {
            var result = util.BeAValidBirthDate(date);
            Assert.True(result);
        }

        [Theory]
        [MemberData(nameof(BeAValidBirthDate_False))]
        public void BeAValidBirthDate_False_Test(string date)
        {
            var result = util.BeAValidBirthDate(date);
            Assert.False(result);
        }

        [Theory]
        [MemberData(nameof(BeAValidDate_True))]
        public void BeAValidDate_True_Test(string date)
        {
            var result = util.BeAValidDate(date);
            Assert.True(result);
        }

        [Theory]
        [MemberData(nameof(BeAValidDate_False))]
        public void BeAValidDate_False_Test(string date)
        {
            var result = util.BeAValidDate(date);
            Assert.False(result);
        }

        [Theory]
        [MemberData(nameof(BeAValidEndDate_True))]
        public void BeAValidEndDate_true_Test(string date, int end)
        {
            var result = util.BeAValidEndDate(date, end);
            Assert.True(result);
        }

        [Theory]
        [MemberData(nameof(BeAValidEndDate_False))]
        public void BeAValidEndDate_False_Test(string date, int end)
        {
            var result = util.BeAValidEndDate(date, end);
            Assert.False(result);
        }

        [Theory]
        [MemberData(nameof(CleanSsn_True))]
        public void CleanSsn_True_Test(string s, string e)
        {
            var expected = e;
            var actual = util.CleanSsn(s);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(CleanSsn_False))]
        public void CleanSsn_False_Test(string s, string e)
        {
            var expected = e;
            var actual = util.CleanSsn(s);
            Assert.NotEqual(expected, actual);
        }

        [Theory]
        [MemberData(nameof(DateIsValidAndNotFuture_True))]
        public void DateIsValidAndNotFuture_True_Test(string date)
        {
            var result = util.DateIsValidAndNotFuture(date);
            Assert.True(result);
        }

        [Theory]
        [MemberData(nameof(DateIsValidAndNotFuture_False))]
        public void DateIsValidAndNotFuture_False_Test(string date)
        {
            var result = util.DateIsValidAndNotFuture(date);
            Assert.False(result);
        }       

        [Theory]
        [MemberData(nameof(EndIsFutureDate_True))]
        public void EndIsFutureDate_True_Test(string s)
        {
            var result = util.EndIsFutureDate(s);
            Assert.True(result);
        }

        [Theory]
        [MemberData(nameof(EndIsFutureDate_False))]
        public void EndIsFutureDate_False_Test(string s)
        {
            var result = util.EndIsFutureDate(s);
            Assert.False(result);
        }

        [Theory]
        [MemberData(nameof(FormatDate_True))]
        public void FormatDate_True_Test(string actual, string expected)
        {
            var result = util.FormatDate(actual);
            Assert.Equal(expected, result);
        }

        [Theory]
        [MemberData(nameof(FormatDate_False))]
        public void FormatDate_False_Test(string actual, string expected)
        {
            var result = util.FormatDate(actual);
            Assert.NotEqual(expected, result);
        }

        [Theory]
        [MemberData(nameof(GenerateDecryptedFilename_True))]
        public void GenerateDecryptedFilename_True_Test(string actual, string expected)
        {
            var result = util.GenerateDecryptedFilename(actual);
            Assert.Equal(expected, result);
        }

        [Theory]
        [MemberData(nameof(GenerateDecryptedFilename_False))]
        public void GenerateDecryptedFilename_False_Test(string actual, string expected)
        {
            var result = util.GenerateDecryptedFilename(actual);
            Assert.NotEqual(expected, result);
        }

        [Theory]
        [MemberData(nameof(IsNotWhiteSpace_True))]
        public void IsNotWhiteSpace_True_Test(string s)
        {
            var result = util.IsNotWhiteSpace(s);
            Assert.True(result);
        }

        [Theory]
        [MemberData(nameof(IsNotWhiteSpace_False))]
        public void IsNotWhiteSpace_False_Test(string s)
        {
            var result = util.IsNotWhiteSpace(s);
            Assert.False(result);
        }

        [Theory]
        [MemberData(nameof(StartBeforeEnd_True))]
        public void StartBeforeEnd_True_Test(string start, string end)
        {
            var result = util.StartBeforeEnd(start, end);
            Assert.True(result);
        }

        [Theory]
        [MemberData(nameof(StartBeforeEnd_False))]
        public void StartBeforeEnd_False_Test(string start, string end)
        {
            var result = util.StartBeforeEnd(start, end);
            Assert.False(result);
        }

        [Theory]
        [MemberData(nameof(TrimPhoneNum_True))]
        public void TrimPhoneNum_True_Test(string s1, string expected)
        {
            var actual = util.TrimPhoneNum(s1);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(TrimPhoneNum_False))]
        public void TrimPhoneNum_False_Test(string s1, string expected)
        {
            var actual = util.TrimPhoneNum(s1);
            Assert.NotEqual(expected, actual);
        }

        [Theory]
        [MemberData(nameof(TrimPoundSign_True))]
        public void TrimPoundSign_True_Test(string s1, string expected)
        {
            var actual = util.TrimPoundSign(s1);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(TrimPoundSign_False))]
        public void TrimPoundSign_False_Test(string s1, string expected)
        {
            var actual = util.TrimPoundSign(s1);
            Assert.NotEqual(expected, actual);
        }

        public static TheoryData<string, string> TrimPoundSign_False =>
            new TheoryData<string, string>
            {
                { "#", "#" },
                { "1#1", "#" },
                { "1# #1", "11" },
                { "", "#" },
                { "##", "#" }
            };

        public static TheoryData<string, string> TrimPoundSign_True =>
            new TheoryData<string, string>
            {
                { "#", "" },
                { "##", "" },
                { "#1#", "1" },
                { "123!@$%", "123!@$%" },
                { "\t#\t#\t", "\t\t\t" }
            };

        public static TheoryData<string, string> TrimPhoneNum_False =>
            new TheoryData<string, string>
            {
                { "1!2@3#4$5%6^7&8*9(", "!@#$%^&*" },
                { "abcdefghijklmn0", "" },
                { "123-456-7890", "123-456-7890" },
                { "(123)456-7890", "()-" },
                { "a,./;'[]-=+_}{\":?><!@#$%^&*()", " " },
                { "   ", "   "}
            };

        public static TheoryData<string, string> TrimPhoneNum_True =>
            new TheoryData<string, string>
            {
                { "1!2@3#4$5%6^7&8*9(", "123456789" },
                { "abcdefghijklmn0", "0" },
                { "123-456-7890", "1234567890" },
                { "(123)456-7890", "1234567890" },
                { "a,./;'[]-=+_}{\":?><!@#$%^&*()", "" },
                { "123 456 7890", "1234567890"}
            };

        public static TheoryData<string, string> StartBeforeEnd_False =>
           new TheoryData<string, string>
           {
                { "2019-01-01", "2018-01-01" },
                { "2018-01-02", "2018-01-01" },
                { DateTime.MaxValue.ToShortDateString(), "2018-12-25" },
                { DateTime.MaxValue.ToShortDateString(), DateTime.MinValue.ToShortDateString() },
                { "2018-01-01", DateTime.MinValue.ToShortDateString() },
                { "10-21-2015", "November 5 1955"}
           };

        public static TheoryData<string, string> StartBeforeEnd_True =>
            new TheoryData<string, string>
            {
                { "2018-01-01", "2019-01-01" },
                { "2018-01-01", "2018-01-02" },
                { "2018-12-25", DateTime.MaxValue.ToShortDateString() },
                { DateTime.MinValue.ToShortDateString(), DateTime.MaxValue.ToShortDateString() },
                { DateTime.MinValue.ToShortDateString(), "2018-01-01" },
                { "November 5 1955", "10-21-2015"}
            };

        public static TheoryData<string> IsNotWhiteSpace_False =>
            new TheoryData<string>
            {
                { " " }, { "  " }, { "   " }, { "\n" }, { "\t" }
            };

        public static TheoryData<string> IsNotWhiteSpace_True =>
            new TheoryData<string>
            {
                { "" }, { " 1 " }, { "abc" }, { "a a" }, { "z" }
            };

        public static TheoryData<string, string> GenerateDecryptedFilename_False =>
            new TheoryData<string, string>
            {
                { "string", "strings-d.docx" },
                { "", "-D.docx" },
                { "string1", "string1-d.DOCX" }
            };

        public static TheoryData<string, string> GenerateDecryptedFilename_True =>
            new TheoryData<string, string>
            {
                { "string", "string-d.docx" },
                { "", "-d.docx" }
            };

        public static TheoryData<string, string> FormatDate_False =>
            new TheoryData<string, string>
            {
                { "November 5, 1955","1955-11-5"},
                { "10-21-2o15", "2015-10-21"}
            };

        public static TheoryData<string, string> FormatDate_True =>
            new TheoryData<string, string>
            {
                { "November 5, 1955","1955-11-05"},
                { "10-21-2015", "2015-10-21"}
            };

        public static TheoryData<string> EndIsFutureDate_False =>
           new TheoryData<string>
           {
                { DateTime.Now.AddHours(1).ToShortDateString() },
                { DateTime.MinValue.ToShortDateString() }
           };

        public static TheoryData<string> EndIsFutureDate_True =>
            new TheoryData<string>
            {
                { DateTime.Now.AddDays(1).ToShortDateString() },
                { DateTime.MaxValue.ToShortDateString() }
            };
        
        public static TheoryData<string> DateIsValidAndNotFuture_False =>
           new TheoryData<string>
           {
                { DateTime.Now.AddDays(1).ToShortDateString() },
                { DateTime.Now.AddYears(1000).ToShortDateString() },
                { "1950-31-31" }
           };

        public static TheoryData<string> DateIsValidAndNotFuture_True =>
            new TheoryData<string>
            {
                { DateTime.Now.AddDays(-1).ToShortDateString() },
                { DateTime.Now.AddYears(-1000).ToShortDateString() }
            };

        public static TheoryData<string, string> CleanSsn_False =>
            new TheoryData<string, string>
            {
                { "111!11!1111", "111111111" },
                { "111@11@1111", "111111111" },
                { "111#11#1111", "111111111" },
                { "111$11$1111", "111111111" },
                { "111%11%1111", "111111111" },
                { "111^11^1111", "111111111" },
                { "111&11&1111", "111111111" },
                { "111*11*1111", "111111111" },
                { "111(11)1111", "111111111" },
                { "111_11_1111", "111111111" },
                { "111+11+1111", "111111111" },
                { "111=11=1111", "111111111" },
                { "111/11/1111", "111111111" },
                { "1\"1\'1,1<1>1.1?1/1", "111111111"},
                { "a111111111b", "111111111"},
                { "1[1]1{1}1\\1|1;1:1", "111111111" }
            };

        public static TheoryData<string, string> CleanSsn_True =>
            new TheoryData<string, string>
            {
                { "111-11-1111", "111111111" },
                { "111 11 1111", "111111111"},
                { " 111111111 ", "111111111"},
                { " 1 1 1 - 1 1 - 1 1 1 1 ", "111111111" }
            };

        public static TheoryData<string, int> BeAValidEndDate_False =>
           new TheoryData<string, int>
           {
                { DateTime.Now.AddYears(10).ToShortDateString(), 9 },
                { DateTime.Now.AddMonths(6).ToShortDateString(), 0}
           };

        public static TheoryData<string, int> BeAValidEndDate_True =>
            new TheoryData<string, int>
            {
                { DateTime.Now.AddYears(10).ToShortDateString(), 11 },
                { DateTime.Now.AddMonths(6).ToShortDateString(), 1}
            };

        public static TheoryData<string> BeAValidDate_False =>
           new TheoryData<string>
           {
                { "13-01-1980" }, { "13-1980-01" }, { "january 12th 1966" },
                { "1-1-19o1" }, { "1950-d-5" }, { "1951-12-5th" }
           };

        public static TheoryData<string> BeAValidDate_True =>
            new TheoryData<string>
            {
                { "1900-01-01" }, { "2020-12-25" }, { "1-1-1" }, { "2001/01/01" },
                { "2000-jan-1" }, { "2001-november-10" },
                { DateTime.MinValue.ToShortDateString() },
                { DateTime.MaxValue.ToShortDateString()}
            };

        public static TheoryData<string> BeAValidBirthDate_False =>
            new TheoryData<string>
            {
                { "19801-01-01" }, { "1981-011-01" }, { "1981-32-01" },
                { new DateTime(DateTime.Now.AddYears(-14).Year, 1, 1).ToShortDateString() },
                { new DateTime(1899,1,1).ToShortDateString() }

            };


        public static TheoryData<string> BeAValidBirthDate_True =>
            new TheoryData<string>
            {
                { "1980-01-01" },
                { new DateTime(DateTime.Now.AddYears(-15).Year, 1, 1).ToShortDateString() },
                { new DateTime(1900,1,1).ToShortDateString() }

            };

        #endregion
    }
}
