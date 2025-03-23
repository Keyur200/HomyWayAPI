using System;
using System.Collections.Generic;

namespace HomyWayAPI.Models;

public partial class PropertyCategoryTbl
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public virtual ICollection<PropertyTbl> PropertyTbls { get; set; } = new List<PropertyTbl>();
}
