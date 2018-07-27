using FluentValidation.Results;
using ProcessCIW.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessCIW.Interface
{
    public interface IValidateCIW
    {
        bool IsDuplicate(List<CIW> ciwInformation);
        bool IsFormValid(List<CIW> ciwInformation);
        void PrintErrors();
        Tuple<ValidationResult, ValidationResult, ValidationResult, ValidationResult, ValidationResult, ValidationResult> GetErrors();

    }
}
