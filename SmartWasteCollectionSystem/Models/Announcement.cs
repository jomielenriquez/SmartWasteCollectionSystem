using System;
using System.Collections.Generic;

namespace SmartWasteCollectionSystem.Models;

public partial class Announcement
{
    public Guid AnnouncementId { get; set; }

    public string Title { get; set; } = null!;

    public string Message { get; set; } = null!;

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedDate { get; set; }
}
