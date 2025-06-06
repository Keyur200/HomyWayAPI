using System;
using System.Collections.Generic;

namespace HomyWayAPI.Models;

public partial class Wishlist
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int PropertyId { get; set; }

    public virtual PropertyTbl Property { get; set; } = null!;

    public virtual User? User { get; set; }
}
