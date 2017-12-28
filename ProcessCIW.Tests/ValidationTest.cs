using System.Linq;
using FluentValidation.TestHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProcessCIW.Models;
using ProcessCIW.Validation;

namespace ProcessCIW.Tests
{
    [TestClass]
    public class ValidationTest
    {
        private ContractorValidator validator;

        private readonly string validFirstName = new string('x', 10);
        private readonly string validLastName = new string('x', 10);
        private const string ValidEmailAddress = "tester@test.com";
        private const string ValidPhoneNumber = "(301)555-7089";
        private const string ValidContractEndDate = "12/31/2030";

        private const string InvalidFirstName = "InvalidFirstName1";
        private const string InvalidLastName = "InvalidLastName1";
        private const string InvalidEmailAddress = "tester@test";
        private const string InvalidPhoneNumber = "(301)555-7X89";
        private readonly string invalidLengthName = new string('x', 46);

        #region Valid First and Second Row

        [TestMethod]
        public void Validation_Fails_When_ContractStartDate_IsEmpty()
        {
            // Arrange
            validator = new ContractorValidator();

            var ciw = new CIW()
            {
                ContractStartDate = string.Empty,
                DataUniversalNumberingSystem = string.Empty,
                HasOptionYears = "No",
                NumberOfOptionYears = string.Empty,
                ContractEndDate = ValidContractEndDate
            };

            const string expectedErrorMessage = "Contract Start Date: Required Field";

            // Act / Assert
            var result = validator.ShouldHaveValidationErrorFor(x => x.ContractStartDate, ciw, "ValidFirstAndSecondRow");
            var errorMessage = result.FirstOrDefault(x => x.ErrorMessage == expectedErrorMessage);
            Assert.IsNotNull(errorMessage);
        }

        [TestMethod]
        public void Validation_Fails_When_ContractStartDate_Is_Not_Valid()
        {
            // Arrange
            validator = new ContractorValidator();

            var ciw = new CIW()
            {
                ContractStartDate = "NOT A DATE",
                DataUniversalNumberingSystem = string.Empty,
                HasOptionYears = "No",
                NumberOfOptionYears = string.Empty,
                ContractEndDate = ValidContractEndDate
            };

            const string expectedErrorMessage = "Contract Start Date: Invalid Date";

            // Act / Assert
            var result = validator.ShouldHaveValidationErrorFor(x => x.ContractStartDate, ciw, "ValidFirstAndSecondRow");
            var errorMessage = result.FirstOrDefault(x => x.ErrorMessage == expectedErrorMessage);
            Assert.IsNotNull(errorMessage);
        }

        [TestMethod]
        public void Validation_Fails_When_ContractStartDate_Is_Starts_After_ContractEndDate()
        {
            // Arrange
            validator = new ContractorValidator();

            var ciw = new CIW()
            {
                ContractStartDate = "1/1/2031",
                DataUniversalNumberingSystem = string.Empty,
                HasOptionYears = "No",
                NumberOfOptionYears = string.Empty,
                ContractEndDate = ValidContractEndDate
            };

            const string expectedErrorMessage =
                "Contract Start Date: Cannot be later than Contract End Date";

            // Act / Assert
            var result = validator.ShouldHaveValidationErrorFor(x => x.ContractStartDate, ciw, "ValidFirstAndSecondRow");
            var errorMessage = result.FirstOrDefault(x => x.ErrorMessage == expectedErrorMessage).ErrorMessage;
            Assert.IsNotNull(errorMessage);
        }

        [TestMethod]
        public void Validation_Passes_When_ContractStartDate_And_ContractEndDate_Are_Valid()
        {
            // Arrange
            validator = new ContractorValidator();

            var ciw = new CIW()
            {
                ContractStartDate = "1/1/2017",
                DataUniversalNumberingSystem = string.Empty,
                HasOptionYears = "No",
                NumberOfOptionYears = string.Empty,
                ContractEndDate = ValidContractEndDate
            };

            // Act / Assert
            validator.ShouldNotHaveValidationErrorFor(x => x.ContractStartDate, ciw, "ValidFirstAndSecondRow");
        }

        #endregion

        #region ValidPOCRow1 RuleSet Tests

        [TestMethod]
        public void Validation_Fails_When_ContractPOCFirstName_Exceeds_CharacterLength()
        {
            // Arrange
            validator = new ContractorValidator();

            var ciw = new CIW()
            {
                ContractPOCFirstName = invalidLengthName
            };

            const string expectedErrorMessage = 
                "Primary Company Point of Contact(POC) First Name: exceeds maximum number of characters. Please double-check the field. If value is correct, please reach out to HSPD-12 Security at HSPD12.Security@gsa.gov or at +1 (202) 501-4459.";

            // Act / Assert
            var result = validator.ShouldHaveValidationErrorFor(x => x.ContractPOCFirstName, ciw, "ValidPOCRow1");
            var errorMessage = result.FirstOrDefault(x => x.ErrorMessage == expectedErrorMessage);
            Assert.IsNotNull(errorMessage);
        }

        [TestMethod]
        public void Validation_Fails_When_ContractPOCLastName_Exceeds_CharacterLength()
        {
            // Arrange
            validator = new ContractorValidator();

            var ciw = new CIW()
            {
                ContractPOCLastName = invalidLengthName
            };

            const string expectedErrorMessage =
                "Primary Company Point of Contact(POC) Last Name: exceeds maximum number of characters. Please double-check the field. If value is correct, please reach out to HSPD-12 Security at HSPD12.Security@gsa.gov or at +1 (202) 501-4459.";

            // Act / Assert
            var result = validator.ShouldHaveValidationErrorFor(x => x.ContractPOCLastName, ciw, "ValidPOCRow1");
            var errorMessage = result.FirstOrDefault(x => x.ErrorMessage == expectedErrorMessage);
            Assert.IsNotNull(errorMessage);
        }

        [TestMethod]
        public void Validation_Fails_When_ContractPOCFirstName_Is_Empty()
        {
            // Arrange
            validator = new ContractorValidator();

            var ciw = new CIW()
            {
                ContractPOCFirstName = string.Empty
            };

            const string expectedErrorMessage = 
                "Primary Company Point of Contact(POC) First Name: Required Field";

            // Act / Assert
            var result = validator.ShouldHaveValidationErrorFor(x => x.ContractPOCFirstName, ciw, "ValidPOCRow1");
            var errorMessage = result.FirstOrDefault(x => x.ErrorMessage == expectedErrorMessage);
            Assert.IsNotNull(errorMessage);
        }

        [TestMethod]
        public void Validation_Fails_When_ContractPOCLastName_Is_Empty()
        {
            // Arrange
            validator = new ContractorValidator();

            var ciw = new CIW()
            {
                ContractPOCLastName = string.Empty
            };

            const string expectedErrorMessage = 
                "Primary Company Point of Contact(POC) Last Name: Required Field";

            // Act / Assert
            var result = validator.ShouldHaveValidationErrorFor(x => x.ContractPOCLastName, ciw, "ValidPOCRow1");
            var errorMessage = result.FirstOrDefault(x => x.ErrorMessage == expectedErrorMessage);
            Assert.IsNotNull(errorMessage);
        }

        [TestMethod]
        public void Validation_Fails_When_ContractPOCFirstName_Regex_Fails()
        {
            // Arrange
            validator = new ContractorValidator();

            var ciw = new CIW()
            {
                ContractPOCFirstName = InvalidFirstName
            };

            const string expectedErrorMessage =
                "Primary Company Point of Contact(POC) First Name: Contains Invalid Characters";

            // Act / Assert
            var result = validator.ShouldHaveValidationErrorFor(x => x.ContractPOCFirstName, ciw, "ValidPOCRow1");
            var errorMessage = result.FirstOrDefault(x => x.ErrorMessage == expectedErrorMessage);
            Assert.IsNotNull(errorMessage);
        }

        [TestMethod]
        public void Validation_Fails_When_ContractPOCFLastName_Regex_Fails()
        {
            // Arrange
            validator = new ContractorValidator();

            var ciw = new CIW()
            {
                ContractPOCLastName = InvalidLastName
            };

            const string expectedErrorMessage = 
                "Primary Company Point of Contact(POC) Last Name: Contains Invalid Characters";

            // Act / Assert
            var result = validator.ShouldHaveValidationErrorFor(x => x.ContractPOCLastName, ciw, "ValidPOCRow1");
            var errorMessage = result.FirstOrDefault(x => x.ErrorMessage == expectedErrorMessage);
            Assert.IsNotNull(errorMessage);
        }

        [TestMethod]
        public void Validation_Fails_When_ContractPOCPhoneWork_Is_Empty()
        {
            // Arrange  
            validator = new ContractorValidator();

            var ciw = new CIW()
            {
                ContractPOCPhoneWork = string.Empty
            };

            const string expectedErrorMessage = 
                "Primary Company Point of Contact(POC) Work Phone Number: Required Field";

            // Act / Assert
            var result = validator.ShouldHaveValidationErrorFor(x => x.ContractPOCPhoneWork, ciw, "ValidPOCRow1");
            var errorMessage = result.FirstOrDefault(x => x.ErrorMessage == expectedErrorMessage);
            Assert.IsNotNull(errorMessage);
        }

        [TestMethod]
        public void Validation_Fails_When_ContractPOCPhoneWork_Regex_Fails()
        {
            // Arrange
            validator = new ContractorValidator();

            var ciw = new CIW()
            {
                ContractPOCPhoneWork = InvalidPhoneNumber
            };

            const string expectedErrorMessage = 
                "Primary Company Point of Contact(POC) Work Phone Number: Invalid Phone Number";

            // Act / Assert
            var result = validator.ShouldHaveValidationErrorFor(x => x.ContractPOCPhoneWork, ciw, "ValidPOCRow1");
            var errorMessage = result.FirstOrDefault(x => x.ErrorMessage == expectedErrorMessage);
            Assert.IsNotNull(errorMessage);
        }

        [TestMethod]
        public void Validation_Fails_When_ContractPOCEMailAddress_Is_Empty()
        {
            // Arrange
            validator = new ContractorValidator();

            var ciw = new CIW()
            {
                ContractPOCEMailAddress = string.Empty
            };

            const string expectedErrorMessage = 
                "Primary Company Point of Contact(POC) E-Mail Address: Required Field";

            // Act / Assert
            var result = validator.ShouldHaveValidationErrorFor(x => x.ContractPOCEMailAddress, ciw, "ValidPOCRow1");
            var errorMessage = result.FirstOrDefault(x => x.ErrorMessage == expectedErrorMessage);
            Assert.IsNotNull(errorMessage);
        }

        [TestMethod]
        public void Validation_Fails_When_ContractPOCEMailAddress_Regex_Fails()
        {
            // Arrange
            validator = new ContractorValidator();

            var ciw = new CIW()
            {
                ContractPOCEMailAddress = InvalidEmailAddress
            };

            const string expectedErrorMessage =
                "Primary Company Point of Contact(POC) E-Mail Address: Invalid E-Mail Address";

            // Act / Assert
            var result = validator.ShouldHaveValidationErrorFor(x => x.ContractPOCEMailAddress, ciw, "ValidPOCRow1");
            var errorMessage = result.FirstOrDefault(x => x.ErrorMessage == expectedErrorMessage);
            Assert.IsNotNull(errorMessage);
        }

        [TestMethod]
        public void Validation_Passes_When_ContractPOC_Is_Valid()
        {
            // Arrange
            validator = new ContractorValidator();

            var ciw = new CIW()
            {
                ContractPOCFirstName = validFirstName,
                ContractPOCLastName = validLastName,
                ContractPOCPhoneWork = ValidPhoneNumber,
                ContractPOCEMailAddress = ValidEmailAddress
            };

            // Act / Assert
            validator.ShouldNotHaveValidationErrorFor(x => x.ContractPOCFirstName, ciw, "ValidPOCRow1");
            validator.ShouldNotHaveValidationErrorFor(x => x.ContractPOCLastName, ciw, "ValidPOCRow1");
            validator.ShouldNotHaveValidationErrorFor(x => x.ContractPOCPhoneWork, ciw, "ValidPOCRow1");
            validator.ShouldNotHaveValidationErrorFor(x => x.ContractPOCEMailAddress, ciw, "ValidPOCRow1");
        }

        #endregion

        #region ValidPOCRow2 RuleSet Tests

        [TestMethod]
        public void Validation_Fails_When_ContractPOCAlternatePocFirstname1_Exceeds_CharacterLength()
        {
            // Arrange
            validator = new ContractorValidator();

            var ciw = new CIW()
            {
                ContractPOCAlternatePocFirstname1 = invalidLengthName,
                ContractPOCAlternatePocLastname1 = validLastName,
                ContractPOCAlternatePocPhoneWork1 = ValidPhoneNumber,
                ContractPOCAlternatePocEmail1 = ValidEmailAddress
            };

            const string expectedErrorMessage = 
                "Alternate Company Point of Contact(POC) 1: First Name: exceeds maximum number of characters. Please double-check the field. If value is correct, please reach out to HSPD-12 Security at HSPD12.Security@gsa.gov or at +1 (202) 501-4459.";

            // Act / Assert
            var result = validator.ShouldHaveValidationErrorFor(x => x.ContractPOCAlternatePocFirstname1, ciw, "ValidPOCRow2");
            var errorMessage = result.FirstOrDefault(x => x.ErrorMessage == expectedErrorMessage);
            Assert.IsNotNull(errorMessage);
        }

        [TestMethod]
        public void Validation_Fails_When_ContractPOCAlternatePocLastname1_Exceeds_CharacterLength()
        {
            // Arrange
            validator = new ContractorValidator();

            var ciw = new CIW()
            {
                ContractPOCAlternatePocFirstname1 = validFirstName,
                ContractPOCAlternatePocLastname1 = invalidLengthName,
                ContractPOCAlternatePocPhoneWork1 = ValidPhoneNumber,
                ContractPOCAlternatePocEmail1 = ValidEmailAddress
            };

            const string expectedErrorMessage =
                "Alternate Company Point of Contact(POC) 1: Last Name: exceeds maximum number of characters. Please double-check the field. If value is correct, please reach out to HSPD-12 Security at HSPD12.Security@gsa.gov or at +1 (202) 501-4459.";

            // Act / Assert
            var result = validator.ShouldHaveValidationErrorFor(x => x.ContractPOCAlternatePocLastname1, ciw, "ValidPOCRow2");
            var errorMessage = result.FirstOrDefault(x => x.ErrorMessage == expectedErrorMessage);
            Assert.IsNotNull(errorMessage);
        }

        [TestMethod]
        public void Validation_Fails_When_ContractPOCAlternatePocPhoneWork1_Is_Empty()
        {
            // Arrange
            validator = new ContractorValidator();

            var ciw = new CIW()
            {
                ContractPOCAlternatePocFirstname1 = validFirstName,
                ContractPOCAlternatePocLastname1 = validLastName,
                ContractPOCAlternatePocPhoneWork1 = string.Empty,
                ContractPOCAlternatePocEmail1 = ValidEmailAddress
            };

            const string expectedErrorMessage =
                "Alternate Company Point of Contact(POC) 1: Phone Number: Required Field";

            // Act / Assert
            var result = validator.ShouldHaveValidationErrorFor(x => x.ContractPOCAlternatePocPhoneWork1, ciw, "ValidPOCRow2");
            var errorMessage = result.FirstOrDefault(x => x.ErrorMessage == expectedErrorMessage);
            Assert.IsNotNull(errorMessage);
        }

        [TestMethod]
        public void Validation_Fails_When_ContractPOCAlternatePocPhoneWork1_Is_Invalid()
        {
            // Arrange
            validator = new ContractorValidator();

            var ciw = new CIW()
            {
                ContractPOCAlternatePocFirstname1 = validFirstName,
                ContractPOCAlternatePocLastname1 = validLastName,
                ContractPOCAlternatePocPhoneWork1 = InvalidPhoneNumber,
                ContractPOCAlternatePocEmail1 = ValidEmailAddress
            };

            const string expectedErrorMessage = 
                "Alternate Company Point of Contact(POC) 1: Phone Number: Invalid Phone Number";

            // Act / Assert
            var result = validator.ShouldHaveValidationErrorFor(x => x.ContractPOCAlternatePocPhoneWork1, ciw, "ValidPOCRow2");
            var errorMessage = result.FirstOrDefault(x => x.ErrorMessage == expectedErrorMessage);
            Assert.IsNotNull(errorMessage);
        }

        [TestMethod]
        public void Validation_Fails_When_ContractPOCAlternatePocEmail1_Is_Invalid()
        {
            // Arrange
            validator = new ContractorValidator();

            var ciw = new CIW()
            {
                ContractPOCAlternatePocFirstname1 = validFirstName,
                ContractPOCAlternatePocLastname1 = validLastName,
                ContractPOCAlternatePocPhoneWork1 = ValidPhoneNumber,
                ContractPOCAlternatePocEmail1 = InvalidEmailAddress
            };

            const string expectedErrorMessage = 
                "Alternate Company Point of Contact(POC) 1: E-Mail Address: Invalid E-Mail Address";

            // Act / Assert
            var result = validator.ShouldHaveValidationErrorFor(x => x.ContractPOCAlternatePocEmail1, ciw, "ValidPOCRow2");
            var errorMessage = result.FirstOrDefault(x => x.ErrorMessage == expectedErrorMessage);
            Assert.IsNotNull(errorMessage);
        }

        [TestMethod]
        public void Validation_Passes_When_ContractPOCAlternate1_Fields_Are_Valid()
        {
            // Arrange
            validator = new ContractorValidator();

            var ciw = new CIW()
            {
                ContractPOCAlternatePocFirstname1 = validFirstName,
                ContractPOCAlternatePocLastname1 = validLastName,
                ContractPOCAlternatePocPhoneWork1 = ValidPhoneNumber,
                ContractPOCAlternatePocEmail1 = ValidEmailAddress
            };

            // Act / Assert
            validator.ShouldNotHaveValidationErrorFor(x => x.ContractPOCAlternatePocFirstname1, ciw, "ValidPOCRow2");
            validator.ShouldNotHaveValidationErrorFor(x => x.ContractPOCAlternatePocLastname1, ciw, "ValidPOCRow2");
            validator.ShouldNotHaveValidationErrorFor(x => x.ContractPOCAlternatePocPhoneWork1, ciw, "ValidPOCRow2");
            validator.ShouldNotHaveValidationErrorFor(x => x.ContractPOCAlternatePocEmail1, ciw, "ValidPOCRow2");
        }

        #endregion

        #region ValidPOCRow3 RuleSet Tests

        [TestMethod]
        public void Validation_Fails_When_ContractPOCAlternatePocFirstname2_Exceeds_CharacterLength()
        {
            // Arrange
            validator = new ContractorValidator();

            var ciw = new CIW()
            {
                ContractPOCAlternatePocFirstname2 = invalidLengthName,
                ContractPOCAlternatePocLastname2 = validLastName,
                ContractPOCAlternatePocPhoneWork2 = ValidPhoneNumber,
                ContractPOCAlternatePocEmail2 = ValidEmailAddress
            };

            const string expectedErrorMessage = 
                "Alternate Company Point of Contact(POC) 2: First Name: exceeds maximum number of characters. Please double-check the field. If value is correct, please reach out to HSPD-12 Security at HSPD12.Security@gsa.gov or at +1 (202) 501-4459.";

            // Act / Assert
            var result = validator.ShouldHaveValidationErrorFor(x => x.ContractPOCAlternatePocFirstname2, ciw, "ValidPOCRow3");
            var errorMessage = result.FirstOrDefault(x => x.ErrorMessage == expectedErrorMessage);
            Assert.IsNotNull(errorMessage); ;
        }

        [TestMethod]
        public void Validation_Fails_When_ContractPOCAlternatePocFirstname2_Is_Empty()
        {
            // Arrange
            validator = new ContractorValidator();

            var ciw = new CIW()
            {
                ContractPOCAlternatePocFirstname2 = string.Empty,
                ContractPOCAlternatePocLastname2 = validLastName,
                ContractPOCAlternatePocPhoneWork2 = ValidPhoneNumber,
                ContractPOCAlternatePocEmail2 = ValidEmailAddress
            };

            const string expectedErrorMessage =
                "Alternate Company Point of Contact(POC) 2: First Name: Required Field";

            // Act / Assert
            var result = validator.ShouldHaveValidationErrorFor(x => x.ContractPOCAlternatePocFirstname2, ciw, "ValidPOCRow3");
            var errorMessage = result.FirstOrDefault(x => x.ErrorMessage == expectedErrorMessage);
            Assert.IsNotNull(errorMessage); ;
        }

        [TestMethod]
        public void Validation_Fails_When_ContractPOCAlternatePocFirstname2_Is_Invalid()
        {
            // Arrange
            validator = new ContractorValidator();

            var ciw = new CIW()
            {
                ContractPOCAlternatePocFirstname2 = InvalidFirstName,
                ContractPOCAlternatePocLastname2 = validLastName,
                ContractPOCAlternatePocPhoneWork2 = ValidPhoneNumber,
                ContractPOCAlternatePocEmail2 = ValidEmailAddress
            };

            const string expectedErrorMessage = 
                "Alternate Company Point of Contact(POC) 2: First Name: Contains Invalid Characters";

            // Act / Assert
            var result = validator.ShouldHaveValidationErrorFor(x => x.ContractPOCAlternatePocFirstname2, ciw, "ValidPOCRow3");
            var errorMessage = result.FirstOrDefault(x => x.ErrorMessage == expectedErrorMessage);
            Assert.IsNotNull(errorMessage); ;
        }

        [TestMethod]
        public void Validation_Fails_When_ContractPOCAlternatePocLastname2_Exceeds_CharacterLength()
        {
            // Arrange
            validator = new ContractorValidator();

            var ciw = new CIW()
            {
                ContractPOCAlternatePocFirstname2 = validFirstName,
                ContractPOCAlternatePocLastname2 = invalidLengthName,
                ContractPOCAlternatePocPhoneWork2 = ValidPhoneNumber,
                ContractPOCAlternatePocEmail2 = ValidEmailAddress
            };

            const string expectedErrorMessage = 
                "Alternate Company Point of Contact(POC) 2: Last Name: exceeds maximum number of characters. Please double-check the field. If value is correct, please reach out to HSPD-12 Security at HSPD12.Security@gsa.gov or at +1 (202) 501-4459.";

            // Act / Assert
            var result = validator.ShouldHaveValidationErrorFor(x => x.ContractPOCAlternatePocLastname2, ciw, "ValidPOCRow3");
            var errorMessage = result.FirstOrDefault(x => x.ErrorMessage == expectedErrorMessage);
            Assert.IsNotNull(errorMessage);
        }

        [TestMethod]
        public void Validation_Fails_When_ContractPOCAlternatePocLastname2_Is_Empty()
        {
            // Arrange
            validator = new ContractorValidator();

            var ciw = new CIW()
            {
                ContractPOCAlternatePocFirstname2 = validFirstName,
                ContractPOCAlternatePocLastname2 = string.Empty,
                ContractPOCAlternatePocPhoneWork2 = ValidPhoneNumber,
                ContractPOCAlternatePocEmail2 = ValidEmailAddress
            };

            const string expectedErrorMessage = 
                "Alternate Company Point of Contact(POC) 2: Last Name: Required Field";

            // Act / Assert
            var result = validator.ShouldHaveValidationErrorFor(x => x.ContractPOCAlternatePocLastname2, ciw, "ValidPOCRow3");
            var errorMessage = result.FirstOrDefault(x => x.ErrorMessage == expectedErrorMessage);
            Assert.IsNotNull(errorMessage);
        }

        [TestMethod]
        public void Validation_Fails_When_ContractPOCAlternatePocLastname2_Is_Invalid()
        {
            // Arrange
            validator = new ContractorValidator();

            var ciw = new CIW()
            {
                ContractPOCAlternatePocFirstname2 = validFirstName,
                ContractPOCAlternatePocLastname2 = InvalidLastName,
                ContractPOCAlternatePocPhoneWork2 = ValidPhoneNumber,
                ContractPOCAlternatePocEmail2 = ValidEmailAddress
            };

            const string expectedErrorMessage = 
                "Alternate Company Point of Contact(POC) 2: Last Name: Contains Invalid Characters";

            // Act / Assert
            var result = validator.ShouldHaveValidationErrorFor(x => x.ContractPOCAlternatePocLastname2, ciw, "ValidPOCRow3");
            var errorMessage = result.FirstOrDefault(x => x.ErrorMessage == expectedErrorMessage);
            Assert.IsNotNull(errorMessage); ;
        }

        [TestMethod]
        public void Validation_Fails_When_ContractPOCAlternatePocPhoneWork2_Is_Invalid()
        {
            // Arrange
            validator = new ContractorValidator();

            var ciw = new CIW()
            {
                ContractPOCAlternatePocFirstname2 = validFirstName,
                ContractPOCAlternatePocLastname2 = validLastName,
                ContractPOCAlternatePocPhoneWork2 = InvalidPhoneNumber,
                ContractPOCAlternatePocEmail2 = ValidEmailAddress
            };

            const string expectedErrorMessage = 
                "Alternate Company Point of Contact(POC) 2: Work Phone Number: Invalid Phone Number";

            // Act / Assert
            var result = validator.ShouldHaveValidationErrorFor(x => x.ContractPOCAlternatePocPhoneWork2, ciw, "ValidPOCRow3");
            var errorMessage = result.FirstOrDefault(x => x.ErrorMessage == expectedErrorMessage);
            Assert.IsNotNull(errorMessage);
        }

        [TestMethod]
        public void Validation_Fails_When_ContractPOCAlternatePocPhoneWork2_Is_Empty()
        {
            // Arrange
            validator = new ContractorValidator();

            var ciw = new CIW()
            {
                ContractPOCAlternatePocFirstname2 = validFirstName,
                ContractPOCAlternatePocLastname2 = validLastName,
                ContractPOCAlternatePocPhoneWork2 = string.Empty,
                ContractPOCAlternatePocEmail2 = ValidEmailAddress
            };

            const string expectedErrorMessage =
                "Alternate Company Point of Contact(POC) 2: Work Phone Number: Required Field";

            // Act / Assert
            var result = validator.ShouldHaveValidationErrorFor(x => x.ContractPOCAlternatePocPhoneWork2, ciw, "ValidPOCRow3");
            var errorMessage = result.FirstOrDefault(x => x.ErrorMessage == expectedErrorMessage);
            Assert.IsNotNull(errorMessage);
        }

        [TestMethod]
        public void Validation_Fails_When_ContractPOCAlternatePocEmail2_Is_Invalid()
        {
            // Arrange
            validator = new ContractorValidator();

            var ciw = new CIW()
            {
                ContractPOCAlternatePocFirstname2 = validFirstName,
                ContractPOCAlternatePocLastname2 = validLastName,
                ContractPOCAlternatePocPhoneWork2 = ValidPhoneNumber,
                ContractPOCAlternatePocEmail2 = InvalidEmailAddress
            };

            const string expectedErrorMessage = 
                "Alternate Company Point of Contact(POC) 2: E-Mail Address: Invalid E-Mail Address";

            // Act / Assert
            var result = validator.ShouldHaveValidationErrorFor(x => x.ContractPOCAlternatePocEmail2, ciw, "ValidPOCRow3");
            var errorMessage = result.FirstOrDefault(x => x.ErrorMessage == expectedErrorMessage);
            Assert.IsNotNull(errorMessage);
        }

        [TestMethod]
        public void Validation_Passes_When_ContractPOCAlternate2_Fields_Are_Valid()
        {
            // Arrange
            validator = new ContractorValidator();

            const string ruleSetName = "ValidPOCRow3";

            var ciw = new CIW()
            {
                ContractPOCAlternatePocFirstname2 = validFirstName,
                ContractPOCAlternatePocLastname2 = validLastName,
                ContractPOCAlternatePocPhoneWork2 = ValidPhoneNumber,
                ContractPOCAlternatePocEmail2 = ValidEmailAddress
            };

            // Act / Assert
            validator.ShouldNotHaveValidationErrorFor(x => x.ContractPOCAlternatePocFirstname2, ciw, ruleSetName);
            validator.ShouldNotHaveValidationErrorFor(x => x.ContractPOCAlternatePocLastname2, ciw, ruleSetName);
            validator.ShouldNotHaveValidationErrorFor(x => x.ContractPOCAlternatePocPhoneWork2, ciw, ruleSetName);
            validator.ShouldNotHaveValidationErrorFor(x => x.ContractPOCAlternatePocEmail2, ciw, ruleSetName);
        }

        #endregion

        #region ValidPOCRow4 RuleSet Tests

        [TestMethod]
        public void Validation_Fails_When_ContractPOCAlternatePocFirstname3_Exceeds_CharacterLength()
        {
            // Arrange
            validator = new ContractorValidator();

            var ciw = new CIW()
            {
                ContractPOCAlternatePocFirstname3 = invalidLengthName,
                ContractPOCAlternatePocLastname3 = validLastName,
                ContractPOCAlternatePocPhoneWork3 = ValidPhoneNumber,
                ContractPOCAlternatePocEmail3 = ValidEmailAddress
            };

            const string expectedErrorMessage = 
                "Alternate Company Point of Contact(POC) 3: First Name: exceeds maximum number of characters. Please double-check the field. If value is correct, please reach out to HSPD-12 Security at HSPD12.Security@gsa.gov or at +1 (202) 501-4459.";

            // Act / Assert
            var result = validator.ShouldHaveValidationErrorFor(x => x.ContractPOCAlternatePocFirstname3, ciw, "ValidPOCRow4");
            var errorMessage = result.FirstOrDefault(x => x.ErrorMessage == expectedErrorMessage);
            Assert.IsNotNull(errorMessage); ;
        }

        [TestMethod]
        public void Validation_Fails_When_ContractPOCAlternatePocFirstname3_Is_Empty()
        {
            // Arrange
            validator = new ContractorValidator();

            var ciw = new CIW()
            {
                ContractPOCAlternatePocFirstname3 = string.Empty,
                ContractPOCAlternatePocLastname3 = validLastName,
                ContractPOCAlternatePocPhoneWork3 = ValidPhoneNumber,
                ContractPOCAlternatePocEmail3 = ValidEmailAddress
            };

            const string expectedErrorMessage =
                "Alternate Company Point of Contact(POC) 3: First Name: Required Field";

            // Act / Assert
            var result = validator.ShouldHaveValidationErrorFor(x => x.ContractPOCAlternatePocFirstname3, ciw, "ValidPOCRow4");
            var errorMessage = result.FirstOrDefault(x => x.ErrorMessage == expectedErrorMessage);
            Assert.IsNotNull(errorMessage); ;
        }

        [TestMethod]
        public void Validation_Fails_When_ContractPOCAlternatePocFirstname3_Is_Invalid()
        {
            // Arrange
            validator = new ContractorValidator();

            var ciw = new CIW()
            {
                ContractPOCAlternatePocFirstname3 = InvalidFirstName,
                ContractPOCAlternatePocLastname3 = validLastName,
                ContractPOCAlternatePocPhoneWork3 = ValidPhoneNumber,
                ContractPOCAlternatePocEmail3 = ValidEmailAddress
            };

            const string expectedErrorMessage = 
                "Alternate Company Point of Contact(POC) 3: First Name: Contains Invalid Characters";

            // Act / Assert
            var result = validator.ShouldHaveValidationErrorFor(x => x.ContractPOCAlternatePocFirstname3, ciw, "ValidPOCRow4");
            var errorMessage = result.FirstOrDefault(x => x.ErrorMessage == expectedErrorMessage);
            Assert.IsNotNull(errorMessage); ;
        }

        [TestMethod]
        public void Validation_Fails_When_ContractPOCAlternatePocLastname3_Exceeds_CharacterLength()
        {
            // Arrange
            validator = new ContractorValidator();

            var ciw = new CIW()
            {
                ContractPOCAlternatePocFirstname3 = validFirstName,
                ContractPOCAlternatePocLastname3 = invalidLengthName,
                ContractPOCAlternatePocPhoneWork3 = ValidPhoneNumber,
                ContractPOCAlternatePocEmail3 = ValidEmailAddress
            };

            const string expectedErrorMessage =
                "Alternate Company Point of Contact(POC) 3: Last Name: exceeds maximum number of characters. Please double-check the field. If value is correct, please reach out to HSPD-12 Security at HSPD12.Security@gsa.gov or at +1 (202) 501-4459.";

            // Act / Assert
            var result = validator.ShouldHaveValidationErrorFor(x => x.ContractPOCAlternatePocLastname3, ciw, "ValidPOCRow4");
            var errorMessage = result.FirstOrDefault(x => x.ErrorMessage == expectedErrorMessage);
            Assert.IsNotNull(errorMessage);
        }

        [TestMethod]
        public void Validation_Fails_When_ContractPOCAlternatePocLastname3_Is_Empty()
        {
            // Arrange
            validator = new ContractorValidator();

            var ciw = new CIW()
            {
                ContractPOCAlternatePocFirstname3 = validFirstName,
                ContractPOCAlternatePocLastname3 = string.Empty,
                ContractPOCAlternatePocPhoneWork3 = ValidPhoneNumber,
                ContractPOCAlternatePocEmail3 = ValidEmailAddress
            };

            const string expectedErrorMessage = 
                "Alternate Company Point of Contact(POC) 3: Last Name: Required Field";

            // Act / Assert
            var result = validator.ShouldHaveValidationErrorFor(x => x.ContractPOCAlternatePocLastname3, ciw, "ValidPOCRow4");
            var errorMessage = result.FirstOrDefault(x => x.ErrorMessage == expectedErrorMessage);
            Assert.IsNotNull(errorMessage);
        }

        [TestMethod]
        public void Validation_Fails_When_ContractPOCAlternatePocLastname3_Is_Invalid()
        {
            // Arrange
            validator = new ContractorValidator();

            var ciw = new CIW()
            {
                ContractPOCAlternatePocFirstname3 = validFirstName,
                ContractPOCAlternatePocLastname3 = InvalidLastName,
                ContractPOCAlternatePocPhoneWork3 = ValidPhoneNumber,
                ContractPOCAlternatePocEmail3 = ValidEmailAddress
            };

            const string expectedErrorMessage = 
                "Alternate Company Point of Contact(POC) 3: Last Name: Contains Invalid Characters";

            // Act / Assert
            var result = validator.ShouldHaveValidationErrorFor(x => x.ContractPOCAlternatePocLastname3, ciw, "ValidPOCRow4");
            var errorMessage = result.FirstOrDefault(x => x.ErrorMessage == expectedErrorMessage);
            Assert.IsNotNull(errorMessage); ;
        }

        [TestMethod]
        public void Validation_Fails_When_ContractPOCAlternatePocPhoneWork3_Is_Invalid()
        {
            // Arrange
            validator = new ContractorValidator();

            var ciw = new CIW()
            {
                ContractPOCAlternatePocFirstname3 = validFirstName,
                ContractPOCAlternatePocLastname3 = validLastName,
                ContractPOCAlternatePocPhoneWork3 = InvalidPhoneNumber,
                ContractPOCAlternatePocEmail3 = ValidEmailAddress
            };

            const string expectedErrorMessage =
                "Alternate Company Point of Contact(POC) 3: Work Phone Number: Invalid Phone Number";

            // Act / Assert
            var result = validator.ShouldHaveValidationErrorFor(x => x.ContractPOCAlternatePocPhoneWork3, ciw, "ValidPOCRow4");
            var errorMessage = result.FirstOrDefault(x => x.ErrorMessage == expectedErrorMessage);
            Assert.IsNotNull(errorMessage);
        }

        [TestMethod]
        public void Validation_Fails_When_ContractPOCAlternatePocPhoneWork3_Is_Empty()
        {
            // Arrange
            validator = new ContractorValidator();

            var ciw = new CIW()
            {
                ContractPOCAlternatePocFirstname3 = validFirstName,
                ContractPOCAlternatePocLastname3 = validLastName,
                ContractPOCAlternatePocPhoneWork3 = string.Empty,
                ContractPOCAlternatePocEmail3 = ValidEmailAddress
            };

            const string expectedErrorMessage = 
                "Alternate Company Point of Contact(POC) 3: Work Phone Number: Required Field";

            // Act / Assert
            var result = validator.ShouldHaveValidationErrorFor(x => x.ContractPOCAlternatePocPhoneWork3, ciw, "ValidPOCRow4");
            var errorMessage = result.FirstOrDefault(x => x.ErrorMessage == expectedErrorMessage);
            Assert.IsNotNull(errorMessage);
        }

        [TestMethod]
        public void Validation_Fails_When_ContractPOCAlternatePocEmail3_Is_Invalid()
        {
            // Arrange
            validator = new ContractorValidator();

            var ciw = new CIW()
            {
                ContractPOCAlternatePocFirstname3 = validFirstName,
                ContractPOCAlternatePocLastname3 = validLastName,
                ContractPOCAlternatePocPhoneWork3 = ValidPhoneNumber,
                ContractPOCAlternatePocEmail3 = InvalidEmailAddress
            };

            const string expectedErrorMessage =
                "Alternate Company Point of Contact(POC) 3: E-Mail Address: Invalid E-Mail Address";

            // Act / Assert
            var result = validator.ShouldHaveValidationErrorFor(x => x.ContractPOCAlternatePocEmail3, ciw, "ValidPOCRow4");
            var errorMessage = result.FirstOrDefault(x => x.ErrorMessage == expectedErrorMessage);
            Assert.IsNotNull(errorMessage);
        }

        [TestMethod]
        public void Validation_Passes_When_ContractPOCAlternate3_Fields_Are_Valid()
        {
            // Arrange
            validator = new ContractorValidator();

            const string ruleSetName = "ValidPOCRow4";

            var ciw = new CIW()
            {
                ContractPOCAlternatePocFirstname3 = validFirstName,
                ContractPOCAlternatePocLastname3 = validLastName,
                ContractPOCAlternatePocPhoneWork3 = ValidPhoneNumber,
                ContractPOCAlternatePocEmail3 = ValidEmailAddress
            };

            // Act / Assert
            validator.ShouldNotHaveValidationErrorFor(x => x.ContractPOCAlternatePocFirstname3, ciw, ruleSetName);
            validator.ShouldNotHaveValidationErrorFor(x => x.ContractPOCAlternatePocLastname3, ciw, ruleSetName);
            validator.ShouldNotHaveValidationErrorFor(x => x.ContractPOCAlternatePocPhoneWork3, ciw, ruleSetName);
            validator.ShouldNotHaveValidationErrorFor(x => x.ContractPOCAlternatePocEmail3, ciw, ruleSetName);
        }

        #endregion

        #region ValidPOCRow5 RuleSet Tests

        [TestMethod]
        public void Validation_Fails_When_ContractPOCAlternatePocFirstname4_Exceeds_CharacterLength()
        {
            // Arrange
            validator = new ContractorValidator();

            var ciw = new CIW()
            {
                ContractPOCAlternatePocFirstname4 = invalidLengthName,
                ContractPOCAlternatePocLastname4 = validLastName,
                ContractPOCAlternatePocPhoneWork4 = ValidPhoneNumber,
                ContractPOCAlternatePocEmail4 = ValidEmailAddress
            };

            const string
                expectedErrorMessage = "Alternate Company Point of Contact(POC) 4: First Name: exceeds maximum number of characters. Please double-check the field. If value is correct, please reach out to HSPD-12 Security at HSPD12.Security@gsa.gov or at +1 (202) 501-4459.";

            // Act / Assert
            var result = validator.ShouldHaveValidationErrorFor(x => x.ContractPOCAlternatePocFirstname4, ciw, "ValidPOCRow5");
            var errorMessage = result.FirstOrDefault(x => x.ErrorMessage == expectedErrorMessage);
            Assert.IsNotNull(errorMessage); ;
        }

        [TestMethod]
        public void Validation_Fails_When_ContractPOCAlternatePocFirstname4_Is_Empty()
        {
            // Arrange
            validator = new ContractorValidator();

            var ciw = new CIW()
            {
                ContractPOCAlternatePocFirstname4 = string.Empty,
                ContractPOCAlternatePocLastname4 = validLastName,
                ContractPOCAlternatePocPhoneWork4 = ValidPhoneNumber,
                ContractPOCAlternatePocEmail4 = ValidEmailAddress
            };

            const string expectedErrorMessage =
                "Alternate Company Point of Contact(POC) 4: First Name: Required Field";

            // Act / Assert
            var result = validator.ShouldHaveValidationErrorFor(x => x.ContractPOCAlternatePocFirstname4, ciw, "ValidPOCRow5");
            var errorMessage = result.FirstOrDefault(x => x.ErrorMessage == expectedErrorMessage);
            Assert.IsNotNull(errorMessage); ;
        }

        [TestMethod]
        public void Validation_Fails_When_ContractPOCAlternatePocFirstname4_Is_Invalid()
        {
            // Arrange
            validator = new ContractorValidator();

            var ciw = new CIW()
            {
                ContractPOCAlternatePocFirstname4 = InvalidFirstName,
                ContractPOCAlternatePocLastname4 = validLastName,
                ContractPOCAlternatePocPhoneWork4 = ValidPhoneNumber,
                ContractPOCAlternatePocEmail4 = ValidEmailAddress
            };

            const string expectedErrorMessage =
                "Alternate Company Point of Contact(POC) 4: First Name: Contains Invalid Characters";

            // Act / Assert
            var result = validator.ShouldHaveValidationErrorFor(x => x.ContractPOCAlternatePocFirstname4, ciw, "ValidPOCRow5");
            var errorMessage = result.FirstOrDefault(x => x.ErrorMessage == expectedErrorMessage);
            Assert.IsNotNull(errorMessage); ;
        }

        [TestMethod]
        public void Validation_Fails_When_ContractPOCAlternatePocLastname4_Exceeds_CharacterLength()
        {
            // Arrange
            validator = new ContractorValidator();

            var ciw = new CIW()
            {
                ContractPOCAlternatePocFirstname4 = validFirstName,
                ContractPOCAlternatePocLastname4 = invalidLengthName,
                ContractPOCAlternatePocPhoneWork4 = ValidPhoneNumber,
                ContractPOCAlternatePocEmail4 = ValidEmailAddress
            };

            const string expectedErrorMessage = 
                "Alternate Company Point of Contact(POC) 4: Last Name: exceeds maximum number of characters. Please double-check the field. If value is correct, please reach out to HSPD-12 Security at HSPD12.Security@gsa.gov or at +1 (202) 501-4459.";

            // Act / Assert
            var result = validator.ShouldHaveValidationErrorFor(x => x.ContractPOCAlternatePocLastname4, ciw, "ValidPOCRow5");
            var errorMessage = result.FirstOrDefault(x => x.ErrorMessage == expectedErrorMessage);
            Assert.IsNotNull(errorMessage);
        }

        [TestMethod]
        public void Validation_Fails_When_ContractPOCAlternatePocLastname4_Is_Empty()
        {
            // Arrange
            validator = new ContractorValidator();

            var ciw = new CIW()
            {
                ContractPOCAlternatePocFirstname4 = validFirstName,
                ContractPOCAlternatePocLastname4 = string.Empty,
                ContractPOCAlternatePocPhoneWork4 = ValidPhoneNumber,
                ContractPOCAlternatePocEmail4 = ValidEmailAddress
            };

            const string expectedErrorMessage = 
                "Alternate Company Point of Contact(POC) 4: Last Name: Required Field";

            // Act / Assert
            var result = validator.ShouldHaveValidationErrorFor(x => x.ContractPOCAlternatePocLastname4, ciw, "ValidPOCRow5");
            var errorMessage = result.FirstOrDefault(x => x.ErrorMessage == expectedErrorMessage);
            Assert.IsNotNull(errorMessage);
        }

        [TestMethod]
        public void Validation_Fails_When_ContractPOCAlternatePocLastname4_Is_Invalid()
        {
            // Arrange
            validator = new ContractorValidator();

            var ciw = new CIW()
            {
                ContractPOCAlternatePocFirstname4 = validFirstName,
                ContractPOCAlternatePocLastname4 = InvalidLastName,
                ContractPOCAlternatePocPhoneWork4 = ValidPhoneNumber,
                ContractPOCAlternatePocEmail4 = ValidEmailAddress
            };

            const string expectedErrorMessage = 
                "Alternate Company Point of Contact(POC) 4: Last Name: Contains Invalid Characters";

            // Act / Assert
            var result = validator.ShouldHaveValidationErrorFor(x => x.ContractPOCAlternatePocLastname4, ciw, "ValidPOCRow5");
            var errorMessage = result.FirstOrDefault(x => x.ErrorMessage == expectedErrorMessage);
            Assert.IsNotNull(errorMessage); ;
        }

        [TestMethod]
        public void Validation_Fails_When_ContractPOCAlternatePocPhoneWork4_Is_Invalid()
        {
            // Arrange
            validator = new ContractorValidator();

            var ciw = new CIW()
            {
                ContractPOCAlternatePocFirstname4 = validFirstName,
                ContractPOCAlternatePocLastname4 = validLastName,
                ContractPOCAlternatePocPhoneWork4 = InvalidPhoneNumber,
                ContractPOCAlternatePocEmail4 = ValidEmailAddress
            };

            const string expectedErrorMessage = 
                "Alternate Company Point of Contact(POC) 4: Work Phone Number: Invalid Phone Number";

            // Act / Assert
            var result = validator.ShouldHaveValidationErrorFor(x => x.ContractPOCAlternatePocPhoneWork4, ciw, "ValidPOCRow5");
            var errorMessage = result.FirstOrDefault(x => x.ErrorMessage == expectedErrorMessage);
            Assert.IsNotNull(errorMessage);
        }

        [TestMethod]
        public void Validation_Fails_When_ContractPOCAlternatePocPhoneWork4_Is_Empty()
        {
            // Arrange
            validator = new ContractorValidator();

            var ciw = new CIW()
            {
                ContractPOCAlternatePocFirstname4 = validFirstName,
                ContractPOCAlternatePocLastname4 = validLastName,
                ContractPOCAlternatePocPhoneWork4 = string.Empty,
                ContractPOCAlternatePocEmail4 = ValidEmailAddress
            };

            var expectedErrorMessage = 
                "Alternate Company Point of Contact(POC) 4: Work Phone Number: Required Field";

            // Act / Assert
            var result = validator.ShouldHaveValidationErrorFor(x => x.ContractPOCAlternatePocPhoneWork4, ciw, "ValidPOCRow5");
            var errorMessage = result.FirstOrDefault(x => x.ErrorMessage == expectedErrorMessage);
            Assert.IsNotNull(errorMessage);
        }

        [TestMethod]
        public void Validation_Fails_When_ContractPOCAlternatePocEmail4_Is_Invalid()
        {
            // Arrange
            validator = new ContractorValidator();

            var ciw = new CIW()
            {
                ContractPOCAlternatePocFirstname4 = validFirstName,
                ContractPOCAlternatePocLastname4 = validLastName,
                ContractPOCAlternatePocPhoneWork4 = ValidPhoneNumber,
                ContractPOCAlternatePocEmail4 = InvalidEmailAddress
            };

            var expectedErrorMessage = 
                "Alternate Company Point of Contact(POC) 4: E-Mail Address: Invalid E-Mail Address";

            // Act / Assert
            var result = validator.ShouldHaveValidationErrorFor(x => x.ContractPOCAlternatePocEmail4, ciw, "ValidPOCRow5");
            var errorMessage = result.FirstOrDefault(x => x.ErrorMessage == expectedErrorMessage);
            Assert.IsNotNull(errorMessage);
        }

        [TestMethod]
        public void Validation_Passes_When_ContractPOCAlternate4_Fields_Are_Valid()
        {
            // Arrange
            validator = new ContractorValidator();

            const string ruleSetName = "ValidPOCRow5";

            var ciw = new CIW()
            {
                ContractPOCAlternatePocFirstname4 = validFirstName,
                ContractPOCAlternatePocLastname4 = validLastName,
                ContractPOCAlternatePocPhoneWork4 = ValidPhoneNumber,
                ContractPOCAlternatePocEmail4 = ValidEmailAddress
            };

            // Act / Assert
            validator.ShouldNotHaveValidationErrorFor(x => x.ContractPOCAlternatePocFirstname4, ciw, ruleSetName);
            validator.ShouldNotHaveValidationErrorFor(x => x.ContractPOCAlternatePocLastname4, ciw, ruleSetName);
            validator.ShouldNotHaveValidationErrorFor(x => x.ContractPOCAlternatePocPhoneWork4, ciw, ruleSetName);
            validator.ShouldNotHaveValidationErrorFor(x => x.ContractPOCAlternatePocEmail4, ciw, ruleSetName);
        }

        #endregion
    }
}