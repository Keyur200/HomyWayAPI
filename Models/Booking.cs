using System;
using System.Collections.Generic;

namespace HomyWayAPI.Models;

public partial class Booking
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int PropertyId { get; set; }

    public DateOnly Checkkin { get; set; }

    public DateOnly Checkout { get; set; }

    public int Guests { get; set; }

    public int Nights { get; set; }

    public string Name { get; set; } = null!;

    public long Phone { get; set; }

    public long Amount { get; set; }

    public DateTime CreatedAt { get; set; }

    public long? HomywayCharges { get; set; }

    public virtual PropertyTbl Property { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
