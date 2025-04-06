namespace HomyWayAPI.DTO
{
    public class PropertyDTO
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


        public List<IFormFile> files { get; set; }
    }
}
