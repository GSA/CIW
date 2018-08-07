using ProcessCIW.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using ProcessCIW.Models;

namespace ProcessCIW.Validation
{
    /// <summary>
    /// Fluent Validation class to validate if user exists
    /// </summary>
    class UserExistsValidator : AbstractValidator<CIW>
    {
        /// <summary>
        /// Validates if user exists
        /// </summary>
        public UserExistsValidator(IDataAccess da)
        {
            RuleFor(employee => employee.LastName)
                    .Must((o, LastName) => da.NotBeADuplicateUser(o.LastName, o.DateOfBirth, o.SocialSecurityNumber))
                    .WithMessage("Duplicate User Found!");
        }
    }
}
