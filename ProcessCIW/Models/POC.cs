
namespace ProcessCIW.Models
{
    /// <summary>
    /// POC class represents a single line of either VendorPOC's or GSAPOC's from the CIW form
    /// </summary>
    class POC 
    {
        //VendorPOC used when evaluating for non empty lines on the CIW form
        public class VendorPOC
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string WorkPhone { get; set; }
            public string EMail { get; set; }
        }

        //GSAPOC used when evaluating for non empty lines on the CIW form
        public class GSAPOC
        {            
            public string EMail { get; set; }
            public string IsPM_COR_CO_CS { get; set; }
        }
    }
}
