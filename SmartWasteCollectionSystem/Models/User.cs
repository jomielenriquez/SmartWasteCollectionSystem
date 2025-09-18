using System;
using System.Collections.Generic;

namespace SmartWasteCollectionSystem.Models;

public partial class User
{
    public Guid UserId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string LotNumber { get; set; } = null!;

    public string BlockNumber { get; set; } = null!;

    public string? StreetName { get; set; }

    public string ContactNumber { get; set; } = null!;

    public DateTime MoveInDate { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public Guid UserRoleId { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public virtual UserRole UserRole { get; set; } = null!;
}
