using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessCIW.Interface
{
    public interface IUtilities
    {
        bool IsNotWhiteSpace(string s);
        bool StartBeforeEnd(string StartDate, string EndDate);
        bool EndIsFutureDate(string End);
        bool BeAValidEndDate(string End, int NumOfYearsInTheFuture);
        bool DateIsValidAndNotFuture(string Date);
        bool BeAValidDate(string date);
        bool BeAValidBirthDate(string date);
        string FormatDate(string birthDate);
        string TrimPhoneNum(string s);
        string TrimPoundSign(string s);
        string CleanSsn(string s);
        byte[] HashSSN(string ssn);
        string GenerateDecryptedFilename(string encryptedFilename);
        void DeleteFiles(List<string> filesToDelete);

    }
}
