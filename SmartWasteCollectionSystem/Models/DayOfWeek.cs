using System;
using System.Collections.Generic;

namespace SmartWasteCollectionSystem.Models;

public partial class DayOfWeek
{
    public Guid DayOfWeekId { get; set; }

    public string Day { get; set; } = null!;

    public virtual ICollection<GarbageCollectionSchedule> GarbageCollectionSchedules { get; set; } = new List<GarbageCollectionSchedule>();
}
