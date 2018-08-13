using Moq;
using ProcessCIW.Interface;
using FluentValidation.Results;
using System.Collections.Generic;
using ProcessCIW.Utilities;
using ProcessCIW.Models;
using CsvHelper.Configuration;
using ProcessCIW.Mapping;

namespace ProcessCIW.Test.dll
{
    public class MockTool
    {
        public Mock<IValidateCiw> createValidationMock()
        {
            var mock = new Mock<IValidateCiw>();
            mock.Setup(a => a.GetErrors()).Returns(
                new System.Tuple<
                    ValidationResult,
                    ValidationResult,
                    ValidationResult,
                    ValidationResult,
                    ValidationResult,
                    ValidationResult>(
                        new ValidationResult(
                            new List<ValidationFailure>(
                                new ValidationFailure[] {
                                    new ValidationFailure(
                                        "prop 1",
                                        "prop 1 message"
                                    )
                                }
                            )
                        ),
                        new ValidationResult(
                            new List<ValidationFailure>(
                                new ValidationFailure[] {
                                    new ValidationFailure(
                                        "prop 2",
                                        "prop 2 message"
                                    )
                                }
                            )
                        ),
                        new ValidationResult(
                            new List<ValidationFailure>(
                                new ValidationFailure[] {
                                    new ValidationFailure(
                                        "prop 3",
                                        "prop 3 message"
                                    )
                                }
                            )
                        ),
                        new ValidationResult(
                            new List<ValidationFailure>(
                                new ValidationFailure[] {
                                    new ValidationFailure(
                                        "prop 4",
                                        "prop 4 message"
                                    )
                                }
                            )
                        ),
                        new ValidationResult(
                            new List<ValidationFailure>(
                                new ValidationFailure[] {
                                    new ValidationFailure(
                                        "prop 5",
                                        "prop 5 message"
                                    )
                                }
                            )
                        ),
                        new ValidationResult(
                            new List<ValidationFailure>(
                                new ValidationFailure[] {
                                    new ValidationFailure(
                                        "prop 6",
                                        "prop 6 message"
                                    )
                                }
                            )
                        )
                    )
                );

            mock.Setup(a => a.IsDuplicate(It.IsAny<List<CIW>>())).Returns(false);
            mock.Setup(a => a.IsFormValid(It.IsAny<List<CIW>>())).Returns(true);
            mock.Setup(a => a.PrintErrors());

            return mock;
        }

        public Mock<ICiwEmails> createCiwEmailMock()
        {
            var mock = new Mock<ICiwEmails>();
            mock.Setup(a => a.SendARRA());
            mock.Setup(a => a.SendDuplicateUser());
            mock.Setup(a => a.SendErrors(
                It.IsAny<ValidationResult>(),
                It.IsAny<ValidationResult>(),
                It.IsAny<ValidationResult>(),
                It.IsAny<ValidationResult>(),
                It.IsAny<ValidationResult>(),
                It.IsAny<ValidationResult>()
                ));
            mock.Setup(a => a.SendPasswordProtection());
            mock.Setup(a => a.SendSponsorshipEMail(It.IsAny<int>()));
            mock.Setup(a => a.SendWrongVersion());
            mock.Setup(a => a.Setup(
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
                ));
            return mock;
        }

        public Mock<IDataAccess> createDataAccessMock()
        {
            Mock<IDataAccess> mock = new Mock<IDataAccess>();
            mock.Setup(a => a.BeAValidBuilding(It.IsAny<string>())).Returns(true);
            mock.Setup(a => a.BeAValidEMail(It.IsAny<string>())).Returns(true);
            mock.Setup(a => a.GetFipsCodeFromCountryName(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new System.Data.DataSet());
            mock.Setup(a => a.GetUnprocessedFiles()).Returns(new List<Models.UnprocessedFiles>());
            mock.Setup(a => a.GetUploaderInformation(It.IsAny<int>())).Returns(new UploaderInformation());
            mock.Setup(a => a.InsertCIW(It.IsAny<CIW>(), It.IsAny<int>())).Returns(1);
            mock.Setup(a => a.NotBeADuplicateSSN(It.IsAny<string>())).Returns(true);
            mock.Setup(a => a.NotBeADuplicateUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            mock.Setup(a => a.UpdateProcessed(It.IsAny<int>(), It.IsAny<int>()));
            mock.Setup(a => a.ValidateState(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            return mock;
        }

        public Mock<IDecryptFile> createDecryptFileMock()
        {
            Mock<IDecryptFile> mock = new Mock<IDecryptFile>();
            mock.Setup(a => a.Decrypt(It.IsAny<string>(), It.IsAny<UnprocessedFiles>())).Returns("decrypted file path");

            return mock;
        }

        public Mock<IDeleteTool> createDeleteMock()
        {
            Mock<IDeleteTool> mock = new Mock<IDeleteTool>();
            mock.Setup(a => a.DeleteFiles(It.IsAny<List<string>>()));
            mock.Setup(a => a.deleteOldCsvFiles());
            mock.Setup(a => a.deleteTempCsvFile(It.IsAny<string>()));

            return mock;
        }

        public Mock<IFileTool> createFileToolMock()
        {
            Mock<IFileTool> mock = new Mock<IFileTool>();
            mock.Setup(a => a.CreateTempFile(It.IsAny<List<CIWData>>())).Returns("Path");
            mock.Setup(a => a.GetFileData<CIW, CIWMapping>(It.IsAny<string>(), It.IsAny<CsvConfiguration>())).Returns(new List<CIW>());

            return mock;
        }

        public Mock<ILogTool> createLogMock()
        {
            var mock = new Mock<ILogTool>();
            mock.Setup(a => a.Info(It.IsAny<object>()));
            mock.Setup(a => a.Error(It.IsAny<object>()));
            mock.Setup(a => a.Fatal(It.IsAny<object>()));
            mock.Setup(a => a.Warn(It.IsAny<object>()));

            return mock;
        }



        public Mock<IUtilities> createUtilMock()
        {
            var mock = new Mock<IUtilities>();
            mock.Setup(a => a.BeAValidBirthDate(It.IsAny<string>())).Returns(true);
            mock.Setup(a => a.BeAValidDate(It.IsAny<string>())).Returns(true);
            mock.Setup(a => a.BeAValidEndDate(It.IsAny<string>(), It.IsAny<int>())).Returns(true);
            mock.Setup(a => a.CleanSsn(It.IsAny<string>())).Returns("1234567890");
            mock.Setup(a => a.DateIsValidAndNotFuture(It.IsAny<string>())).Returns(true);
            mock.Setup(a => a.EndIsFutureDate(It.IsAny<string>())).Returns(false);
            mock.Setup(a => a.fileNameHelper(It.IsAny<string>())).Returns("returnedFileName");
            mock.Setup(a => a.FormatDate(It.IsAny<string>())).Returns("1900-0-01");
            mock.Setup(a => a.GenerateDecryptedFilename(It.IsAny<string>()));
            mock.Setup(a => a.HashSSN(It.IsAny<string>())).Returns(new byte[] { 0x0001, 0x00a2, 0x00ec, 0x0039 });
            mock.Setup(a => a.IsNotWhiteSpace(It.IsAny<string>())).Returns(true);
            mock.Setup(a => a.StartBeforeEnd(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            mock.Setup(a => a.TrimPhoneNum(It.IsAny<string>())).Returns("5555555555");
            mock.Setup(a => a.TrimPoundSign(It.IsAny<string>())).Returns("5555555555");

            return mock;
        }

        public Mock<IXmlTool> createXmlToolMock()
        {
            Mock<IXmlTool> mock = new Mock<IXmlTool>();
            mock.Setup(a => a.isPasswordProtected(It.IsAny<string>())).Returns(true);
            int x = 0;
            mock.Setup(a => a.parseCiwDocument(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), out x)).Returns(new List<CIWData>());

            return mock;
        }

    }
}
