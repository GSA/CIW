using ProcessCIW.Mapping;

namespace ProcessCIW.Models
{
    /// <summary>
    /// Class used while processing CIW and creating csv file
    /// </summary>
    class CIWData
    {
        //Backing field for InnerText
        private string _innerText;

        //Property used during XML parsing of the CIW.
        //Sets InnerText to empty string if null or default value
        public string InnerText
        {
            get { return _innerText; }
            set
            {
                if (value != null && (value.Equals(CIWWordConstants.CLICK_HERE_ENTER_TEXT) || value.Equals(CIWWordConstants.CHOOSE_AN_ITEM) ||
                    value.Equals(CIWWordConstants.CLICK_HERE_TO_ENTER_DATE) || value.Equals(CIWWordConstants.ENTER_DATE_OF_BIRTH)))
                    InnerText = _innerText;
                else
                    _innerText = value;
            }
        }

        //Used for logging
        public string TagName { get; set; }    
   
        //Used for error checking, if not null, indicates nested text boxes on the CIW form.
        //This can result from improper cut/paste operations on Microsoft Word Forms 
		public string Child { get; set; }
    }
}