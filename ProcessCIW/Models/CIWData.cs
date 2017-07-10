using ProcessCIW.Mapping;

namespace ProcessCIW.Models
{
    class CIWData
    {
        private string _innerText;

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

        public string TagName { get; set; }       
		public string Child { get; set; }
    }
}
