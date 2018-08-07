using FluentValidation;
using ProcessCIW.Interface;
using ProcessCIW.Mapping;
using ProcessCIW.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessCIW.Validation
{
    /// <summary>
    /// Section 4 Validation
    /// </summary>
    class ProjectLocationValidator : AbstractValidator<CIW>
    {

        /// <summary>
        /// Contains all the validation rules for section 4
        /// </summary>
        public ProjectLocationValidator(IDataAccess da)
        {
            //Building ID is only required when other is not home, vendor or non-gsa
            When(b => (b.Other.Equals("")), () =>
            {
                RuleFor(building => building.BuildingNumber)
                    .NotEmpty()
                    .WithMessage("GSA Building Number: Required when Other is not 'HOME', 'VENDOR', or 'NONGSA'")
                    .Must(da.BeAValidBuilding)
                    .WithMessage("GSA Building Number: Not Found in GCIMS");
            });

            When(b => (b.Other.Equals("HOME") || b.Other.Equals("VENDOR") || b.Other.Equals("NONGSA")), () =>
            {
                RuleFor(building => building.BuildingNumber)
                    .Empty()
                    .WithMessage("Project/Work Location: You have entered two work locations, a GSA Building Number and a Non-GSA Work Location, please make only one selection");
            });

            //Other is only required when Building ID is equal to ""
            When(b => b.BuildingNumber.Equals(""), () =>
            {
                //Other
                RuleFor(building => building.Other)
                    .NotEmpty()
                    .WithMessage("Other: Required field when GSA Building Number is not entered");
            });

            //Type Contractor
            RuleFor(building => building.ContractorType)
                    .NotEmpty()
                    .WithMessage("Type Contractor: Required Field");

            //Ensure type contractor is child care when investigation type requested is tier 1C
            When(b => b.InvestigationTypeRequested == "Tier 1C", () =>
            {
                RuleFor(building => building.ContractorType)
                    .Equal("Child Care")
                    .WithMessage("Type Contractor: Applicants with Investigation Type Requested Child Care Worker must have Type Contractor Child Care");
            });

            //ARRA Long Term
            RuleFor(building => building.ArraLongTermContractor)
                    .NotEqual(CIWWordConstants.CHOOSE_AN_ITEM)
                    .WithMessage("ARRA Long Term Contractor: Required Field");

            //Sponsoring Major Org
            RuleFor(building => building.SponsoringMajorOrg)
                    .NotEmpty()
                    .WithMessage("Sponsoring Major Org: Required Field");

            //Office Symbol
            RuleFor(building => building.SponsoringOfficeSymbol)
                    .NotEmpty()
                    .WithMessage("Sponsoring Office Symbol: Required Field")
                    .Matches(@"^[a-zA-Z0-9_.-]*$")
                    .WithMessage("Sponsoring Office Symbol: Contains Invalid Characters");

            //Region
            RuleFor(building => building.Region)
                    .NotEmpty()
                    .WithMessage("GSA Region: Required Field");
        }
    }
}
