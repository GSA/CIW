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
    /// Section 2 Validation
    /// </summary>
    class ContractorValidator : AbstractValidator<CIW>
    {
        private readonly IUtilities U = new Utilities.Utilities();
        private const int YearsInTheFuture = 30;
        /// <summary>
        /// Contains all the validation rules for section 2
        /// </summary>
        public ContractorValidator()
        {
            RuleSet("ValidFirstAndSecondRow", () =>
            {
                //Company Name
                RuleFor(employee => employee.CompanyName)
                    .NotEmpty()
                    .WithMessage("Company Name (Primary): Required Field");

                When(e => !(e.DataUniversalNumberingSystem.Equals("")), () =>
                {
                    RuleFor(employee => employee.DataUniversalNumberingSystem)
                        .Matches(@"^[0-9]{9,9}$")
                        .WithMessage("DUNS Number: Invalid - should be only numeric and exactly 9 characters long");
                });

                // TO/DO
                RuleFor(employee => employee.TaskOrderDeliveryOrder)
                    .NotEmpty()
                    .WithMessage("Task Order (TO)/ Delivery Order (DO) Number/ Contract Base Number: Required Field");

                //Contract Number Type
                RuleFor(employee => employee.ContractNumberType)
                    .NotEmpty()
                    .WithMessage("Contract Number Type: Required Field");

                //changes for contract dates to be used with child care changes
                //This will be when neither is child care
                When((e => e.ContractorType != "Child Care" && e.InvestigationTypeRequested != "Tier 1C"), () =>
                {
                    //Contract start date
                    RuleFor(employee => employee.ContractStartDate)
                        .NotEqual("")
                        .WithMessage("Contract Start Date: Required Field")
                        .Must(U.BeAValidDate)
                        .WithMessage("Contract Start Date: Invalid Date")
                        .Must((c, ContractStartDate) => U.StartBeforeEnd(c.ContractStartDate, c.ContractEndDate) && U.EndIsFutureDate(c.ContractEndDate))
                        .WithMessage("Contract Start Date: Cannot be later than Contract End Date");

                    //Contract End Date
                    RuleFor(employee => employee.ContractEndDate)
                        .NotEqual("")
                        .WithMessage("Contract End Date: Required Field")
                        .Must((c, x) => U.BeAValidEndDate(c.ContractEndDate, YearsInTheFuture))
                        .WithMessage("Contract End Date: Invalid Date: date must follow mm/dd/yyyy format and be no more than 30 years in the future")
                        .Must(U.EndIsFutureDate)
                        .WithMessage("Contract End Date: Must be a future date");
                });

                //This will be when one or the other is child care
                Unless((e => e.ContractorType != "Child Care" && e.InvestigationTypeRequested != "Tier 1C"), () =>
                {
                    Unless((e => e.ContractorType == "Child Care" && e.InvestigationTypeRequested == "Tier 1C"), () =>
                    {
                        //Contract start date
                        RuleFor(employee => employee.ContractStartDate)
                            .NotEqual("")
                            .WithMessage("Contract Start Date: Required Field (unless Child Care)")
                            .Must(U.BeAValidDate)
                            .WithMessage("Contract Start Date: Invalid Date")
                            .Must((c, ContractStartDate) => U.StartBeforeEnd(c.ContractStartDate, c.ContractEndDate) && U.EndIsFutureDate(c.ContractEndDate))
                            .WithMessage("Contract Start Date: Cannot be later than Contract End Date");

                        //Contract End Date
                        RuleFor(employee => employee.ContractEndDate)
                            .NotEqual("")
                            .WithMessage("Contract End Date: Required Field (unless Child Care)")
                            .Must((c, x) => U.BeAValidEndDate(c.ContractEndDate, YearsInTheFuture))
                            .WithMessage("Contract End Date: Invalid Date: date must follow mm/dd/yyyy format and be no more than 30 years in the future")
                            .Must(U.EndIsFutureDate)
                            .WithMessage("Contract End Date: Must be a future date");
                    });
                });

                //Has Option Years
                RuleFor(employee => employee.HasOptionYears)
                        .NotEqual("")
                        .WithMessage("Has Option Years: Required Field");

                When(e => e.HasOptionYears.Equals("Yes"), () =>
                {
                    RuleFor(employee => employee.NumberOfOptionYears)
                        .NotEqual("")
                        .WithMessage("Has Option Years: You have indicated the contract Has Option Years, please select a value for # Of Options Years");
                });

                When(e => e.HasOptionYears.Equals("No"), () =>
                {
                    RuleFor(employee => employee.NumberOfOptionYears)
                        .Equal("")
                        .WithMessage("Has Option Years: This field value contradicts the input for # Of Options Years");
                });
            });

            //Row 3 is required
            RuleSet("ValidPOCRow1", () =>
            {
                RuleFor(contractInformation => contractInformation.ContractPOCFirstName)
                        .Length(0, 45)
                        .WithMessage("Primary Company Point of Contact(POC) First Name: exceeds maximum number of characters. Please double-check the field. If value is correct, please reach out to HSPD-12 Security at HSPD12.Security@gsa.gov or at +1 (202) 501-4459.")
                        .NotEmpty()
                        .WithMessage("Primary Company Point of Contact(POC) First Name: Required Field")
                        .Matches(@"^[a-zA-Z \-\‘\’\'\`]{1,45}$")
                        .WithMessage("Primary Company Point of Contact(POC) First Name: Contains Invalid Characters");

                RuleFor(contractInformation => contractInformation.ContractPOCLastName)
                        .Length(0, 45)
                        .WithMessage("Primary Company Point of Contact(POC) Last Name: exceeds maximum number of characters. Please double-check the field. If value is correct, please reach out to HSPD-12 Security at HSPD12.Security@gsa.gov or at +1 (202) 501-4459.")
                        .NotEmpty()
                        .WithMessage("Primary Company Point of Contact(POC) Last Name: Required Field")
                        .Matches(@"^[a-zA-Z \-\‘\’\'\`]{1,45}$")
                        .WithMessage("Primary Company Point of Contact(POC) Last Name: Contains Invalid Characters");

                RuleFor(contractInformation => contractInformation.ContractPOCPhoneWork)
                        .NotEmpty()
                        .WithMessage("Primary Company Point of Contact(POC) Work Phone Number: Required Field")
                        .Matches(@"^\(?([0-9]{3})\)?[-.●]?([0-9]{3})[-.●]?([0-9]{4})$")
                        .WithMessage("Primary Company Point of Contact(POC) Work Phone Number: Invalid Phone Number");

                RuleFor(contractInformation => contractInformation.ContractPOCEMailAddress)
                        .NotEmpty()
                        .WithMessage("Primary Company Point of Contact(POC) E-Mail Address: Required Field")
                        .EmailAddress()
                        .WithMessage("Primary Company Point of Contact(POC) E-Mail Address: Invalid E-Mail Address");
            });

            RuleSet("ValidPOCRow2", () =>
            {
                //Row 2
                When(r => (r.ContractPOCAlternatePocFirstname1 != "" || r.ContractPOCAlternatePocLastname1 != "" ||
                           r.ContractPOCAlternatePocPhoneWork1 != "" || r.ContractPOCAlternatePocEmail1 != ""), () =>
                           {
                               RuleFor(r => r.ContractPOCAlternatePocFirstname1)
                                    .Length(0, 45)
                                    .WithMessage("Alternate Company Point of Contact(POC) 1: First Name: exceeds maximum number of characters. Please double-check the field. If value is correct, please reach out to HSPD-12 Security at HSPD12.Security@gsa.gov or at +1 (202) 501-4459.")
                                    .NotEmpty()
                                    .WithMessage("Alternate Company Point of Contact(POC) 1: First Name: Required Field")
                                    .Matches(@"^[a-zA-Z \-\‘\’\'\`]{1,45}$")
                                    .WithMessage("Alternate Company Point of Contact(POC) 1: First Name: Contains Invalid Characters");

                               RuleFor(r => r.ContractPOCAlternatePocLastname1)
                                    .Length(0, 45)
                                    .WithMessage("Alternate Company Point of Contact(POC) 1: Last Name: exceeds maximum number of characters. Please double-check the field. If value is correct, please reach out to HSPD-12 Security at HSPD12.Security@gsa.gov or at +1 (202) 501-4459.")
                                    .NotEmpty()
                                    .WithMessage("Alternate Company Point of Contact(POC) 1: Last Name: Required Field")
                                    .Matches(@"^[a-zA-Z \-\‘\’\'\`]{1,45}$")
                                    .WithMessage("Alternate Company Point of Contact(POC) 1: Last Name: Contains Invalid Characters");

                               RuleFor(r => r.ContractPOCAlternatePocPhoneWork1)
                                       .NotEmpty()
                                       .WithMessage("Alternate Company Point of Contact(POC) 1: Phone Number:Required Field")
                                       .Matches(@"^\(?([0-9]{3})\)?[-.●]?([0-9]{3})[-.●]?([0-9]{4})$")
                                       .WithMessage("Alternate Company Point of Contact(POC) 1: Phone Number:Invalid Phone Number");

                               RuleFor(r => r.ContractPOCAlternatePocEmail1)
                                       .NotEmpty()
                                       .WithMessage("Alternate Company Point of Contact(POC) 1: E-Mail Address: Required Field")
                                       .EmailAddress()
                                       .WithMessage("Alternate Company Point of Contact(POC) 1: E-Mail Address: Invalid E-Mail Address");
                           });
            });

            RuleSet("ValidPOCRow3", () =>
            {
                //Row 3
                When(r => (r.ContractPOCAlternatePocFirstname2 != "" || r.ContractPOCAlternatePocLastname2 != "" ||
                           r.ContractPOCAlternatePocPhoneWork2 != "" || r.ContractPOCAlternatePocEmail2 != ""), () =>
                           {
                               RuleFor(r => r.ContractPOCAlternatePocFirstname2)
                                    .Length(0, 45)
                                    .WithMessage("Alternate Company Point of Contact(POC) 2: First Name: exceeds maximum number of characters. Please double-check the field. If value is correct, please reach out to HSPD-12 Security at HSPD12.Security@gsa.gov or at +1 (202) 501-4459.")
                                    .NotEmpty()
                                    .WithMessage("Alternate Company Point of Contact(POC) 2: First Name: Required Field")
                                    .Matches(@"^[a-zA-Z \-\‘\’\'\`]{1,45}$")
                                    .WithMessage("Alternate Company Point of Contact(POC) 2: First Name: Contains Invalid Characters");

                               RuleFor(r => r.ContractPOCAlternatePocLastname2)
                                    .Length(0, 45)
                                    .WithMessage("Alternate Company Point of Contact(POC) 2: Last Name: exceeds maximum number of characters. Please double-check the field. If value is correct, please reach out to HSPD-12 Security at HSPD12.Security@gsa.gov or at +1 (202) 501-4459.")
                                    .NotEmpty()
                                    .WithMessage("Alternate Company Point of Contact(POC) 2: Last Name: Required Field")
                                    .Matches(@"^[a-zA-Z \-\‘\’\'\`]{1,45}$")
                                    .WithMessage("Alternate Company Point of Contact(POC) 2: Last Name: Contains Invalid Characters");

                               RuleFor(r => r.ContractPOCAlternatePocPhoneWork2)
                                       .NotEmpty()
                                       .WithMessage("Alternate Company Point of Contact(POC) 2: Work Phone: Required Field")
                                       .Matches(@"^\(?([0-9]{3})\)?[-.●]?([0-9]{3})[-.●]?([0-9]{4})$")
                                       .WithMessage("Alternate Company Point of Contact(POC) 2: Work Phone: Invalid Phone Number");

                               RuleFor(r => r.ContractPOCAlternatePocEmail2)
                                       .NotEmpty()
                                       .WithMessage("Alternate Company Point of Contact(POC) 2: E-Mail Address: Required Field")
                                       .EmailAddress()
                                       .WithMessage("Alternate Company Point of Contact(POC) 2: E-Mail Address: Invalid E-Mail Address");

                           });
            });

            RuleSet("ValidPOCRow4", () =>
            {
                //Row 4
                When(r => (r.ContractPOCAlternatePocFirstname3 != "" || r.ContractPOCAlternatePocLastname3 != "" ||
                           r.ContractPOCAlternatePocPhoneWork3 != "" || r.ContractPOCAlternatePocEmail3 != ""), () =>
                           {
                               RuleFor(r => r.ContractPOCAlternatePocFirstname3)
                                    .Length(0, 45)
                                    .WithMessage("Alternate Company Point of Contact(POC) 3: First Name: exceeds maximum number of characters. Please double-check the field. If value is correct, please reach out to HSPD-12 Security at HSPD12.Security@gsa.gov or at +1 (202) 501-4459.")
                                    .NotEmpty()
                                    .WithMessage("Alternate Company Point of Contact(POC) 3: First Name: Required Field")
                                    .Matches(@"^[a-zA-Z \-\‘\’\'\`]{1,45}$")
                                    .WithMessage("Alternate Company Point of Contact(POC) 3: First Name: Contains Invalid Characters");

                               RuleFor(r => r.ContractPOCAlternatePocLastname3)
                                    .Length(0, 45)
                                    .WithMessage("Alternate Company Point of Contact(POC) 3: Last Name: exceeds maximum number of characters. Please double-check the field. If value is correct, please reach out to HSPD-12 Security at HSPD12.Security@gsa.gov or at +1 (202) 501-4459.")
                                    .NotEmpty()
                                    .WithMessage("Alternate Company Point of Contact(POC) 3: Last Name: Required Field")
                                    .Matches(@"^[a-zA-Z \-\‘\’\'\`]{1,45}$")
                                    .WithMessage("Alternate Company Point of Contact(POC) 3: Last Name: Contains Invalid Characters");

                               RuleFor(r => r.ContractPOCAlternatePocPhoneWork3)
                                       .NotEmpty()
                                       .WithMessage("Alternate Company Point of Contact(POC) 3: Work Phone: Required Field")
                                       .Matches(@"^\(?([0-9]{3})\)?[-.●]?([0-9]{3})[-.●]?([0-9]{4})$")
                                       .WithMessage("Alternate Company Point of Contact(POC) 3: Work Phone: Invalid Phone Number");

                               RuleFor(r => r.ContractPOCAlternatePocEmail3)
                                       .NotEmpty()
                                       .WithMessage("Alternate Company Point of Contact(POC) 3: E-Mail Address: Required Field")
                                       .EmailAddress()
                                       .WithMessage("Alternate Company Point of Contact(POC) 3: E-Mail Address: Invalid E-Mail Address");
                           });
            });


            RuleSet("ValidPOCRow5", () =>
            {
                //Row 5
                When(r => (r.ContractPOCAlternatePocFirstname4 != "" || r.ContractPOCAlternatePocLastname4 != "" ||
                           r.ContractPOCAlternatePocPhoneWork4 != "" || r.ContractPOCAlternatePocEmail4 != ""), () =>
                           {
                               RuleFor(r => r.ContractPOCAlternatePocFirstname4)
                                    .Length(0, 45)
                                    .WithMessage("Alternate Company Point of Contact(POC) 4: First Name: exceeds maximum number of characters. Please double-check the field. If value is correct, please reach out to HSPD-12 Security at HSPD12.Security@gsa.gov or at +1 (202) 501-4459.")
                                    .NotEmpty()
                                    .WithMessage("Alternate Company Point of Contact(POC) 4: First Name: Required Field")
                                    .Matches(@"^[a-zA-Z \-\‘\’\'\`]{1,45}$")
                                    .WithMessage("Alternate Company Point of Contact(POC) 4: First Name: Contains Invalid Characters");

                               RuleFor(r => r.ContractPOCAlternatePocLastname4)
                                    .Length(0, 45)
                                    .WithMessage("Alternate Company Point of Contact(POC) 4: Last Name: exceeds maximum number of characters. Please double-check the field. If value is correct, please reach out to HSPD-12 Security at HSPD12.Security@gsa.gov or at +1 (202) 501-4459.")
                                    .NotEmpty()
                                    .WithMessage("Alternate Company Point of Contact(POC) 4: Last Name: Required Field")
                                    .Matches(@"^[a-zA-Z \-\‘\’\'\`]{1,45}$")
                                    .WithMessage("Alternate Company Point of Contact(POC) 4: Last Name: Contains Invalid Characters");

                               RuleFor(r => r.ContractPOCAlternatePocPhoneWork4)
                                       .NotEmpty()
                                       .WithMessage("Alternate Company Point of Contact(POC) 4: Work Phone: Required Field")
                                       .Matches(@"^\(?([0-9]{3})\)?[-.●]?([0-9]{3})[-.●]?([0-9]{4})$")
                                       .WithMessage("Alternate Company Point of Contact(POC) 4: Work Phone: Invalid Phone Number");

                               RuleFor(r => r.ContractPOCAlternatePocEmail4)
                                       .NotEmpty()
                                       .WithMessage("Alternate Company Point of Contact(POC) 4: E-Mail Address: Required Field")
                                       .EmailAddress()
                                       .WithMessage("Alternate Company Point of Contact(POC) 4: E-Mail Address: Invalid E-Mail Address");
                           });
            });
        }
    }
}
