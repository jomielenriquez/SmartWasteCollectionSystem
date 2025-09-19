using System;
using System.Collections.Generic;

namespace SmartWasteCollectionSystem.Models;

public partial class GarbageCollectionSchedule
{
    public Guid GarbageCollectionScheduleId { get; set; }

    public Guid DayOfWeekId { get; set; }

    public Guid FrequencyTypeId { get; set; }

    public TimeOnly CollectionTime { get; set; }

    public DateOnly? EffectiveFrom { get; set; }

    public DateOnly? EffectiveTo { get; set; }

    public bool IsActive { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual DayOfWeek DayOfWeek { get; set; } = null!;

    public virtual FrequencyType FrequencyType { get; set; } = null!;
}
