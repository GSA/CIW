using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ProcessCIW.Models
{
    

    /// <summary>
    /// The CIW class is the main data object of ProcessCIW. After the CIW is extracted into a csv, this data is then mapped to the CIW object.
    /// </summary>
    class CIW
    {
        delegate string del(string s);
        del TrimPhone = new del(Utilities.Utilities.TrimPhoneNum);

        //List of backing fields
        private string _PhoneNumberWork;
        private string _PhoneNumberWorkCell;
        private string _ContractPOCPhoneWork;
        private string _ContractPOCAlternatePocPhoneWork1;
        private string _ContractPOCAlternatePocPhoneWork2;
        private string _ContractPOCAlternatePocPhoneWork3;
        private string _ContractPOCAlternatePocPhoneWork4;
        private string _SponsorPhoneWork;
        private string _SponsorAlternatePhoneWork1;
        private string _SponsorAlternatePhoneWork2;
        private string _SponsorAlternatePhoneWork3;
        private string _SponsorAlternatePhoneWork4;
        private string _VersionNumber;
        private string _Suffix;
        private string _ArraLongTermContractor;
        private string _PersonalEmailAddress;
        private string _ContractPOCEMailAddress;
        private string _ContractPOCAlternatePocEmail1;
        private string _ContractPOCAlternatePocEmail2;
        private string _ContractPOCAlternatePocEmail3;
        private string _ContractPOCAlternatePocEmail4;
        private string _SponsorEmailAddress;
        private string _SponsorAlternateEmailAddress1;
        private string _SponsorAlternateEmailAddress2;
        private string _SponsorAlternateEmailAddress3;
        private string _SponsorAlternateEmailAddress4;

        //_ApproximiateInvestigationDate is the pers_prior_investigation_date
        private string _ApproximiateInvestigationDate;

        /// <summary>
        /// Section 1
        /// </summary>


        //Assigns "V0" to version if version is null
        public string VersionNumber
        {
            get { return (_VersionNumber == null) ? "V0" : _VersionNumber; }
            set { _VersionNumber = value; }
        }

        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        //If suffix value is "N/A", stores an empty string
        public string Suffix
        {
            get { return _Suffix; }
            set { _Suffix = (value == "N/A") ? "" : value; }
        }

        public string Sex { get; set; } 
        public string SocialSecurityNumber { get; set; }
        public string DateOfBirth { get; set; }
        public string PlaceOfBirthCity { get; set; }
        public string PlaceOfBirthCountry { get; set; }
        public string PlaceOfBirthState { get; set; }
        public string PlaceOfBirthMexicoCanada { get; set; }
        public string HomeAddressOne { get; set; }
        public string HomeAddressTwo { get; set; }
        public string HomeAddressCity { get; set; }
        public string HomeAddressCountry { get; set; }
        public string HomeAddressUSState { get; set; }
        public string HomeAddressMexicoStateCanadaProvince { get; set; }
        public string HomeAddressZip { get; set; }

        //Removes all non digit characters from PhoneNumberWork
        public string PhoneNumberWork
        {
            get
            { return _PhoneNumberWork; }
            set
            { _PhoneNumberWork = TrimPhone(value); }
        }

        //Removes all non digit characters from PhoneNumberCell
        public string PhoneNumberWorkCell
        {
            get
            { return _PhoneNumberWorkCell; }
            set
            { _PhoneNumberWorkCell = TrimPhone(value); }
        }

        //Stores PersonalEmailAddress as lower case
        public string PersonalEmailAddress
        {
            get
            { return _PersonalEmailAddress; }
            set
            { _PersonalEmailAddress = value.ToLower(); }
        }

        public string PositionJobTitle { get; set; }
        public string PriorInvestigation { get; set; }

        //_ApproximiateInvestigationDate is the pers_prior_investigation_date
        public string ApproximiateInvestigationDate
        {
            get { return _ApproximiateInvestigationDate; }
            set { _ApproximiateInvestigationDate = value.Trim(); }
        }
        public string AgencyAdjudicatedPriorInvestigation { get; set; }
        public string Citizen { get; set; }
        public string PortOfEntryUSCityAndState { get; set; }
        public string DateOfEntry { get; set; }
        public string LessThanThreeYearsResident { get; set; }
        public string AlienRegistrationNumber { get; set; } 
        public string CitzenshipCountry { get; set; }

        /// <summary>
        /// Section 2
        /// </summary>
        
        public string CompanyName { get; set; }
        public string CompanyNameSub { get; set; }
        public string DataUniversalNumberingSystem { get; set; }
        public string TaskOrderDeliveryOrder { get; set; }
        public string ContractNumberType { get; set; } 
        public string ContractStartDate { get; set; }
        public string ContractEndDate { get; set; }
        public string HasOptionYears { get; set; }
        public string NumberOfOptionYears { get; set; }
        public string ContractPOCFirstName { get; set; }
        public string ContractPOCLastName { get; set; }

        //Removes all non digit characters from ContractPOCPhoneWork
        public string ContractPOCPhoneWork
        {
            get
            { return _ContractPOCPhoneWork; }
            set
            { _ContractPOCPhoneWork = TrimPhone(value); }
        }

        //Stores ContractPOCEMailAddress as lower case
        public string ContractPOCEMailAddress
        {
            get
            { return _ContractPOCEMailAddress; }
            set
            { _ContractPOCEMailAddress = value.ToLower(); }
        }

        public string ContractPOCAlternatePocFirstname1 { get; set; }
        public string ContractPOCAlternatePocLastname1 { get; set; }

        //Removes all non digit characters from ContractPOCAlternatePocPhoneWork1
        public string ContractPOCAlternatePocPhoneWork1
        {
            get
            { return _ContractPOCAlternatePocPhoneWork1; }
            set
            { _ContractPOCAlternatePocPhoneWork1 = TrimPhone(value); }
        }

        //Stores ContractPOCAlternatePocEmail1 as lower case
        public string ContractPOCAlternatePocEmail1
        {
            get
            { return _ContractPOCAlternatePocEmail1; }
            set
            { _ContractPOCAlternatePocEmail1 = value.ToLower(); }
        }

        public string ContractPOCAlternatePocFirstname2 { get; set; }
        public string ContractPOCAlternatePocLastname2 { get; set; }

        //Removes all non digit characters from ContractPOCAlternatePocPhoneWork2
        public string ContractPOCAlternatePocPhoneWork2
        {
            get
            { return _ContractPOCAlternatePocPhoneWork2; }
            set
            { _ContractPOCAlternatePocPhoneWork2 = TrimPhone(value); }
        }

        //Stores ContractPOCAlternatePocEmail2 as lower case
        public string ContractPOCAlternatePocEmail2
        {
            get
            { return _ContractPOCAlternatePocEmail2; }
            set
            { _ContractPOCAlternatePocEmail2 = value.ToLower(); }
        }

        public string ContractPOCAlternatePocFirstname3 { get; set; }
        public string ContractPOCAlternatePocLastname3 { get; set; }

        //Removes all non digit characters from ContractPOCAlternatePocPhoneWork3
        public string ContractPOCAlternatePocPhoneWork3
        {
            get
            { return _ContractPOCAlternatePocPhoneWork3; }
            set
            { _ContractPOCAlternatePocPhoneWork3 = TrimPhone(value); }
        }

        //Stores ContractPOCAlternatePocEmail3 as lower case
        public string ContractPOCAlternatePocEmail3
        {
            get
            { return _ContractPOCAlternatePocEmail3; }
            set
            { _ContractPOCAlternatePocEmail3 = value.ToLower(); }
        }

        public string ContractPOCAlternatePocFirstname4 { get; set; }
        public string ContractPOCAlternatePocLastname4 { get; set; }

        //Removes all non digit characters from ContractPOCAlternatePocPhoneWork4
        public string ContractPOCAlternatePocPhoneWork4
        {
            get
            { return _ContractPOCAlternatePocPhoneWork4; }
            set
            { _ContractPOCAlternatePocPhoneWork4 = TrimPhone(value); }
        }

        //Stores ContractPOCAlternatePocEmail4 as lower case
        public string ContractPOCAlternatePocEmail4
        {
            get
            { return _ContractPOCAlternatePocEmail4; }
            set
            { _ContractPOCAlternatePocEmail4 = value.ToLower(); }
        }

        /// <summary>
        /// Section 3
        /// </summary>

        public string RWAIAANumber { get; set; }
        public string RWAIAAAgency { get; set; }

        /// <summary>
        /// Section 4
        /// </summary>

        public string BuildingNumber { get; set; }
        public string Other { get; set; }
        public string ContractorType { get; set; }

        //Stores ArraLongTermContractor as empty string if value is "Not Applicable"
        public string ArraLongTermContractor
        {
            get { return _ArraLongTermContractor; }
            set { _ArraLongTermContractor = (value == "Not Applicable") ? "" : value; }
        }

        public string SponsoringMajorOrg { get; set; }
        public string SponsoringOfficeSymbol { get; set; }
        public string Region { get; set; }

        /// <summary>
        /// Section 5
        /// </summary>

        public string InvestigationTypeRequested { get; set; }
        public string AccessCardRequired { get; set; }

        /// <summary>
        /// Section 6
        /// </summary>

        public string SponsorFirstName { get; set; }
        public string SponsorMiddleName { get; set; }
        public string SponsorLastName { get; set; }

        //Stores SponsorEmailAddress as lower case
        public string SponsorEmailAddress
        {
            get
            { return _SponsorEmailAddress; }
            set
            { _SponsorEmailAddress = value.ToLower(); }
        }

        //Removes all non digit characters from SponsorPhoneWork
        public string SponsorPhoneWork
        {
            get
            { return _SponsorPhoneWork; }
            set
            { _SponsorPhoneWork = TrimPhone(value); }
        }

        public string SponsorIsPMCORCO { get; set; }
        public string SponsorAlternateFirstName1 { get; set; }
        public string SponsorAlternateMiddleName1 { get; set; }
        public string SponsorAlternateLastName1 { get; set; }

        //Stores SponsorAlternateEmailAddress1 as lower case
        public string SponsorAlternateEmailAddress1
        {
            get
            { return _SponsorAlternateEmailAddress1; }
            set
            { _SponsorAlternateEmailAddress1 = value.ToLower(); }
        }

        //Removes all non digit characters from SponsorAlternatePhoneWork1
        public string SponsorAlternatePhoneWork1
        {
            get
            { return _SponsorAlternatePhoneWork1; }
            set
            { _SponsorAlternatePhoneWork1 = TrimPhone(value); }
        }

        public string SponsorAlternateIsPMCORCO1 { get; set; }
        public string SponsorAlternateFirstName2 { get; set; }
        public string SponsorAlternateMiddleName2 { get; set; }
        public string SponsorAlternateLastName2 { get; set; }

        //Stores SponsorAlternateEmailAddress2 as lower case
        public string SponsorAlternateEmailAddress2
        {
            get
            { return _SponsorAlternateEmailAddress2; }
            set
            { _SponsorAlternateEmailAddress2 = value.ToLower(); }
        }

        //Removes all non digit characters from SponsorAlternatePhoneWork2
        public string SponsorAlternatePhoneWork2
        {
            get
            { return _SponsorAlternatePhoneWork2; }
            set
            { _SponsorAlternatePhoneWork2 = TrimPhone(value); }
        }

        public string SponsorAlternateIsPMCORCO2 { get; set; }
        public string SponsorAlternateFirstName3 { get; set; }
        public string SponsorAlternateMiddleName3 { get; set; }
        public string SponsorAlternateLastName3 { get; set; }

        //Stores SponsorAlternateEmailAddress3 as lower case
        public string SponsorAlternateEmailAddress3
        {
            get
            { return _SponsorAlternateEmailAddress3; }
            set
            { _SponsorAlternateEmailAddress3 = value.ToLower(); }
        }

        //Removes all non digit characters from SponsorAlternatePhoneWork3
        public string SponsorAlternatePhoneWork3
        {
            get
            { return _SponsorAlternatePhoneWork3; }
            set
            { _SponsorAlternatePhoneWork3 = TrimPhone(value); }
        }

        public string SponsorAlternateIsPMCORCO3 { get; set; }
        public string SponsorAlternateFirstName4 { get; set; }
        public string SponsorAlternateMiddleName4 { get; set; }
        public string SponsorAlternateLastName4 { get; set; }

        //Stores SponsorAlternateEmailAddress4 as lower case
        public string SponsorAlternateEmailAddress4
        {
            get
            { return _SponsorAlternateEmailAddress4; }
            set
            { _SponsorAlternateEmailAddress4 = value.ToLower(); }
        }

        //Removes all non digit characters from SponsorAlternatePhoneWork4
        public string SponsorAlternatePhoneWork4
        {
            get
            { return _SponsorAlternatePhoneWork4; }
            set
            { _SponsorAlternatePhoneWork4 = TrimPhone(value); }
        }

        public string SponsorAlternateIsPMCORCO4 { get; set; }

        //List of VendorPOC's to iterate through, checking for non empty lines
        public List<POC.VendorPOC> VendorPOC { get; set; }

        //List of GSAPOC's to iterate  through, checking for non empty lines
        public List<POC.GSAPOC> GSAPOC { get; set; }

        
        /// <summary>
        /// Property utilizing stringbuilder to generate a name string pre-pended with "Invalid CIW - " for email purposes
        /// </summary>
        //public string FullName                    *** no references ***
        //{
        //    get
        //    {
        //        StringBuilder fullName = new StringBuilder();

        //        fullName.Append("Invalid CIW - ");
        //        fullName.Append(LastName);
        //        fullName.Append(" ");

        //        if (!Suffix.Equals("N/A"))
        //        {
        //            fullName.Append(Suffix);
        //        }

        //        fullName.Append(",");
        //        fullName.Append(" ");
        //        fullName.Append(FirstName);
        //        fullName.Append(" ");

        //        if (!MiddleName.Equals("NMN"))
        //            fullName.Append(MiddleName);

        //        return fullName.ToString();
        //    }
        //}

        /// <summary>
        /// Property utilizing StringBuilder to genererate a name field to be used in logging
        /// </summary>
        public string FullNameForLog
        {
            get
            {
                StringBuilder fullName = new StringBuilder();

                fullName.Append(FirstName == null ? "FirstName is NULL" : FirstName == "" ? "FirstName is EMPTY" : FirstName);
                fullName.Append(" ");
                fullName.Append(LastName == null ? "LastName is NULL" : LastName == "" ? "LastName is EMPTY" : LastName);

                return fullName.ToString();
            }
        }
    }
}