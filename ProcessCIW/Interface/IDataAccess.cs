using ProcessCIW.Models;
using System.Collections.Generic;
using System.Data;

namespace ProcessCIW.Interface
{
    interface IDataAccess
    {
        bool BeAValidEMail(string workEMail);
        bool BeAValidBuilding(string buildingID);
        bool ValidateState(string state, string country);
        bool NotBeADuplicateSSN(string ssn);
        bool NotBeADuplicateUser(string lastName, string dob, string ssn);
        void UpdateProcessed(int documentID, int processedResult);
        List<UnprocessedFiles> GetUnprocessedFiles();
        DataSet GetFipsCodeFromCountryName(string placeOfBirthCountryName, string homeCountryName, string citizenshipCountryName);
        UploaderInformation GetUploaderInformation(int uploaderID);


    }
}
