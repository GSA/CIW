using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ProcessCIW.Models
{
    class CIW
    {
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

        //Section 1
        public string VersionNumber
        {
            get { return (_VersionNumber == null) ? "V0" : _VersionNumber; }
            set { _VersionNumber = value; }
        }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
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
        public string PhoneNumberWork
        {
            get
            { return _PhoneNumberWork; }
            set
            { _PhoneNumberWork = Regex.Replace(value, "[^0-9]", ""); }
        }
        public string PhoneNumberWorkCell
        {
            get
            { return _PhoneNumberWorkCell; }
            set
            { _PhoneNumberWorkCell = Regex.Replace(value, "[^0-9]", ""); }
        }
        public string PersonalEmailAddress
        {
            get
            { return _PersonalEmailAddress; }
            set
            { _PersonalEmailAddress = value.ToLower(); }
        }
        public string PositionJobTitle { get; set; }
        public string PriorInvestigation { get; set; }
        public string ApproximiateInvestigationDate { get; set; }
        public string AgencyAdjudicatedPriorInvestigation { get; set; }
        public string Citizen { get; set; }
        public string PortOfEntryUSCityAndState { get; set; }
        public string DateOfEntry { get; set; }
        public string LessThanThreeYearsResident { get; set; }
        public string AlienRegistrationNumber { get; set; } 
        public string CitzenshipCountry { get; set; }

        //Section 2
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
        public string ContractPOCPhoneWork
        {
            get
            { return _ContractPOCPhoneWork; }
            set
            { _ContractPOCPhoneWork = Regex.Replace(value, "[^0-9]", ""); }
        }
        public string ContractPOCEMailAddress
        {
            get
            { return _ContractPOCEMailAddress; }
            set
            { _ContractPOCEMailAddress = value.ToLower(); }
        }
        public string ContractPOCAlternatePocFirstname1 { get; set; }
        public string ContractPOCAlternatePocLastname1 { get; set; }
        public string ContractPOCAlternatePocPhoneWork1
        {
            get
            { return _ContractPOCAlternatePocPhoneWork1; }
            set
            { _ContractPOCAlternatePocPhoneWork1 = Regex.Replace(value, "[^0-9]", ""); }
        }
        public string ContractPOCAlternatePocEmail1
        {
            get
            { return _ContractPOCAlternatePocEmail1; }
            set
            { _ContractPOCAlternatePocEmail1 = value.ToLower(); }
        }
        public string ContractPOCAlternatePocFirstname2 { get; set; }
        public string ContractPOCAlternatePocLastname2 { get; set; }
        public string ContractPOCAlternatePocPhoneWork2
        {
            get
            { return _ContractPOCAlternatePocPhoneWork2; }
            set
            { _ContractPOCAlternatePocPhoneWork2 = Regex.Replace(value, "[^0-9]", ""); }
        }
        public string ContractPOCAlternatePocEmail2
        {
            get
            { return _ContractPOCAlternatePocEmail2; }
            set
            { _ContractPOCAlternatePocEmail2 = value.ToLower(); }
        }
        public string ContractPOCAlternatePocFirstname3 { get; set; }
        public string ContractPOCAlternatePocLastname3 { get; set; }
        public string ContractPOCAlternatePocPhoneWork3
        {
            get
            { return _ContractPOCAlternatePocPhoneWork3; }
            set
            { _ContractPOCAlternatePocPhoneWork3 = Regex.Replace(value, "[^0-9]", ""); }
        }
        public string ContractPOCAlternatePocEmail3
        {
            get
            { return _ContractPOCAlternatePocEmail3; }
            set
            { _ContractPOCAlternatePocEmail3 = value.ToLower(); }
        }
        public string ContractPOCAlternatePocFirstname4 { get; set; }
        public string ContractPOCAlternatePocLastname4 { get; set; }
        public string ContractPOCAlternatePocPhoneWork4
        {
            get
            { return _ContractPOCAlternatePocPhoneWork4; }
            set
            { _ContractPOCAlternatePocPhoneWork4 = Regex.Replace(value, "[^0-9]", ""); }
        }
        public string ContractPOCAlternatePocEmail4
        {
            get
            { return _ContractPOCAlternatePocEmail4; }
            set
            { _ContractPOCAlternatePocEmail4 = value.ToLower(); }
        }

        //Section 3
        public string RWAIAANumber { get; set; }
        public string RWAIAAAgency { get; set; }

        //Section 4
        public string BuildingNumber { get; set; }
        public string Other { get; set; }
        public string ContractorType { get; set; }
        public string ArraLongTermContractor
        {
            get { return _ArraLongTermContractor; }
            set { _ArraLongTermContractor = (value == "Not Applicable") ? "" : value; }
        }
        public string SponsoringMajorOrg { get; set; }
        public string SponsoringOfficeSymbol { get; set; }
        public string Region { get; set; }

        //Section 5
        public string InvestigationTypeRequested { get; set; }
        public string AccessCardRequired { get; set; }

        //Setion 6
        public string SponsorFirstName { get; set; }
        public string SponsorMiddleName { get; set; }
        public string SponsorLastName { get; set; }
        public string SponsorEmailAddress
        {
            get
            { return _SponsorEmailAddress; }
            set
            { _SponsorEmailAddress = value.ToLower(); }
        }
        public string SponsorPhoneWork
        {
            get
            { return _SponsorPhoneWork; }
            set
            { _SponsorPhoneWork = Regex.Replace(value, "[^0-9]", ""); }
        }
        public string SponsorIsPMCORCO { get; set; }
        public string SponsorAlternateFirstName1 { get; set; }
        public string SponsorAlternateMiddleName1 { get; set; }
        public string SponsorAlternateLastName1 { get; set; }
        public string SponsorAlternateEmailAddress1
        {
            get
            { return _SponsorAlternateEmailAddress1; }
            set
            { _SponsorAlternateEmailAddress1 = value.ToLower(); }
        }
        public string SponsorAlternatePhoneWork1
        {
            get
            { return _SponsorAlternatePhoneWork1; }
            set
            { _SponsorAlternatePhoneWork1 = Regex.Replace(value, "[^0-9]", ""); }
        }
        public string SponsorAlternateIsPMCORCO1 { get; set; }
        public string SponsorAlternateFirstName2 { get; set; }
        public string SponsorAlternateMiddleName2 { get; set; }
        public string SponsorAlternateLastName2 { get; set; }
        public string SponsorAlternateEmailAddress2
        {
            get
            { return _SponsorAlternateEmailAddress2; }
            set
            { _SponsorAlternateEmailAddress2 = value.ToLower(); }
        }
        public string SponsorAlternatePhoneWork2
        {
            get
            { return _SponsorAlternatePhoneWork2; }
            set
            { _SponsorAlternatePhoneWork2 = Regex.Replace(value, "[^0-9]", ""); }
        }
        public string SponsorAlternateIsPMCORCO2 { get; set; }
        public string SponsorAlternateFirstName3 { get; set; }
        public string SponsorAlternateMiddleName3 { get; set; }
        public string SponsorAlternateLastName3 { get; set; }
        public string SponsorAlternateEmailAddress3
        {
            get
            { return _SponsorAlternateEmailAddress3; }
            set
            { _SponsorAlternateEmailAddress3 = value.ToLower(); }
        }
        public string SponsorAlternatePhoneWork3
        {
            get
            { return _SponsorAlternatePhoneWork3; }
            set
            { _SponsorAlternatePhoneWork3 = Regex.Replace(value, "[^0-9]", ""); }
        }
        public string SponsorAlternateIsPMCORCO3 { get; set; }
        public string SponsorAlternateFirstName4 { get; set; }
        public string SponsorAlternateMiddleName4 { get; set; }
        public string SponsorAlternateLastName4 { get; set; }
        public string SponsorAlternateEmailAddress4
        {
            get
            { return _SponsorAlternateEmailAddress4; }
            set
            { _SponsorAlternateEmailAddress4 = value.ToLower(); }
        }
        public string SponsorAlternatePhoneWork4
        {
            get
            { return _SponsorAlternatePhoneWork4; }
            set
            { _SponsorAlternatePhoneWork4 = Regex.Replace(value, "[^0-9]", ""); }
        }
        public string SponsorAlternateIsPMCORCO4 { get; set; }

        public List<POC.VendorPOC> VendorPOC { get; set; }
        public List<POC.GSAPOC> GSAPOC { get; set; }
        public List<CIWData> Dupes { get; set; }

        public string FullName
        {
            get
            {

                StringBuilder fullName = new StringBuilder();

                fullName.Append("Invalid CIW - ");
                fullName.Append(LastName);
                fullName.Append(" ");

                if (!Suffix.Equals("N/A"))
                {
                    fullName.Append(Suffix);
                }

                fullName.Append(",");
                fullName.Append(" ");
                fullName.Append(FirstName);
                fullName.Append(" ");

                if (!MiddleName.Equals("NMN"))
                    fullName.Append(MiddleName);

                return fullName.ToString();
            }
        }

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