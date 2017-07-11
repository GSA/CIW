using System;

namespace ProcessCIW.Validation
{
    //Contract class, subset of CIW
    //Currently has no References
    class Contract
    {
        public string CompanyName { get; set; }
        public string CompanyNameSub { get; set; }
        public string DataUniversalNumberingSystem { get; set; }
        public string TaskOrderDeliveryOrder { get; set; }
        public string ContractNumberType { get; set; } 
        public DateTime? ContractStartDate { get; set; }
        public DateTime? ContractEndDate { get; set; }
        public bool HasOptionYears { get; set; }
        public int? NumberOfOptionYears { get; set; }
        public string PointOfContactFirstName { get; set; }
        public string PointOfContactLastName { get; set; }
        public string PointOfContactPhoneNumber { get; set; }
        public string PointOfContactEMailAddress { get; set; }
    }
}
