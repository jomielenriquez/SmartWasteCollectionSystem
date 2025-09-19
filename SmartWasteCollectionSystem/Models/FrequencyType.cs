using System;
using System.Collections.Generic;

namespace SmartWasteCollectionSystem.Models;

public partial class FrequencyType
{
    public Guid FrequencyTypeId { get; set; }

    public string FrequencyName { get; set; } = null!;

    public virtual ICollection<GarbageCollectionSchedule> GarbageCollectionSchedules { get; set; } = new List<GarbageCollectionSchedule>();
}
