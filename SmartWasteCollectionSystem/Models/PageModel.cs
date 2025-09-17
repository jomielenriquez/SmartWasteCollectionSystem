namespace SmartWasteCollectionSystem.Models
{
    public class PageModel
    {
        private int _page = 1;
        private int _pageSize = 10;
        private string _orderByProperty = "Name";
        private bool _isAscending = false;
        public string? Search { get; set; }
        public int Page
        {
            get
            {
                return _page;
            }
            set
            {
                _page = value;
            }
        }
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = value;
            }
        }
        public string OrderByProperty
        {
            get
            {
                return _orderByProperty;
            }
            set
            {
                _orderByProperty = value;
            }
        }
        public bool IsAscending
        {
            get
            {
                return _isAscending;
            }
            set
            {
                _isAscending = value;
            }
        }
    }
}
