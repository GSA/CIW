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
    /// Section 1 Validation
    /// </summary>
    class EmployeeValidator : AbstractValidator<CIW>
    {

        /// <summary>
        /// Contains all the validation rules for Section 1 of the CIW
        /// </summary>
        public EmployeeValidator(IDataAccess da, IUtilities U)
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
}
