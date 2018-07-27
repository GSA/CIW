using FluentValidation;
using FluentValidation.Results;
using MySql.Data.MySqlClient;
using ProcessCIW.Interface;
using ProcessCIW.Mapping;
using ProcessCIW.Models;
using ProcessCIW.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace ProcessCIW.Validation
{
    /// <summary>
    /// Fluent Validation class to validate if user exists
    /// </summary>
    class UserExistsValidator : AbstractValidator<CIW>
    {
        private readonly IDataAccess da;
        
        /// <summary>
        /// Validates if user exists
        /// </summary>
        public UserExistsValidator()
        {
            da = DataAccess.GetInstance(new MySqlConnection(ConfigurationManager.ConnectionStrings["GCIMS"].ToString()));

            RuleFor(employee => employee.LastName)
                    .Must((o, LastName) => da.NotBeADuplicateUser(o.LastName, o.DateOfBirth, o.SocialSecurityNumber))
                    .WithMessage("Duplicate User Found!");
        }
    }

    /// <summary>
    /// Section 1 Validation
    /// </summary>
    class EmployeeValidator : AbstractValidator<CIW>
    {
        private readonly IDataAccess da = DataAccess.GetInstance(new MySqlConnection(ConfigurationManager.ConnectionStrings["GCIMS"].ToString()));
        private readonly IUtilities U = new Utilities.Utilities();

        /// <summary>
        /// Contains all the validation rules for Section 1 of the CIW
        /// </summary>
        public EmployeeValidator()
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;

            //Last Name
            RuleFor(employee => employee.LastName)
                    .Length(0, 60)
                    .WithMessage("Full Last Name(s)(Family): exceeds maximum number of characters. Please double-check the field. If value is correct, please reach out to HSPD-12 Security at HSPD12.Security@gsa.gov or at +1 (202) 501-4459.")
                    .NotEmpty()
                    .WithMessage("Full Last Name(s)(Family): Required Field")
                    .Matches(@"^[a-zA-Z \-\‘\’\'\`]+$")
                    .WithMessage("Full Last Name(s)(Family): Contains Invalid Characters");

            //First Name
            RuleFor(employee => employee.FirstName)
                    .Length(0, 60)
                    .WithMessage("Full First Name(Given): exceeds maximum number of characters. Please double-check the field. If value is correct, please reach out to HSPD-12 Security at HSPD12.Security@gsa.gov or at +1 (202) 501-4459.")
                    .NotEmpty()
                    .WithMessage("Full First Name(Given): Required Field")
                    .Matches(@"^[a-zA-Z \-\‘\’\'\`]+$")
                    .WithMessage("Full First Name(Given): Contains Invalid Characters");

            //Middle Name
            RuleFor(employee => employee.MiddleName)
                    .Length(0, 60)
                    .WithMessage("Full Middle Name(or NMN if none): exceeds maximum number of characters. Please double-check the field. If value is correct, please reach out to HSPD-12 Security at HSPD12.Security@gsa.gov or at +1 (202) 501-4459.")
                    .NotEmpty()
                    .WithMessage("Full Middle Name(or NMN if none): Required Field")
                    .Matches(@"^[A-Za-z \-\‘\’\'\`]{1,40}|[NMN]{1,3}$")
                    .WithMessage("Full Middle Name(or NMN if none): Contains Invalid Characters");
            
            //Gender
            RuleFor(employee => employee.Sex)
                    .NotEmpty()
                    .WithMessage("Gender: Required Field");

            //Social Security Number (need to check for more than one)
            RuleFor(employee => employee.SocialSecurityNumber)
                    .NotEmpty()
                    .WithMessage("Social Security Number: Required Field")
                    .Matches(@"^(?!000|666)[0-9]{3}([ -]?)(?!00)[0-9]{2}\1(?!0000)[0-9]{4}$")
                    .WithMessage("Social Security Number: Invalid Format")
                    .Must(da.NotBeADuplicateSSN)
                    .WithMessage("Social Security Number: Already in GCIMS");

            //Date Of Birth (Need method to make sure date is valid)
            RuleFor(employee => employee.DateOfBirth)
                    .NotEmpty()
                    .WithMessage("Date Of Birth: Required Field")
                    .Must(U.BeAValidBirthDate)
                    .WithMessage("Date Of Birth: Invalid Date");

            //POB:City
            RuleFor(employee => employee.PlaceOfBirthCity)
                    .NotEmpty()
                    .WithMessage("POB: City: Required Field")
                    .Matches(@"^[a-zA-Z-\. \'\‘\’]{1,75}$")
                    .WithMessage("POB: City: Contains Invalid Characters");

            //POB: Country
            RuleFor(employee => employee.PlaceOfBirthCountry)
                    .NotEmpty()
                    .WithMessage("POB: Country: Required Field");

            //POB: U.S. State
            When(employee => (employee.PlaceOfBirthCountry == "US"), () =>
            {
                RuleFor(employee => employee.PlaceOfBirthState)
                    .NotEmpty()
                    .WithMessage("POB: U.S. State: Required for the selected Country of Birth");
            });

            When(e => e.PlaceOfBirthCountry != "US", () =>
            {
                RuleFor(employee => employee.PlaceOfBirthState)
                    .Empty()
                    .WithMessage("POB: U.S. State: Leave blank when Country of Birth is not the United States");
            });

            //POB: Mexico or Canada State/Province
            When(employee => employee.PlaceOfBirthCountry == "MX" || employee.PlaceOfBirthCountry == "CA", () =>
            {
                RuleFor(employee => employee.PlaceOfBirthMexicoCanada)
                    .NotEmpty()
                    .WithMessage("POB: Mexico (State)/Canada (Province): Required for the selected Country of Birth")
                    .Must((e, x) => da.ValidateState(e.PlaceOfBirthMexicoCanada, e.PlaceOfBirthCountry))
                    .WithMessage("POB: Mexico (State)/Canada (Province): Selected Province/State does not match the selected Country of Birth");

            });

            When(employee => employee.PlaceOfBirthCountry != "MX" && employee.PlaceOfBirthCountry != "CA", () =>
            {
                RuleFor(employee => employee.PlaceOfBirthMexicoCanada)
                    .Empty()
                    .WithMessage("POB: Mexico (State)/Canada (Province): Leave blank when Country of Birth is not Canada or Mexico");
            });

            //Home Address One && Home Address Two
            When(h => !h.HomeAddressOne.Equals(""), () =>
            {
                RuleFor(employee => employee.HomeAddressOne)
                    .Matches(@"^[a-zA-Z0-9 .\\-\\\']+$")
                    .WithMessage("Home Address Street: Contains Invalid Characters");
            });

            When(h => !h.HomeAddressOne.Equals("") && !h.HomeAddressTwo.Equals(""), () =>
            {
                RuleFor(employee => employee.HomeAddressTwo)
                    .Matches(@"^[a-zA-Z0-9 .\\-\\\']+$")
                    .WithMessage("Home: Address Street (Line 2): Contains Invalid Characters");
            });

            When(h => h.HomeAddressOne.Equals(""), () =>
            {
                RuleFor(employee => employee.HomeAddressOne)
                   .NotEmpty()
                   .WithMessage("Home Address Street: Required Field");

                RuleFor(employee => employee.HomeAddressTwo)
                    .Empty()
                    .WithMessage("Home Address Street: Please fill out this field before filling out Address Street (line 2)");
            });

            //HomeAddressCity
            RuleFor(employee => employee.HomeAddressCity)
                    .NotEmpty()
                    .WithMessage("Home: City: Required Field")
                    .Matches(@"^[a-zA-Z-. \'\‘\’]{1,40}$")
                    .WithMessage("Home: City: Contains Invalid Characters");

            //HomeAddressCountry
            RuleFor(employee => employee.HomeAddressCountry)
                    .NotEmpty()
                    .WithMessage("Home: Country: Required Field")
                    .Equal("US")
                    .WithMessage("Home: Country: Only US residents are eligible for sponsorship at this time");

            //HomeAddressUSState
            When(employee => employee.HomeAddressCountry == "US", () =>
            {
                RuleFor(employee => employee.HomeAddressUSState)
                    .NotEmpty()
                    .WithMessage("Home: U.S. State: State of residence required for the selected Country of Residence");
            });

            When(employee => employee.HomeAddressCountry != "US", () =>
            {
                RuleFor(employee => employee.HomeAddressUSState)
                    .Empty()
                    .WithMessage("Home: U.S. State: State of residence should be left blank when Country of Residence is not the United States");
            });

            //HomeAddressMexicoCanada
            When(employee => employee.HomeAddressCountry == "CA" || employee.HomeAddressCountry == "MX", () =>
            {
                RuleFor(employee => employee.HomeAddressMexicoStateCanadaProvince)
                    .NotEmpty()
                    .WithMessage("Home: Mexico (State)/Canada (Province): Required Field")
                    .Must((e, x) => da.ValidateState(e.HomeAddressMexicoStateCanadaProvince, e.HomeAddressCountry))
                    .WithMessage("Home: Mexico (State)/Canada (Province): Selected state/province does not match the selected Country of Residence");
            });

            When(employee => employee.HomeAddressCountry != "CA" && employee.HomeAddressCountry != "MX", () =>
            {
                RuleFor(employee => employee.HomeAddressMexicoStateCanadaProvince)
                    .Empty()
                    .WithMessage("Home: Mexico (State)/Canada (Province): Should be left blank when Country of Residence is not Canada or Mexico");
            });

            //Home Address Zip US/MX
            RuleFor(employee => employee.HomeAddressZip)
                    .NotEmpty()
                    .Matches(@"^\d{5}(-\d{4})?$")
                    .When(employee => employee.HomeAddressCountry.Equals("US") || employee.HomeAddressCountry.Equals("MX"))
                    .WithMessage("Home: Zip: Postal code provided is not valid for the selected Country of Residence");

            //Home Address Zip CA
            RuleFor(employee => employee.HomeAddressZip)
                    .NotEmpty()
                    .Matches(@"^[ABCEGHJKLMNPRSTVXYabceghjklmnprstvxy]{1}\d{1}[A-Za-z]{1} *\d{1}[A-Za-z]{1}\d{1}$")
                    .When(employee => employee.HomeAddressCountry.Equals("CA"))
                    .WithMessage("Home: Zip: Postal code provided is not valid for the selected Country of Residence");

            //Home Address Zip not US/MX/CA
            RuleFor(employee => employee.HomeAddressZip)
                    .Empty()
                    .When(employee => !employee.HomeAddressCountry.Equals("US") && !employee.HomeAddressCountry.Equals("MX") && !employee.HomeAddressCountry.Equals("CA"))
                    .WithMessage("Home: ZIP: Postal codes are not accepted for the selected Country of Residence");

            When(employee => !employee.PhoneNumberWorkCell.Equals(""), () =>
            {
                //Phone Number (Work Cell)
                RuleFor(employee => employee.PhoneNumberWorkCell)
                        .NotEmpty()
                        .Matches(@"^\(?([0-9]{3})\)?[-.●]?([0-9]{3})[-.●]?([0-9]{4})$")
                        .WithMessage("Phone Number (Work Cell): Invalid Phone Number");
            });

            When(employee => !employee.PhoneNumberWork.Equals(""), () =>
            {
                //Phone Number (Work Number)
                RuleFor(employee => employee.PhoneNumberWork)
                        .NotEmpty()
                        .Matches(@"^\(?([0-9]{3})\)?[-.●]?([0-9]{3})[-.●]?([0-9]{4})$")
                        .WithMessage("Phone Number (Work Number): Invalid Phone Number");
            });

            //Personal E-Mail
            RuleFor(employee => employee.PersonalEmailAddress)
                    .NotEmpty()
                    .WithMessage("Personal E-Mail: Required Field")
                    .EmailAddress()
                    .WithMessage("Personal E-Mail: Invalid E-Mail Address");

            //Job Title
            RuleFor(employee => employee.PositionJobTitle)
                    .NotEmpty()
                    .WithMessage("Position (Job) Title: Required Field")
                    .Matches(@"^[a-zA-Z0-9 .\\-\\\']+$")
                    .WithMessage("Position (Job) Title: Contains Invalid Characters");

            //Prior Investigation
            RuleFor(employee => employee.PriorInvestigation)
                    .NotEmpty()
                    .WithMessage("Prior Investigation: Required Field");

            //Agency Adjudicated Prior Investigation (When Yes)
            When(employee => employee.PriorInvestigation.Equals("Yes"), () =>
            {
                RuleFor(employee => employee.ApproximiateInvestigationDate)
                        .NotEmpty()
                        .WithMessage("Approx. Investigation Date: Required Field")
                        .Must(U.DateIsValidAndNotFuture)
                        .WithMessage("Approx. Investigation Date: Invalid Date");

                RuleFor(employee => employee.AgencyAdjudicatedPriorInvestigation)
                        .NotEmpty()
                        .WithMessage("Agency Adjudicated Prior Investigation: Required Field")
                        .Matches(@"^[a-zA-Z ]+$")
                        .WithMessage("Agency Adjudicated Prior Investigation: Contains Invalid Characters");
            });

            //Agency Adjudicated Prior Investigation (When No)
            When(employee => employee.PriorInvestigation.Equals("No"), () =>
            {                
                RuleFor(employee => employee.ApproximiateInvestigationDate)
                        .Must(U.IsNotWhiteSpace)
                        .WithMessage("Approx. Investigation Date: The date that you have entered contains only spaces and is not valid")
                        .Empty()
                        .WithMessage("Prior Investigation: The information regarding your previous background investigation requires your attention, please be advised that when indicating 'No' in the Prior Investigation field, Approx. Investigation Date field must be left blank");

                RuleFor(employee => employee.AgencyAdjudicatedPriorInvestigation)
                        .Empty()
                        .WithMessage("Prior Investigation: The information regarding your previous background investigation requires your attention, please be advised that when indicating 'No' in the Prior Investigation field, Agency Adjudicated Prior investigation field must be left blank");
            });

            //US Citizen
            RuleFor(employee => employee.Citizen)
                    .NotEmpty()
                    .WithMessage("U.S. Citizen: Required Field");

            When(e => (e.CitzenshipCountry.Equals("US")), () =>
            {
                RuleFor(employee => employee.Citizen)
                    .Equal("Yes")
                    .WithMessage("Citizenship Country: Citizenship Country field value contradicts the input for U.S. Citizen");
            });

            When(e => !(e.CitzenshipCountry.Equals("US")), () =>
            {
                RuleFor(employee => employee.Citizen)
                    .NotEqual("Yes")
                    .WithMessage("Citizenship Country: Citizenship Country field value contradicts the input for U.S. Citizen");
            });

            //Port of Entry
            When(e => (e.Citizen.Equals("No") && !e.CitzenshipCountry.Equals("US")), () =>
            {
                //Port Of Entry US City And State
                RuleFor(employee => employee.PortOfEntryUSCityAndState)
                    .NotEmpty()
                    .WithMessage("Port of Entry, US City and State: Required Field")
                    .Matches(@"^[a-zA-Z\-\. \'\,]{1,50}$")
                    .WithMessage("Port of Entry, US City and State: Contains Invalid Characters");

                //date of entry
                RuleFor(employee => employee.DateOfEntry)
                   .NotEmpty()
                   .WithMessage("Date Of Entry: Required Field")
                   .Must(U.DateIsValidAndNotFuture)
                   .WithMessage("Date Of Entry: Invalid Date");

                //Less than 3 Yrs. U.S. Resident
                RuleFor(employee => employee.LessThanThreeYearsResident)
                        .NotEmpty()
                        .WithMessage("Less than 3 Yrs. U.S. Resident: Required Field");

                //Alien Registration Number
                RuleFor(employee => employee.AlienRegistrationNumber)
                        .NotEmpty()
                        .WithMessage("Alien Registration #: Required Field")
                        .Matches(@"^[a-zA-Z0-9\- ]+$")
                        .WithMessage("Alien Registration #: The value provided is not a valid registration number");


            });

            //Citizenship Country
            RuleFor(employee => employee.CitzenshipCountry)
                    .NotEmpty()
                    .WithMessage("Citizenship Country: Required Field");
        }
    }

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
                        .Must((c,x) => U.BeAValidEndDate(c.ContractEndDate,YearsInTheFuture))
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
                            .Must((c,x) => U.BeAValidEndDate(c.ContractEndDate,YearsInTheFuture))
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

    /// <summary>
    /// Section 3 Validation
    /// </summary>
    class RWAIAAValidator : AbstractValidator<CIW>
    {

        /// <summary>
        /// Contains all the validation rules for section 3
        /// </summary>
        public RWAIAAValidator()
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

    /// <summary>
    /// Section 4 Validation
    /// </summary>
    class ProjectLocationValidator : AbstractValidator<CIW>
    {
        private readonly IDataAccess da = Utilities.DataAccess.GetInstance(new MySqlConnection(ConfigurationManager.ConnectionStrings["GCIMS"].ToString()));

        /// <summary>
        /// Contains all the validation rules for section 4
        /// </summary>
        public ProjectLocationValidator()
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

    /// <summary>
    /// Section 6 Validation
    /// </summary>
    class RequestingOfficialValidator : AbstractValidator<CIW>
    {
        private readonly IDataAccess da = Utilities.DataAccess.GetInstance(new MySqlConnection(ConfigurationManager.ConnectionStrings["GCIMS"].ToString()));
        
        /// <summary>
        /// Contains all the validation rules for section 6
        /// </summary>
        public RequestingOfficialValidator()
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

    /// <summary>
    /// Class VallidateCIW
    /// Does validation for entire CIW form
    /// </summary>
    class ValidateCIW : IValidateCIW
    {
        private readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        ValidationResult section1 = new ValidationResult();
        ValidationResult section2 = new ValidationResult();
        ValidationResult section3 = new ValidationResult();
        ValidationResult section4 = new ValidationResult();
        ValidationResult section5 = new ValidationResult();
        ValidationResult section6 = new ValidationResult();

        public ValidateCIW()
        {
        }

        /// <summary>
        /// Calls validation for duplicate users
        /// </summary>
        /// <param name="ciwInformation"></param>
        /// <returns></returns>
        public bool IsDuplicate(List<CIW> ciwInformation)
        {
            UserExistsValidator validator = new UserExistsValidator();

            ValidationResult duplicate = validator.Validate(ciwInformation.First());

            return duplicate.IsValid;
        }

        /// <summary>
        /// Calls validation for sections 1 through 6
        /// </summary>
        /// <param name="ciwInformation"></param>
        /// <returns></returns>
        public bool IsFormValid(List<CIW> ciwInformation)
        {
            //Section 1
            ValidateEmployeeInformation(ciwInformation);
            log.Info(String.Format("Section 1 validation completed with {0} errors", section1.Errors.Count));

            if (section1.Errors.Count > 0)
                PrintToLog(section1.Errors,1);

            //Section 2
            ValidateContractInformation(ciwInformation);
            log.Info(String.Format("Section 2 validation completed with {0} errors", section2.Errors.Count));

            if (section2.Errors.Count > 0)
                PrintToLog(section2.Errors,2);

            //Section 3
            ValidateRWAIAAInformation(ciwInformation);
            log.Info(String.Format("Section 3 validation completed with {0} errors", section3.Errors.Count));

            if (section3.Errors.Count > 0)
                PrintToLog(section3.Errors,3);

            //Section 4
            ValidateProjectLocationInformation(ciwInformation);
            log.Info(String.Format("Section 4 validation completed with {0} errors", section4.Errors.Count));

            if (section4.Errors.Count > 0)
                PrintToLog(section4.Errors,4);

            //Section 5
            ValidateInvestigationRequested(ciwInformation);
            log.Info(String.Format("Section 5 validation completed with {0} errors", section5.Errors.Count));

            if (section5.Errors.Count > 0)
                PrintToLog(section5.Errors,5);

            //Section 6
            ValidateGSARequestionOfficialInformation(ciwInformation);
            log.Info(String.Format("Section 6 validation completed with {0} errors", section6.Errors.Count));

            if (section6.Errors.Count > 0)
                PrintToLog(section6.Errors,6);

            //Verify all sections are valid and return result
            if ((section1.IsValid && section2.IsValid && section3.IsValid && section4.IsValid && section5.IsValid && section6.IsValid) == false)
                return false;

            return true;
        }

        /// <summary>
        /// Prints errors to console
        /// </summary>
        public void PrintErrors()
        {
            PrintToConsole(section1.Errors, "Section 1: ");
            PrintToConsole(section2.Errors, "Section 2: ");
            PrintToConsole(section3.Errors, "Section 3: ");
            PrintToConsole(section4.Errors, "Section 4: ");
            PrintToConsole(section5.Errors, "Section 5: ");
            PrintToConsole(section6.Errors, "Section 6: ");
        }

        /// <summary>
        /// Gets all errors
        /// </summary>
        /// <returns>Tuple of errors</returns>
        public Tuple<ValidationResult, ValidationResult, ValidationResult, ValidationResult, ValidationResult, ValidationResult> GetErrors()
        {
            return new Tuple<ValidationResult, ValidationResult, ValidationResult, ValidationResult, ValidationResult, ValidationResult>(section1, section2, section3, section4, section5, section6);
        }

        /// <summary>
        /// Takes a list of validation errors and prints them to the console
        /// </summary>
        /// <param name="failures"></param>
        /// <param name="section"></param>
        private void PrintToConsole(IList<ValidationFailure> failures, string section)
        {
            foreach (var rule in failures)
            {
                Console.WriteLine(section + rule.ErrorMessage);
            }
        }

        /// <summary>
        /// Takes a list of validation errors and prints them to the log
        /// </summary>
        /// <param name="failures"></param>
        private void PrintToLog(IList<ValidationFailure> failures, int section)
        {
            foreach (var rule in failures)
            {
                //Print values only if not section 1 unless PropertyName is job title or part of name
                if (section != 1 || rule.PropertyName == "PositionJobTitle" || rule.PropertyName == "LastName" || rule.PropertyName == "FirstName" || rule.PropertyName == "MiddleName")
                {
                    log.Warn(string.Format("{0} failed with attempted value {1}", rule.PropertyName, String.IsNullOrWhiteSpace(rule.AttemptedValue.ToString()) ? "Empty" : '"' + rule.AttemptedValue.ToString() + '"'));
                }
                else
                {
                    log.Warn(string.Format("{0} failed with attempted value {1}", rule.PropertyName, rule.AttemptedValue == null ? "Null" : rule.AttemptedValue.Equals("") ? "Empty" : "PII"));
                }
            }
        }

        /// <summary>
        /// Call Section 1 validation
        /// </summary>
        /// <param name="employeeInforamtion"></param>
        private void ValidateEmployeeInformation(List<CIW> ciwInformation)
        {
            EmployeeValidator validator = new EmployeeValidator();

            section1 = validator.Validate(ciwInformation.First());
        }

        /// <summary>
        /// Call Section 2 validation
        /// </summary>
        /// <param name="ciwInformation"></param>
        private void ValidateContractInformation(List<CIW> ciwInformation)
        {
            ContractorValidator validator = new ContractorValidator();

            section2 = validator.Validate(ciwInformation.First(), ruleSet: "ValidFirstAndSecondRow,ValidPOCRow1,ValidPOCRow2,ValidPOCRow3,ValidPOCRow4,ValidPOCRow5");

            if (section2.IsValid)
            {
                ciwInformation.First().VendorPOC = new List<POC.VendorPOC>();

                //Add each vendor poc to list if not empty by using helper function
                AddVendorPOC(
                    ciwInformation.First().ContractPOCFirstName,
                    ciwInformation.First().ContractPOCLastName,
                    ciwInformation.First().ContractPOCEMailAddress,
                    ciwInformation.First().ContractPOCPhoneWork,
                    ciwInformation
                );

                AddVendorPOC(
                    ciwInformation.First().ContractPOCAlternatePocFirstname1,
                    ciwInformation.First().ContractPOCAlternatePocLastname1,
                    ciwInformation.First().ContractPOCAlternatePocEmail1,
                    ciwInformation.First().ContractPOCAlternatePocPhoneWork1,
                    ciwInformation
                );

                AddVendorPOC(
                    ciwInformation.First().ContractPOCAlternatePocFirstname2,
                    ciwInformation.First().ContractPOCAlternatePocLastname2,
                    ciwInformation.First().ContractPOCAlternatePocEmail2,
                    ciwInformation.First().ContractPOCAlternatePocPhoneWork2,
                    ciwInformation
                );

                AddVendorPOC(
                    ciwInformation.First().ContractPOCAlternatePocFirstname3,
                    ciwInformation.First().ContractPOCAlternatePocLastname3,
                    ciwInformation.First().ContractPOCAlternatePocEmail3,
                    ciwInformation.First().ContractPOCAlternatePocPhoneWork3,
                    ciwInformation
                );

                AddVendorPOC(
                    ciwInformation.First().ContractPOCAlternatePocFirstname4,
                    ciwInformation.First().ContractPOCAlternatePocLastname4,
                    ciwInformation.First().ContractPOCAlternatePocEmail4,
                    ciwInformation.First().ContractPOCAlternatePocPhoneWork4,
                    ciwInformation
                );
            }
        }
        private void AddVendorPOC(string fName, string lName, string email, string phone, List<CIW> ciwInfo)
        {
            if (!string.IsNullOrEmpty(fName) && !string.IsNullOrEmpty(lName) && !string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(phone))
            {
                ciwInfo.First().VendorPOC.Add(new POC.VendorPOC
                {
                    FirstName = fName,
                    LastName = lName,
                    EMail = email,
                    WorkPhone = phone
                });
            }
        }

        /// <summary>
        /// Call Section 3 validation
        /// </summary>
        /// <param name="ciwInformation"></param>
        private void ValidateRWAIAAInformation(List<CIW> ciwInformation)
        {
            RWAIAAValidator validator = new RWAIAAValidator();

            section3 = validator.Validate(ciwInformation.First());
        }

        /// <summary>
        /// Call Section 4 validation
        /// </summary>
        /// <param name="ciwInformation"></param>
        private void ValidateProjectLocationInformation(List<CIW> ciwInformation)
        {
            ProjectLocationValidator validator = new ProjectLocationValidator();

            section4 = validator.Validate(ciwInformation.First());
        }

        /// <summary>
        /// Call Section 5 validation
        /// </summary>
        /// <param name="ciwInformation"></param>
        private void ValidateInvestigationRequested(List<CIW> ciwInformation)
        {
            InvestigationValidator validator = new InvestigationValidator();

            section5 = validator.Validate(ciwInformation.First());
        }

        /// <summary>
        /// Call Section 6 validation
        /// </summary>
        /// <param name="ciwInformation"></param>
        private void ValidateGSARequestionOfficialInformation(List<CIW> ciwInformation)
        {
            RequestingOfficialValidator validator = new RequestingOfficialValidator();

            section6 = validator.Validate(ciwInformation.First(), ruleSet: "ValidGSARow1,ValidGSARow2,ValidGSARow3,ValidGSARow4,ValidGSARow5");

            if (section6.IsValid)
            {
                ciwInformation.First().GSAPOC = new List<POC.GSAPOC>();

                //Add each row to a list of GSAPOC's checking to make sure its not empty first using helper function
                AddGSAPOC(
                    ciwInformation.First().SponsorEmailAddress,
                    ciwInformation.First().SponsorIsPMCORCO,
                    ciwInformation
                );

                AddGSAPOC(
                    ciwInformation.First().SponsorAlternateEmailAddress1,
                    ciwInformation.First().SponsorAlternateIsPMCORCO1,
                    ciwInformation
                );

                AddGSAPOC(
                    ciwInformation.First().SponsorAlternateEmailAddress2,
                    ciwInformation.First().SponsorAlternateIsPMCORCO2,
                    ciwInformation
                );

                AddGSAPOC(
                    ciwInformation.First().SponsorAlternateEmailAddress3,
                    ciwInformation.First().SponsorAlternateIsPMCORCO3,
                    ciwInformation
                );

                AddGSAPOC(
                    ciwInformation.First().SponsorAlternateEmailAddress4,
                    ciwInformation.First().SponsorAlternateIsPMCORCO4,
                    ciwInformation
                );
            }
        }

        /// <summary>
        /// Helper function that adds GSAPOC to list if not empty
        /// </summary>
        /// <param name="email"></param>
        /// <param name="pmCorCoCs"></param>
        /// <param name="ciwInfo"></param>
        private void AddGSAPOC(string email, string pmCorCoCs, List<CIW> ciwInfo)
        {
            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(pmCorCoCs))
            {
                ciwInfo.First().GSAPOC.Add(new POC.GSAPOC
                {
                    EMail = email,
                    IsPM_COR_CO_CS = pmCorCoCs
                });
            }
        }
    }
}