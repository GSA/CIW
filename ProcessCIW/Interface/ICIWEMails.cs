using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessCIW.Interface
{
    public interface ICiwEmails
    {
        void Setup(int uID, string firstName, string middleName, string lastName, string suffix, string fileName, bool isChildCareWorker = false);
        void SendWrongVersion();
        void SendPasswordProtection();
        void SendDuplicateUser();
        void SendARRA();
        void SendErrors(ValidationResult s1, ValidationResult s2, ValidationResult s3, ValidationResult s4, ValidationResult s5, ValidationResult s6);
        void SendSponsorshipEMail(int id);
    }
}
