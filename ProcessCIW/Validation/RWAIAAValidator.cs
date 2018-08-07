using FluentValidation;
using ProcessCIW.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessCIW.Validation
{
    /// <summary>
    /// Section 3 Validation
    /// </summary>
    class RwaIaaValidator : AbstractValidator<CIW>
    {

        /// <summary>
        /// Contains all the validation rules for section 3
        /// </summary>
        public RwaIaaValidator()
        {
            When(r => !r.RWAIAANumber.Equals(""), () =>
            {
                RuleFor(employee => employee.RWAIAANumber)
                        .Matches(@"^[a-zA-Z0-9\-\s]{3,20}$")
                        .WithMessage("RWA/IAA Number: Invalid - RWA/IAA Numbers are alphanumeric and must be between 3 and 20 characters in length");

                RuleFor(e => e.RWAIAAAgency)
                    .NotEmpty()
                    .WithMessage("RWA/IAA Agency: When providing an RWA/IAA Number, an RWA/IAA Agency must also be provided");
            });

            When(r => !r.RWAIAAAgency.Equals(""), () =>
            {
                RuleFor(employee => employee.RWAIAANumber)
                    .NotEmpty()
                    .WithMessage("RWA/IAA Agency: When this field is populated, an RWA/IAA Number must also be provided");

                RuleFor(employee => employee.RWAIAAAgency)
                        .Length(1, 45)
                        .WithMessage("RWA/IAA Agency: Invalid length")
                        .Matches(@"^[a-zA-Z ]+$")
                        .WithMessage("RWAA/IAA Agency: Contains Invalid Characters");
            });
        }
    }
}
