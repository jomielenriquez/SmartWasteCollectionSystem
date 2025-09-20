namespace SmartWasteCollectionSystem.Models
{
    public class ListScreenModel<T>
    {
        public List<object> Data { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int DataCount { get; set; }
        public PageModel PageMode { get; set; }
        public T SearchModel { get; set; }
        public List<object> User { get; set; }
        public List<object> DayOfWeek { get; set; }
        public List<object> FrequencyType { get; set; }
    }
}
