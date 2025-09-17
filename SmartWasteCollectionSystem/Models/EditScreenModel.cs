namespace SmartWasteCollectionSystem.Models
{
    public class EditScreenModel<T>
    {
        public T Data { get; set; }
        public List<object> UserRoles { get; set; }
        public List<string> ErrorMessages { get; set; }
    }
}
