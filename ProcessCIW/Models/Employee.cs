
namespace ProcessCIW.Validation
{
    //Employee class, subset of CIW
    //Currently has no References
    class Employee
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
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
        public string PhoneNumberWork { get; set; }
        public string PhoneNumberWorkCell { get; set; }
        public string PersonalEmailAddress { get; set; }
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

        /// <summary>
        /// Returns the Name of the person formatted
        /// </summary>
        public string Name
        {
            get { return FirstName + ' ' + LastName + ' ' + Suffix; }
        }

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(LastName) || string.IsNullOrEmpty(MiddleName))
                return false;

            return false;
        }
    }
}
