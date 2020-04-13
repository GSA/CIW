using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using ProcessCIW.Utilities;

namespace ProcessCIW.Models
{
    

    /// <summary>
    /// The CIW class is the main data object of ProcessCIW. After the CIW is extracted into a csv, this data is then mapped to the CIW object.
    /// </summary>
    class CIW
    {
        delegate string del(string s);
        readonly del TrimPhone = new del(Utilities.Utilities.TrimPhoneNum);
        readonly del TrimPound = new del(Utilities.Utilities.TrimPoundSign);
        readonly del CleanSsn = new del(Utilities.Utilities.CleanSsn);

        //List of backing fields
        private string _VersionNumber;
        private string _PlaceOfBirthCountryName;
        private string _HomeCountryName;
        private string _CitizenCountryName;
        private string _FirstName;
        private string _MiddleName;
        private string _LastName;
        private string _Suffix;
        private string _Sex;
        private string _SocialSecurityNumber;
        private string _DateOfBirth;
        private string _PlaceOfBirthCity;
        private string _PlaceOfBirthCountry;
        private string _PlaceOfBirthState;
        private string _PlaceOfBirthMexicoCanada;
        private string _HomeAddressOne;
        private string _HomeAddressTwo;
        private string _HomeAddressCity;
        private string _HomeAddressCountry;
        private string _HomeAddressUSState;
        private string _HomeAddressMexicoStateCanadaProvince;
        private string _HomeAddressZip;
        private string _PhoneNumberWork;
        private string _PhoneNumberWorkCell;
        private string _PersonalEmailAddress;
        private string _PositionJobTitle;
        private string _PriorInvestigation;
        //_ApproximiateInvestigationDate is the pers_prior_investigation_date
        private string _ApproximiateInvestigationDate;
        private string _AgencyAdjudicatedPriorInvestigation;
        private string _Citizen;
        private string _PortOfEntryUSCityAndState;
        private string _DateOfEntry;
        private string _LessThanThreeYearsResident;
        private string _AlienRegistrationNumber;
        private string _CitzenshipCountry;
        private string _CompanyName;
        private string _CompanyNameSub;
        private string _DataUniversalNumberingSystem;
        private string _TaskOrderDeliveryOrder;
        private string _ContractNumberType;
        private string _ContractStartDate;
        private string _ContractEndDate;
        private string _HasOptionYears;
        private string _NumberOfOptionYears;
        private string _ContractPOCFirstName;
        private string _ContractPOCLastName;
        private string _ContractPOCPhoneWork;
        private string _ContractPOCEMailAddress;
        private string _ContractPOCAlternatePocFirstname1;
        private string _ContractPOCAlternatePocLastname1;
        private string _ContractPOCAlternatePocPhoneWork1;
        private string _ContractPOCAlternatePocEmail1;
        private string _ContractPOCAlternatePocFirstname2;
        private string _ContractPOCAlternatePocLastname2;
        private string _ContractPOCAlternatePocPhoneWork2;
        private string _ContractPOCAlternatePocEmail2;
        private string _ContractPOCAlternatePocFirstname3;
        private string _ContractPOCAlternatePocLastname3;
        private string _ContractPOCAlternatePocPhoneWork3;
        private string _ContractPOCAlternatePocEmail3;
        private string _ContractPOCAlternatePocFirstname4;
        private string _ContractPOCAlternatePocLastname4;
        private string _ContractPOCAlternatePocPhoneWork4;
        private string _ContractPOCAlternatePocEmail4;
        private string _RWAIAANumber;
        private string _RWAIAAAgency;
        private string _BuildingNumber;
        private string _Other;
        private string _ContractorType;
        private string _ArraLongTermContractor;
        private string _SponsoringMajorOrg;
        private string _SponsoringOfficeSymbol;
        private string _Region;
        private string _InvestigationTypeRequested;
        private string _AccessCardRequired;
        private string _SponsorFirstName;
        private string _SponsorMiddleName;
        private string _SponsorLastName;
        private string _SponsorEmailAddress;
        private string _SponsorPhoneWork;
        private string _SponsorIsPMCORCO;
        private string _SponsorAlternateFirstName1;
        private string _SponsorAlternateMiddleName1;
        private string _SponsorAlternateLastName1;
        private string _SponsorAlternateEmailAddress1;
        private string _SponsorAlternatePhoneWork1;
        private string _SponsorAlternateIsPMCORCO1;
        private string _SponsorAlternateFirstName2;
        private string _SponsorAlternateMiddleName2;
        private string _SponsorAlternateLastName2;
        private string _SponsorAlternateEmailAddress2;
        private string _SponsorAlternatePhoneWork2;
        private string _SponsorAlternateIsPMCORCO2;
        private string _SponsorAlternateFirstName3;
        private string _SponsorAlternateMiddleName3;
        private string _SponsorAlternateLastName3;
        private string _SponsorAlternateEmailAddress3;
        private string _SponsorAlternatePhoneWork3;
        private string _SponsorAlternateIsPMCORCO3;
        private string _SponsorAlternateFirstName4;
        private string _SponsorAlternateMiddleName4;
        private string _SponsorAlternateLastName4;
        private string _SponsorAlternateEmailAddress4;
        private string _SponsorAlternatePhoneWork4;
        private string _SponsorAlternateIsPMCORCO4;

        

        

        /// <summary>
        /// Section 1
        /// </summary>

        //Assigns "V0" to version if version is null
        public string VersionNumber
        {
            get { return (_VersionNumber == null) ? "V0" : _VersionNumber; }
            set { _VersionNumber = value.Trim(); }
        }

        public string PlaceOfBirthCountryName
        {
            get { return _PlaceOfBirthCountryName; }
            set { _PlaceOfBirthCountryName = value.Trim(); }
        }
        public string HomeCountryName
        {
            get { return _HomeCountryName; }
            set { _HomeCountryName = value.Trim(); }
        }
        public string CitizenCountryName
        {
            get { return _CitizenCountryName; }
            set { _CitizenCountryName = value.Trim(); }
        }

        //Trim 3 name parts
        public string FirstName
        {
            get { return _FirstName; }
            set { _FirstName = value.Trim(); }
        }
        public string MiddleName
        {
            get { return _MiddleName; }
            set { _MiddleName = value.Trim(); }
        }
        public string LastName
        {
            get { return _LastName; }
            set { _LastName = value.Trim(); }
        }

        //If suffix value is "N/A", stores an empty string
        public string Suffix
        {
            get { return _Suffix; }
            set { _Suffix = (value == "N/A") ? "" : value.Trim(); }
        }

        public string Sex
        {
            get { return _Sex; }
            set { _Sex = value.Trim(); }
        }

        public string SocialSecurityNumber
        {
            get { return _SocialSecurityNumber; }
            set { _SocialSecurityNumber = CleanSsn(value); }
        }
        public string DateOfBirth
        {
            get { return _DateOfBirth; }
            set { _DateOfBirth = value.Trim(); }
        }
        public string PlaceOfBirthCity
        {
            get { return _PlaceOfBirthCity; }
            set { _PlaceOfBirthCity = value.Trim(); }
        }
        public string PlaceOfBirthCountry
        {
            get { return _PlaceOfBirthCountry;
            }
            set {
                _PlaceOfBirthCountry = value.Trim();
            }
        }
        public string PlaceOfBirthState
        {
            get { return _PlaceOfBirthState;
            }
            set { _PlaceOfBirthState = value.Trim();
            }
        }
        public string PlaceOfBirthMexicoCanada
        {
            get { return _PlaceOfBirthMexicoCanada; }
            set { _PlaceOfBirthMexicoCanada = value.Trim(); }
        }

        //Remove # sign from addresses, mso does not support, 7-2-2018
        public string HomeAddressOne
        {
            get { return _HomeAddressOne; }
            set { _HomeAddressOne = TrimPound(value.Trim()); }
        }
        public string HomeAddressTwo
        {
            get { return _HomeAddressTwo; }
            set { _HomeAddressTwo = TrimPound(value.Trim()); }
        }
        public string HomeAddressCity
        {
            get { return _HomeAddressCity; }
            set { _HomeAddressCity = TrimPound(value.Trim()); }
        }

        public string HomeAddressCountry
        {
            get { return _HomeAddressCountry; }
            set { _HomeAddressCountry = value.Trim(); }
        }
        public string HomeAddressUSState
        {
            get { return _HomeAddressUSState; }
            set { _HomeAddressUSState = value.Trim(); }
        }
        public string HomeAddressMexicoStateCanadaProvince
        {
            get { return _HomeAddressMexicoStateCanadaProvince; }
            set { _HomeAddressMexicoStateCanadaProvince = value.Trim(); }
        }
        public string HomeAddressZip
        {
            get { return _HomeAddressZip; }
            set { _HomeAddressZip = value.Trim(); }
        }

        //Removes all non digit characters from PhoneNumberWork
        public string PhoneNumberWork
        {
            get
            { return _PhoneNumberWork; }
            set
            { _PhoneNumberWork = TrimPhone(value.Trim()); }
        }

        //Removes all non digit characters from PhoneNumberCell
        public string PhoneNumberWorkCell
        {
            get
            { return _PhoneNumberWorkCell; }
            set
            { _PhoneNumberWorkCell = TrimPhone(value.Trim()); }
        }

        //Stores PersonalEmailAddress as lower case
        public string PersonalEmailAddress
        {
            get
            { return _PersonalEmailAddress; }
            set
            { _PersonalEmailAddress = value.ToLower().Trim(); }
        }

        public string PositionJobTitle
        {
            get { return _PositionJobTitle; }
            set { _PositionJobTitle = value.Trim(); }
        }
        public string PriorInvestigation
        {
            get { return _PriorInvestigation; }
            set { _PriorInvestigation = value.Trim(); }
        }

        //_ApproximiateInvestigationDate is the pers_prior_investigation_date
        public string ApproximiateInvestigationDate
        {
            get { return _ApproximiateInvestigationDate; }
            set { _ApproximiateInvestigationDate = value.Trim(); }
        }
        public string AgencyAdjudicatedPriorInvestigation
        {
            get { return _AgencyAdjudicatedPriorInvestigation; }
            set { _AgencyAdjudicatedPriorInvestigation = value.Trim(); }
        }
        public string Citizen
        {
            get { return _Citizen; }
            set { _Citizen = value.Trim(); }
        }
        public string PortOfEntryUSCityAndState
        {
            get { return _PortOfEntryUSCityAndState; }
            set { _PortOfEntryUSCityAndState = value.Trim(); }
        }
        public string DateOfEntry
        {
            get { return _DateOfEntry; }
            set { _DateOfEntry = value.Trim(); }
        }
        public string LessThanThreeYearsResident
        {
            get { return _LessThanThreeYearsResident; }
            set { _LessThanThreeYearsResident = value.Trim(); }
        }
        public string AlienRegistrationNumber
        {
            get { return _AlienRegistrationNumber; }
            set { _AlienRegistrationNumber = value.Trim(); }
        }
        public string CitzenshipCountry
        {
            get { return _CitzenshipCountry; }
            set { _CitzenshipCountry = value.Trim(); }
        }

        /// <summary>
        /// Section 2
        /// </summary>

        public string CompanyName
        {
            get { return _CompanyName; }
            set { _CompanyName = value.Trim(); }
        }
        public string CompanyNameSub
        {
            get { return _CompanyNameSub; }
            set { _CompanyNameSub = value.Trim(); }
        }
        public string DataUniversalNumberingSystem
        {
            get { return _DataUniversalNumberingSystem; }
            set { _DataUniversalNumberingSystem = value.Trim(); }
        }
        public string TaskOrderDeliveryOrder
        {
            get { return _TaskOrderDeliveryOrder; }
            set { _TaskOrderDeliveryOrder = value.Trim(); }
        }
        public string ContractNumberType
        {
            get { return _ContractNumberType; }
            set { _ContractNumberType = value.Trim(); }
        }
        public string ContractStartDate
        {
            get { return _ContractStartDate; }
            set { _ContractStartDate = value.Trim(); }
        }
        public string ContractEndDate
        {
            get { return _ContractEndDate; }

            set { _ContractEndDate = value.Trim(); }
        }
        public string HasOptionYears
        {
            get { return _HasOptionYears; }

            set { _HasOptionYears = value.Trim(); }
        }
        public string NumberOfOptionYears
        {
            get { return _NumberOfOptionYears; }

            set { _NumberOfOptionYears = value.Trim(); }
        }
        public string ContractPOCFirstName
        {
            get { return _ContractPOCFirstName; }

            set { _ContractPOCFirstName = value.Trim(); }
        }
        public string ContractPOCLastName
        {
            get { return _ContractPOCLastName; }

            set { _ContractPOCLastName = value.Trim(); }
        }

        //Removes all non digit characters from ContractPOCPhoneWork
        public string ContractPOCPhoneWork
        {
            get
            { return _ContractPOCPhoneWork; }
            set
            { _ContractPOCPhoneWork = TrimPhone(value.Trim()); }
        }

        //Stores ContractPOCEMailAddress as lower case
        public string ContractPOCEMailAddress
        {
            get
            { return _ContractPOCEMailAddress; }
            set
            { _ContractPOCEMailAddress = value.ToLower().Trim(); }
        }

        public string ContractPOCAlternatePocFirstname1
        {
            get { return _ContractPOCAlternatePocFirstname1; }

            set { _ContractPOCAlternatePocFirstname1 = value.Trim(); }
        }
        public string ContractPOCAlternatePocLastname1
        {
            get { return _ContractPOCAlternatePocLastname1; }

            set { _ContractPOCAlternatePocLastname1 = value.Trim(); }
        }

        //Removes all non digit characters from ContractPOCAlternatePocPhoneWork1
        public string ContractPOCAlternatePocPhoneWork1
        {
            get
            { return _ContractPOCAlternatePocPhoneWork1; }
            set
            { _ContractPOCAlternatePocPhoneWork1 = TrimPhone(value.Trim()); }
        }

        //Stores ContractPOCAlternatePocEmail1 as lower case
        public string ContractPOCAlternatePocEmail1
        {
            get
            { return _ContractPOCAlternatePocEmail1; }
            set
            { _ContractPOCAlternatePocEmail1 = value.ToLower().Trim(); }
        }

        public string ContractPOCAlternatePocFirstname2
        {
            get { return _ContractPOCAlternatePocFirstname2; }

            set { _ContractPOCAlternatePocFirstname2 = value.Trim(); }
        }
        public string ContractPOCAlternatePocLastname2
        {
            get { return _ContractPOCAlternatePocLastname2; }

            set { _ContractPOCAlternatePocLastname2 = value.Trim(); }
        }

        //Removes all non digit characters from ContractPOCAlternatePocPhoneWork2
        public string ContractPOCAlternatePocPhoneWork2
        {
            get
            { return _ContractPOCAlternatePocPhoneWork2; }
            set
            { _ContractPOCAlternatePocPhoneWork2 = TrimPhone(value.Trim()); }
        }

        //Stores ContractPOCAlternatePocEmail2 as lower case
        public string ContractPOCAlternatePocEmail2
        {
            get
            { return _ContractPOCAlternatePocEmail2; }
            set
            { _ContractPOCAlternatePocEmail2 = value.ToLower().Trim(); }
        }

        public string ContractPOCAlternatePocFirstname3
        {
            get { return _ContractPOCAlternatePocFirstname3; }

            set { _ContractPOCAlternatePocFirstname3 = value.Trim(); }
        }
        public string ContractPOCAlternatePocLastname3
        {
            get { return _ContractPOCAlternatePocLastname3; }

            set { _ContractPOCAlternatePocLastname3 = value.Trim(); }
        }

        //Removes all non digit characters from ContractPOCAlternatePocPhoneWork3
        public string ContractPOCAlternatePocPhoneWork3
        {
            get
            { return _ContractPOCAlternatePocPhoneWork3; }
            set
            { _ContractPOCAlternatePocPhoneWork3 = TrimPhone(value.Trim()); }
        }

        //Stores ContractPOCAlternatePocEmail3 as lower case
        public string ContractPOCAlternatePocEmail3
        {
            get
            { return _ContractPOCAlternatePocEmail3; }
            set
            { _ContractPOCAlternatePocEmail3 = value.ToLower().Trim(); }
        }

        public string ContractPOCAlternatePocFirstname4
        {
            get { return _ContractPOCAlternatePocFirstname4; }

            set { _ContractPOCAlternatePocFirstname4 = value.Trim(); }
        }
        public string ContractPOCAlternatePocLastname4
        {
            get { return _ContractPOCAlternatePocLastname4; }

            set { _ContractPOCAlternatePocLastname4 = value.Trim(); }
        }

        //Removes all non digit characters from ContractPOCAlternatePocPhoneWork4
        public string ContractPOCAlternatePocPhoneWork4
        {
            get
            { return _ContractPOCAlternatePocPhoneWork4; }
            set
            { _ContractPOCAlternatePocPhoneWork4 = TrimPhone(value.Trim()); }
        }

        //Stores ContractPOCAlternatePocEmail4 as lower case
        public string ContractPOCAlternatePocEmail4
        {
            get
            { return _ContractPOCAlternatePocEmail4; }
            set
            { _ContractPOCAlternatePocEmail4 = value.ToLower().Trim(); }
        }

        /// <summary>
        /// Section 3
        /// </summary>

        public string RWAIAANumber
        {
            get { return _RWAIAANumber; }

            set { _RWAIAANumber = value.Trim(); }
        }
        public string RWAIAAAgency
        {
            get { return _RWAIAAAgency; }

            set { _RWAIAAAgency = value.Trim(); }
        }

        /// <summary>
        /// Section 4
        /// </summary>

        public string BuildingNumber
        {
            get { return _BuildingNumber; }

            set { _BuildingNumber = value.Trim(); }
        }
        public string Other
        {
            get { return _Other; }

            set { _Other = value.Trim(); }
        }
        public string ContractorType
        {
            get { return _ContractorType; }

            set { _ContractorType = value.Trim(); }
        }

        //Stores ArraLongTermContractor as empty string if value is "Not Applicable"
        public string ArraLongTermContractor
        {
            get { return _ArraLongTermContractor; }
            set { _ArraLongTermContractor = (value == "Not Applicable") ? "" : value.Trim(); }
        }

        public string SponsoringMajorOrg
        {
            get { return _SponsoringMajorOrg; }

            set { _SponsoringMajorOrg = value.Trim(); }
        }
        public string SponsoringOfficeSymbol
        {
            get { return _SponsoringOfficeSymbol; }

            set { _SponsoringOfficeSymbol = value.Trim(); }
        }
        public string Region
        {
            get { return _Region; }

            set { _Region = value.Trim(); }
        }

        /// <summary>
        /// Section 5
        /// </summary>

        public string InvestigationTypeRequested
        {
            get { return _InvestigationTypeRequested; }

            set { _InvestigationTypeRequested = value.Trim(); }
        }
        public string AccessCardRequired
        {
            get { return _AccessCardRequired; }

            set { _AccessCardRequired = value.Trim(); }
        }

        /// <summary>
        /// Section 6
        /// </summary>

        public string SponsorFirstName
        {
            get { return _SponsorFirstName; }

            set { _SponsorFirstName = value.Trim(); }
        }
        public string SponsorMiddleName
        {
            get { return _SponsorMiddleName; }

            set { _SponsorMiddleName = value.Trim(); }
        }
        public string SponsorLastName
        {
            get { return _SponsorLastName; }

            set { _SponsorLastName = value.Trim(); }
        }

        //Stores SponsorEmailAddress as lower case
        public string SponsorEmailAddress
        {
            get
            { return _SponsorEmailAddress; }
            set
            { _SponsorEmailAddress = value.ToLower().Trim(); }
        }

        //Removes all non digit characters from SponsorPhoneWork
        public string SponsorPhoneWork
        {
            get
            { return _SponsorPhoneWork; }
            set
            { _SponsorPhoneWork = TrimPhone(value.Trim()); }
        }

        public string SponsorIsPMCORCO
        {
            get { return _SponsorIsPMCORCO; }

            set { _SponsorIsPMCORCO = value.Trim(); }
        }
        public string SponsorAlternateFirstName1
        {
            get { return _SponsorAlternateFirstName1; }

            set { _SponsorAlternateFirstName1 = value.Trim(); }
        }
        public string SponsorAlternateMiddleName1
        {
            get { return _SponsorAlternateMiddleName1; }

            set { _SponsorAlternateMiddleName1 = value.Trim(); }
        }
        public string SponsorAlternateLastName1
        {
            get { return _SponsorAlternateLastName1; }

            set { _SponsorAlternateLastName1 = value.Trim(); }
        }

        //Stores SponsorAlternateEmailAddress1 as lower case
        public string SponsorAlternateEmailAddress1
        {
            get
            { return _SponsorAlternateEmailAddress1; }
            set
            { _SponsorAlternateEmailAddress1 = value.ToLower().Trim(); }
        }

        //Removes all non digit characters from SponsorAlternatePhoneWork1
        public string SponsorAlternatePhoneWork1
        {
            get
            { return _SponsorAlternatePhoneWork1; }
            set
            { _SponsorAlternatePhoneWork1 = TrimPhone(value.Trim()); }
        }

        public string SponsorAlternateIsPMCORCO1
        {
            get { return _SponsorAlternateIsPMCORCO1; }

            set { _SponsorAlternateIsPMCORCO1 = value.Trim(); }
        }
        public string SponsorAlternateFirstName2
        {
            get { return _SponsorAlternateFirstName2; }

            set { _SponsorAlternateFirstName2 = value.Trim(); }
        }
        public string SponsorAlternateMiddleName2
        {
            get { return _SponsorAlternateMiddleName2; }

            set { _SponsorAlternateMiddleName2 = value.Trim(); }
        }
        public string SponsorAlternateLastName2
        {
            get { return _SponsorAlternateLastName2; }

            set { _SponsorAlternateLastName2 = value.Trim(); }
        }

        //Stores SponsorAlternateEmailAddress2 as lower case
        public string SponsorAlternateEmailAddress2
        {
            get
            { return _SponsorAlternateEmailAddress2; }
            set
            { _SponsorAlternateEmailAddress2 = value.ToLower().Trim(); }
        }

        //Removes all non digit characters from SponsorAlternatePhoneWork2
        public string SponsorAlternatePhoneWork2
        {
            get
            { return _SponsorAlternatePhoneWork2; }
            set
            { _SponsorAlternatePhoneWork2 = TrimPhone(value.Trim()); }
        }

        public string SponsorAlternateIsPMCORCO2
        {
            get { return _SponsorAlternateIsPMCORCO2; }

            set { _SponsorAlternateIsPMCORCO2 = value.Trim(); }
        }
        public string SponsorAlternateFirstName3
        {
            get { return _SponsorAlternateFirstName3; }

            set { _SponsorAlternateFirstName3 = value.Trim(); }
        }
        public string SponsorAlternateMiddleName3
        {
            get { return _SponsorAlternateMiddleName3; }

            set { _SponsorAlternateMiddleName3 = value.Trim(); }
        }
        public string SponsorAlternateLastName3
        {
            get { return _SponsorAlternateLastName3; }

            set { _SponsorAlternateLastName3 = value.Trim(); }
        }

        //Stores SponsorAlternateEmailAddress3 as lower case
        public string SponsorAlternateEmailAddress3
        {
            get
            { return _SponsorAlternateEmailAddress3; }
            set
            { _SponsorAlternateEmailAddress3 = value.ToLower().Trim(); }
        }

        //Removes all non digit characters from SponsorAlternatePhoneWork3
        public string SponsorAlternatePhoneWork3
        {
            get
            { return _SponsorAlternatePhoneWork3; }
            set
            { _SponsorAlternatePhoneWork3 = TrimPhone(value.Trim()); }
        }

        public string SponsorAlternateIsPMCORCO3
        {
            get { return _SponsorAlternateIsPMCORCO3; }

            set { _SponsorAlternateIsPMCORCO3 = value.Trim(); }
        }
        public string SponsorAlternateFirstName4
        {
            get { return _SponsorAlternateFirstName4; }

            set { _SponsorAlternateFirstName4 = value.Trim(); }
        }
        public string SponsorAlternateMiddleName4
        {
            get { return _SponsorAlternateMiddleName4; }

            set { _SponsorAlternateMiddleName4 = value.Trim(); }
        }
        public string SponsorAlternateLastName4
        {
            get { return _SponsorAlternateLastName4; }

            set { _SponsorAlternateLastName4 = value.Trim(); }
        }

        //Stores SponsorAlternateEmailAddress4 as lower case
        public string SponsorAlternateEmailAddress4
        {
            get
            { return _SponsorAlternateEmailAddress4; }
            set
            { _SponsorAlternateEmailAddress4 = value.ToLower().Trim(); }
        }

        //Removes all non digit characters from SponsorAlternatePhoneWork4
        public string SponsorAlternatePhoneWork4
        {
            get
            { return _SponsorAlternatePhoneWork4; }
            set
            { _SponsorAlternatePhoneWork4 = TrimPhone(value.Trim()); }
        }

        public string SponsorAlternateIsPMCORCO4
        {
            get { return _SponsorAlternateIsPMCORCO4; }

            set { _SponsorAlternateIsPMCORCO4 = value.Trim(); }
        }

        //List of VendorPOC's to iterate through, checking for non empty lines
        public List<POC.VendorPOC> VendorPOC { get; set; }

        //List of GSAPOC's to iterate  through, checking for non empty lines
        public List<POC.GSAPOC> GSAPOC { get; set; }       

        /// <summary>
        /// Property utilizing StringBuilder to genererate a name field to be used in logging
        /// </summary>
        public string FullNameForLog
        {
            get
            {
                StringBuilder fullName = new StringBuilder();

                fullName.Append(FirstName == null ? "FirstName is NULL" : ReturnFieldOrAltText(FirstName, "FirstName"));
                fullName.Append(" ");
                fullName.Append(LastName == null ? "LastName is NULL" : ReturnFieldOrAltText(LastName, "LastName"));

                return fullName.ToString();
            }
        }

        public string ReturnFieldOrAltText(string value, string fieldName)
        {
            return value == "" ? string.Format("{0} is EMPTY", fieldName) : value;
        }
    }
}