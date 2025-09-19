namespace SmartWasteCollectionSystem.Models
{
    public class MonthlyDueSearchModel
    {
        public Guid UserId { get; set; }
        public bool IsPaid { get; set; }
    }
}
