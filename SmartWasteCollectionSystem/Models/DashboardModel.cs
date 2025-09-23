namespace SmartWasteCollectionSystem.Models
{
    public class DashboardModel
    {
        public List<User> Users { get; set; }
        public List<GarbageCollectionSchedule> GarbageCollectionSchedules { get; set; }
        public int BinStatusPercentage { get; set; }
    }
}
