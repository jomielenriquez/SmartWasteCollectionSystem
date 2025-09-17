namespace SmartWasteCollectionSystem.Models
{
    public class Results<T>
    {
        public string Error { get; set; }
        public bool IsSuccess { get; set; }
        public T Data { get; set; }
        public string? Message { get; set; }
        public int DeleteCount { get; set; }
        public List<string>? Errors { get; set; } = new List<string>();
    }
}
