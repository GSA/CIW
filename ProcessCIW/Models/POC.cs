
namespace ProcessCIW.Models
{
    class POC 
    {
        public class VendorPOC
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string WorkPhone { get; set; }
            public string EMail { get; set; }
        }

        public class GSAPOC
        {            
            public string EMail { get; set; }
            public string IsPM_COR_CO_CS { get; set; }
        }
    }
}
