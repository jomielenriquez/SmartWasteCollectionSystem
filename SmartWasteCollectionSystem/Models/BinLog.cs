using System;
using System.Collections.Generic;

namespace SmartWasteCollectionSystem.Models;

public partial class BinLog
{
    public Guid BinLogId { get; set; }

    public Guid UserId { get; set; }

    public int BinStatusPercentage { get; set; }

    public DateTime CreatedDate { get; set; }

    public int? Mq3reading { get; set; }

    public virtual User User { get; set; } = null!;
}
