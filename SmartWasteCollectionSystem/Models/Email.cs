using System;
using System.Collections.Generic;

namespace SmartWasteCollectionSystem.Models;

public partial class Email
{
    public Guid EmailId { get; set; }

    public string Title { get; set; } = null!;

    public string Message { get; set; } = null!;

    public string Recipients { get; set; } = null!;

    public bool IsSent { get; set; }

    public DateTime? SentDate { get; set; }

    public DateTime CreatedDate { get; set; }
}
