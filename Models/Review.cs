using System;
using System.Collections.Generic;

namespace HomyWayAPI.Models;

public partial class Review
{
    public int Id { get; set; }

    public int? Rating { get; set; }

    public string? Review1 { get; set; }

    public int? UserId { get; set; }

    public int? PropertyId { get; set; }

    public virtual PropertyTbl? Property { get; set; }

    public virtual User? User { get; set; }
}
