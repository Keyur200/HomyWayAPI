using System;
using System.Collections.Generic;

namespace HomyWayAPI.Models;

public partial class PropertyTbl
{
    public int PropertyId { get; set; }

    public int HostId { get; set; }

    public string PropertyName { get; set; } = null!;

    public string? PropertyDescription { get; set; }

    public string PropertyAdderss { get; set; } = null!;

    public string PropertyCity { get; set; } = null!;

    public string PropertyState { get; set; } = null!;

    public string PropertyCountry { get; set; } = null!;

    public int MaxGuests { get; set; }

    public int BedRoom { get; set; }

    public int Bed { get; set; }

    public int Bathroom { get; set; }

    public string Status { get; set; } = null!;

    public decimal PropertyPrice { get; set; }

    public int CategoryId { get; set; }

    public string? Images { get; set; }

    public string? Latitude { get; set; }

    public string? Longitude { get; set; }

    public string? Amenities { get; set; }

    public string? SlugName { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual PropertyCategoryTbl Category { get; set; } = null!;

    public virtual User Host { get; set; } = null!;

    public virtual ICollection<Image> ImagesNavigation { get; set; } = new List<Image>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
