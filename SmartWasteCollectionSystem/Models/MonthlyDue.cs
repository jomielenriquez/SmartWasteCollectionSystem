using System;
using System.Collections.Generic;

namespace SmartWasteCollectionSystem.Models;

public partial class MonthlyDue
{
    public Guid MonthlyDueId { get; set; }

    public Guid UserId { get; set; }

    public decimal Amount { get; set; }

    public DateTime DueDate { get; set; }

    public bool IsPaid { get; set; }

    public DateTime? PaidDate { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual User User { get; set; } = null!;
}
