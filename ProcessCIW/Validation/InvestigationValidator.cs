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
    /// Section 5 Validation
    /// </summary>
    class InvestigationValidator : AbstractValidator<CIW>
    {
        /// <summary>
        /// Contains all the validation rules for section 5
        /// </summary>
        public InvestigationValidator()
        {
            RuleFor(employee => employee.InvestigationTypeRequested)
                    .NotEqual("")
                    .WithMessage("Investigation Type Request: Required Field");

            //Ensure investigation type requested is tier 1c when contractor type is child care
            When(b => b.ContractorType == "Child Care", () =>
            {
                RuleFor(building => building.InvestigationTypeRequested)
                    .Equal("Tier 1C")
                    .WithMessage("Investigation Type Request: Applicants with Type Contractor Child Care must have Investigation Type Requested Child Care Worker");
            });

            RuleFor(employee => employee.AccessCardRequired)
                    .NotEqual("")
                    .WithMessage("HSPD-12 Card Required: Required Field");
        }
    }
}
