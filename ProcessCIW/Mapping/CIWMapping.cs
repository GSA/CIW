using CsvHelper.Configuration;
using ProcessCIW.Models;

namespace ProcessCIW.Mapping
{
    sealed class CIWMapping : CsvClassMap<CIW>
    {
        public CIWMapping()
        {
            //Version
            Map(m => m.VersionNumber).Name(CIWConstants.VERSION_NUMBER).Default("V0");

            //Section 1
            Map(m => m.LastName).Name(CIWConstants.EMPLOYEE_LASTNAME);
            Map(m => m.FirstName).Name(CIWConstants.EMPLOYEE_FIRSTNAME);
            Map(m => m.MiddleName).Name(CIWConstants.EMPLOYEE_MIDDLENAME);
            Map(m => m.Suffix).Name(CIWConstants.EMPLOYEE_SUFFIX);
            Map(m => m.Sex).Name(CIWConstants.EMPLOYEE_GENDER);
            Map(m => m.SocialSecurityNumber).Name(CIWConstants.EMPLOYEE_SSN);
            Map(m => m.DateOfBirth).Name(CIWConstants.EMPLOYEE_DATEOFBIRTH);
            Map(m => m.PlaceOfBirthCity).Name(CIWConstants.EMPLOYEE_PLACEOFBIRTHCITY);
            Map(m => m.PlaceOfBirthCountry).Name(CIWConstants.EMPLOYEE_PLACEOFBIRTHCOUNTRY);
            Map(m => m.PlaceOfBirthState).Name(CIWConstants.EMPLOYEE_PLACEOFBIRTHSTATE);
            Map(m => m.PlaceOfBirthMexicoCanada).Name(CIWConstants.EMPLOYEE_PLACEOFBIRTHMEXICOCANADA);
            Map(m => m.HomeAddressOne).Name(CIWConstants.EMPLOYEE_HOMEADDRESS);
            Map(m => m.HomeAddressTwo).Name(CIWConstants.EMPLOYEE_HOMEADDRESS2);
            Map(m => m.HomeAddressCity).Name(CIWConstants.EMPLOYEE_CITY);
            Map(m => m.HomeAddressCountry).Name(CIWConstants.EMPLOYEE_COUNTRY);
            Map(m => m.HomeAddressUSState).Name(CIWConstants.EMPLOYEE_STATE);
            Map(m => m.HomeAddressMexicoStateCanadaProvince).Name(CIWConstants.EMPLOYEE_ADDRESSMEXICOCANADA);
            Map(m => m.HomeAddressZip).Name(CIWConstants.EMPLOYEE_ZIP);
            Map(m => m.PhoneNumberWorkCell).Name(CIWConstants.EMPLOYEE_PHONECELL);
            Map(m => m.PhoneNumberWork).Name(CIWConstants.EMPLOYEE_PHONEWORK);
            Map(m => m.PersonalEmailAddress).Name(CIWConstants.EMPLOYEE_EMAIL);
            Map(m => m.PositionJobTitle).Name(CIWConstants.EMPLOYEE_JOBTITLE);
            Map(m => m.PriorInvestigation).Name(CIWConstants.EMPLOYEE_PRIORINVESTIGATION);
            Map(m => m.ApproximiateInvestigationDate).Name(CIWConstants.EMPLOYEE_APPROXIMATEINVESTIGATIONDATE);
            Map(m => m.AgencyAdjudicatedPriorInvestigation).Name(CIWConstants.EMPLOYEE_ADJUDICATEDPRIORINVESTIGATION);
            Map(m => m.Citizen).Name(CIWConstants.EMPLOYEE_CITIZEN);
            Map(m => m.PortOfEntryUSCityAndState).Name(CIWConstants.EMPLOYEE_PORTOFENTRYCITYANDSTATE);
            Map(m => m.DateOfEntry).Name(CIWConstants.EMPLOYEE_DATEOFENTRY);
            Map(m => m.LessThanThreeYearsResident).Name(CIWConstants.EMPLOYEE_LESSTHANTHREEYEARS);
            Map(m => m.AlienRegistrationNumber).Name(CIWConstants.EMPLOYEE_ALIENREGISTRATIONNUMBER);
            Map(m => m.CitzenshipCountry).Name(CIWConstants.EMPLOYEE_CITZENSHIPCOUNTRY);

            //Seciton 2
            Map(m => m.CompanyName).Name(CIWConstants.CONTRACT_COMPANYNAME);
            Map(m => m.CompanyNameSub).Name(CIWConstants.CONTRACT_IFSUBNAMEOFPRIME);
            Map(m => m.DataUniversalNumberingSystem).Name(CIWConstants.CONTRACT_DUNS);
            Map(m => m.TaskOrderDeliveryOrder).Name(CIWConstants.CONTRACT_TASKORDERDELIVERYORDER);
            Map(m => m.ContractNumberType).Name(CIWConstants.CONTRACT_NUMBERTYPE);
            Map(m => m.ContractStartDate).Name(CIWConstants.CONTRACT_STARTDATE);
            Map(m => m.ContractEndDate).Name(CIWConstants.CONTRACT_ENDDATE);
            Map(m => m.HasOptionYears).Name(CIWConstants.CONTRACT_HASOPTIONYEARS);
            Map(m => m.NumberOfOptionYears).Name(CIWConstants.CONTRACT_NUMBEROFOPTIONYEARS);
            Map(m => m.ContractPOCFirstName).Name(CIWConstants.CONTRACT_POCFIRSTNAME);
            Map(m => m.ContractPOCLastName).Name(CIWConstants.CONTRACT_POCLASTNAME);
            Map(m => m.ContractPOCPhoneWork).Name(CIWConstants.CONTRACT_POCPHONEWORK);
            Map(m => m.ContractPOCEMailAddress).Name(CIWConstants.CONTRACT_POCEMAIL);
            Map(m => m.ContractPOCAlternatePocFirstname1).Name(CIWConstants.CONTRACT_ALTERNATEPOCFIRSTNAME1);
            Map(m => m.ContractPOCAlternatePocLastname1).Name(CIWConstants.CONTRACT_ALTERNATEPOCLASTNAME1);
            Map(m => m.ContractPOCAlternatePocPhoneWork1).Name(CIWConstants.CONTRACT_ALTERNATEPOCPHONEWORK1);
            Map(m => m.ContractPOCAlternatePocEmail1).Name(CIWConstants.CONTRACT_ALTERNATEPOCEMAIL1);
            Map(m => m.ContractPOCAlternatePocFirstname2).Name(CIWConstants.CONTRACT_ALTERNATEPOCFIRSTNAME2);
            Map(m => m.ContractPOCAlternatePocLastname2).Name(CIWConstants.CONTRACT_ALTERNATEPOCLASTNAME2);
            Map(m => m.ContractPOCAlternatePocPhoneWork2).Name(CIWConstants.CONTRACT_ALTERNATEPOCPHONEWORK2);
            Map(m => m.ContractPOCAlternatePocEmail2).Name(CIWConstants.CONTRACT_ALTERNATEPOCEMAIL2);

            Map(m => m.ContractPOCAlternatePocFirstname3).Name(CIWConstants.CONTRACT_ALTERNATEPOCFIRSTNAME3);
            Map(m => m.ContractPOCAlternatePocLastname3).Name(CIWConstants.CONTRACT_ALTERNATEPOCLASTNAME3);
            Map(m => m.ContractPOCAlternatePocPhoneWork3).Name(CIWConstants.CONTRACT_ALTERNATEPOCPHONEWORK3);
            Map(m => m.ContractPOCAlternatePocEmail3).Name(CIWConstants.CONTRACT_ALTERNATEPOCEMAIL3);

            Map(m => m.ContractPOCAlternatePocFirstname4).Name(CIWConstants.CONTRACT_ALTERNATEPOCFIRSTNAME4);
            Map(m => m.ContractPOCAlternatePocLastname4).Name(CIWConstants.CONTRACT_ALTERNATEPOCLASTNAME4);
            Map(m => m.ContractPOCAlternatePocPhoneWork4).Name(CIWConstants.CONTRACT_ALTERNATEPOCPHONEWORK4);
            Map(m => m.ContractPOCAlternatePocEmail4).Name(CIWConstants.CONTRACT_ALTERNATEPOCEMAIL4);           

            //Section 3
            Map(m => m.RWAIAANumber).Name(CIWConstants.RWAIAA_NUMBER);
            Map(m => m.RWAIAAAgency).Name(CIWConstants.RWAIAA_AGENCY);

            //Section 4
            Map(m => m.BuildingNumber).Name(CIWConstants.PROJECT_BUILDINGNUMBER);
            Map(m => m.Other).Name(CIWConstants.PROJECT_OTHER);
            Map(m => m.ContractorType).Name(CIWConstants.PROJECT_CONTRACTORTYPE);
            Map(m => m.ArraLongTermContractor).Name(CIWConstants.PROJECT_ARRALONGTERMCONTRACTOR);
            Map(m => m.SponsoringMajorOrg).Name(CIWConstants.PROJECT_SPONSORINGMAJORORG);
            Map(m => m.SponsoringOfficeSymbol).Name(CIWConstants.PROJECT_SPONSORINGOFFICESYMBOL);
            Map(m => m.Region).Name(CIWConstants.PROJECT_REGION);

            //Section 5
            Map(m => m.InvestigationTypeRequested).Name(CIWConstants.INVESTIGATION_TYPEOFINVESTIGATIONREQUESTED);
            Map(m => m.AccessCardRequired).Name(CIWConstants.INVESTIGATION_CARDREQUIRED);

            //Section 6
            Map(m => m.SponsorLastName).Name(CIWConstants.SPONSOR_LASTNAME);
            Map(m => m.SponsorFirstName).Name(CIWConstants.SPONSOR_FIRSTNAME);
            Map(m => m.SponsorMiddleName).Name(CIWConstants.SPONSOR_MIDDLENAME);
            Map(m => m.SponsorEmailAddress).Name(CIWConstants.SPONSOR_EMAIL);
            Map(m => m.SponsorPhoneWork).Name(CIWConstants.SPONSOR_PHONEWORK);
            Map(m => m.SponsorIsPMCORCO).Name(CIWConstants.SPONSOR_ISPMCORCO);

            Map(m => m.SponsorAlternateLastName1).Name(CIWConstants.SPONSOR_ALTERNATELASTNAME1);
            Map(m => m.SponsorAlternateFirstName1).Name(CIWConstants.SPONSOR_ALTERNATEFIRSTNAME1);
            Map(m => m.SponsorAlternateMiddleName1).Name(CIWConstants.SPONSOR_ALTERNATEMIDDLENAME1);
            Map(m => m.SponsorAlternateEmailAddress1).Name(CIWConstants.SPONSOR_ALTERNATEEMAIL1);
            Map(m => m.SponsorAlternatePhoneWork1).Name(CIWConstants.SPONSOR_ALTERNATEPHONEWORK1);
            Map(m => m.SponsorAlternateIsPMCORCO1).Name(CIWConstants.SPONSOR_ALTERNATEISPMCORCO1);

            Map(m => m.SponsorAlternateLastName2).Name(CIWConstants.SPONSOR_ALTERNATELASTNAME2);
            Map(m => m.SponsorAlternateFirstName2).Name(CIWConstants.SPONSOR_ALTERNATEFIRSTNAME2);
            Map(m => m.SponsorAlternateMiddleName2).Name(CIWConstants.SPONSOR_ALTERNATEMIDDLENAME2);
            Map(m => m.SponsorAlternateEmailAddress2).Name(CIWConstants.SPONSOR_ALTERNATEEMAIL2);
            Map(m => m.SponsorAlternatePhoneWork2).Name(CIWConstants.SPONSOR_ALTERNATEPHONEWORK2);
            Map(m => m.SponsorAlternateIsPMCORCO2).Name(CIWConstants.SPONSOR_ALTERNATEISPMCORCO2);

            Map(m => m.SponsorAlternateLastName3).Name(CIWConstants.SPONSOR_ALTERNATELASTNAME3);
            Map(m => m.SponsorAlternateFirstName3).Name(CIWConstants.SPONSOR_ALTERNATEFIRSTNAME3);
            Map(m => m.SponsorAlternateMiddleName3).Name(CIWConstants.SPONSOR_ALTERNATEMIDDLENAME3);
            Map(m => m.SponsorAlternateEmailAddress3).Name(CIWConstants.SPONSOR_ALTERNATEEMAIL3);
            Map(m => m.SponsorAlternatePhoneWork3).Name(CIWConstants.SPONSOR_ALTERNATEPHONEWORK3);
            Map(m => m.SponsorAlternateIsPMCORCO3).Name(CIWConstants.SPONSOR_ALTERNATEISPMCORCO3);

            Map(m => m.SponsorAlternateLastName4).Name(CIWConstants.SPONSOR_ALTERNATELASTNAME4);
            Map(m => m.SponsorAlternateFirstName4).Name(CIWConstants.SPONSOR_ALTERNATEFIRSTNAME4);
            Map(m => m.SponsorAlternateMiddleName4).Name(CIWConstants.SPONSOR_ALTERNATEMIDDLENAME4);
            Map(m => m.SponsorAlternateEmailAddress4).Name(CIWConstants.SPONSOR_ALTERNATEEMAIL4);
            Map(m => m.SponsorAlternatePhoneWork4).Name(CIWConstants.SPONSOR_ALTERNATEPHONEWORK4);
            Map(m => m.SponsorAlternateIsPMCORCO4).Name(CIWConstants.SPONSOR_ALTERNATEISPMCORCO4);
        }
    }    
}
