using FluentValidation;
using ProcessCIW.Interface;
using ProcessCIW.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessCIW.Validation
{
    /// <summary>
    /// Section 6 Validation
    /// </summary>
    class RequestingOfficialValidator : AbstractValidator<CIW>
    {

        /// <summary>
        /// Contains all the validation rules for section 6
        /// </summary>
        public RequestingOfficialValidator(IDataAccess da)
        {
            //Row 1 check for email and PMCORCO
            RuleSet("ValidGSARow1", () =>
            {
                RuleFor(requestingOfficial => requestingOfficial.SponsorEmailAddress)
                        .Cascade(FluentValidation.CascadeMode.StopOnFirstFailure)
                        .NotEmpty()
                        .WithMessage("Primary GSA Requesting Official E-Mail Address: Required Field")
                        .Matches(@"^[a-zA-Z0-9_.+-]+@(?:(?:[a-zA-Z0-9-]+\.)?[a-zA-Z]+\.)?(gsa)(ig)?\.gov$")
                        .WithMessage("Primary GSA Requesting Official E-Mail Address: Not Valid GSA E-Mail")
                        .Must(da.BeAValidEMail)
                        .WithMessage("Primary GSA Requesting Official E-Mail Address: Not Found in GCIMS");

                RuleFor(requestingOfficial => requestingOfficial.SponsorIsPMCORCO)
                        .NotEmpty()
                        .WithMessage("Primary GSA Requesting Official Is PM/COR/CO/CS: Required Field");
            });

            RuleSet("ValidGSARow2", () =>
            {
                //GSA Row 2
                When(r => (r.SponsorAlternateEmailAddress1 != "" || r.SponsorAlternateIsPMCORCO1 != ""), () =>
                {
                    RuleFor(r => r.SponsorAlternateEmailAddress1)
                            .Matches(@"^[a-zA-Z0-9_.+-]+@(?:(?:[a-zA-Z0-9-]+\.)?[a-zA-Z]+\.)?(gsa)(ig)?\.gov$")
                            .WithMessage("Alternate GSA Requesting Official 1: E-Mail Address: Not Valid GSA E-Mail")
                            .Must(da.BeAValidEMail)
                            .WithMessage("Alternate GSA Requesting Official 1: E-Mail Address: Not Found in GCIMS");

                    RuleFor(requestingOfficial => requestingOfficial.SponsorAlternateIsPMCORCO1)
                            .NotEmpty()
                            .WithMessage("Alternate GSA Requesting Official 1: Is PM/COR/CO: Required Field");
                });
            });

            RuleSet("ValidGSARow3", () =>
            {
                //GSA Row 3
                When(r => (r.SponsorAlternateEmailAddress2 != "" || r.SponsorAlternateIsPMCORCO2 != ""), () =>
                {
                    RuleFor(r => r.SponsorAlternateEmailAddress2)
                            .Matches(@"^[a-zA-Z0-9_.+-]+@(?:(?:[a-zA-Z0-9-]+\.)?[a-zA-Z]+\.)?(gsa)(ig)?\.gov$")
                            .WithMessage("Alternate GSA Requesting Official 2: E-Mail Address: Not Valid GSA E-Mail")
                            .Must(da.BeAValidEMail)
                            .WithMessage("Alternate GSA Requesting Official 2: E-Mail Address: Not Found in GCIMS");

                    RuleFor(requestingOfficial => requestingOfficial.SponsorAlternateIsPMCORCO2)
                            .NotEmpty()
                            .WithMessage("Alternate GSA Requesting Official 2: Is PM/COR/CO: Required Field");
                });
            });

            RuleSet("ValidGSARow4", () =>
            {
                //GSA Row 4
                When(r => (r.SponsorAlternateEmailAddress3 != "" || r.SponsorAlternateIsPMCORCO3 != ""), () =>
                {
                    RuleFor(r => r.SponsorAlternateEmailAddress3)
                            .Matches(@"^[a-zA-Z0-9_.+-]+@(?:(?:[a-zA-Z0-9-]+\.)?[a-zA-Z]+\.)?(gsa)(ig)?\.gov$")
                            .WithMessage("Alternate GSA Requesting Official 3: E-Mail Address: Not Valid GSA E-Mail")
                            .Must(da.BeAValidEMail)
                            .WithMessage("Alternate GSA Requesting Official 3: E-Mail Address: Not Found in GCIMS");

                    RuleFor(requestingOfficial => requestingOfficial.SponsorAlternateIsPMCORCO3)
                            .NotEmpty()
                            .WithMessage("Alternate GSA Requesting Official 3: Is PM/COR/CO: Required Field");
                });
            });

            RuleSet("ValidGSARow5", () =>
            {
                //GSA Row 5
                When(r => (r.SponsorAlternateEmailAddress4 != "" || r.SponsorAlternateIsPMCORCO4 != ""), () =>
                {
                    RuleFor(r => r.SponsorAlternateEmailAddress4)
                            .Matches(@"^[a-zA-Z0-9_.+-]+@(?:(?:[a-zA-Z0-9-]+\.)?[a-zA-Z]+\.)?(gsa)(ig)?\.gov$")
                            .WithMessage("Alternate GSA Requesting Official 4: E-Mail Address: Not Valid GSA E-Mail")
                            .Must(da.BeAValidEMail)
                            .WithMessage("Alternate GSA Requesting Official 4: E-Mail Address: Not Found in GCIMS");

                    RuleFor(requestingOfficial => requestingOfficial.SponsorAlternateIsPMCORCO4)
                            .NotEmpty()
                            .WithMessage("Alternate GSA Requesting Official 4: Is PM/COR/CO: Required Field");
                });
            });
        }
    }
}
