using System;
using System.Collections.Generic;

namespace SmartWasteCollectionSystem.Models;

public partial class UserRole
{
    public Guid UserRoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public DateTime CreatedDate { get; set; }
}
