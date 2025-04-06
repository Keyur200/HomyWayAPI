using System;
using System.Collections.Generic;

namespace HomyWayAPI.Models;

public partial class Image
{
    public int Id { get; set; }

    public string? ImageUrl { get; set; }

    public int PropertId { get; set; }

    public virtual PropertyTbl Propert { get; set; } = null!;
}
