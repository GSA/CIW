using FluentValidation;
using FluentValidation.Results;
using ProcessCIW.Interface;
using ProcessCIW.Mapping;
using ProcessCIW.Models;
using ProcessCIW.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProcessCIW.Validation
{    
    /// <summary>
    /// Class VallidateCIW
    /// Does validation for entire CIW form
    /// </summary>
    class ValidateCiw : IValidateCiw
    {
        readonly IDataAccess da;
        readonly IUtilities U;
        private readonly ILogTool log;
        ValidationResult section1 = new ValidationResult();
        ValidationResult section2 = new ValidationResult();
        ValidationResult section3 = new ValidationResult();
        ValidationResult section4 = new ValidationResult();
        ValidationResult section5 = new ValidationResult();
        ValidationResult section6 = new ValidationResult();

        public ValidateCiw(IDataAccess da, IUtilities U, ILogTool log)
        {
            this.da = da;
            this.U = U;
            this.log = log;
        }

        /// <summary>
        /// Calls validation for duplicate users
        /// </summary>
        /// <param name="ciwInformation"></param>
        /// <returns></returns>
        public bool IsDuplicate(List<CIW> ciwInformation)
        {
            UserExistsValidator validator = new UserExistsValidator(da);

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
            EmployeeValidator validator = new EmployeeValidator(da,U);

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
            RwaIaaValidator validator = new RwaIaaValidator();

            section3 = validator.Validate(ciwInformation.First());
        }

        /// <summary>
        /// Call Section 4 validation
        /// </summary>
        /// <param name="ciwInformation"></param>
        private void ValidateProjectLocationInformation(List<CIW> ciwInformation)
        {
            ProjectLocationValidator validator = new ProjectLocationValidator(da);

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
            RequestingOfficialValidator validator = new RequestingOfficialValidator(da);

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