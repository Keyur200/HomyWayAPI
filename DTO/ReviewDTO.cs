namespace HomyWayAPI.DTO
{
    public class ReviewDTO
    {
        public int Id { get; set; }

        public int? Rating { get; set; }

        public string? Review1 { get; set; }

        public int? UserId { get; set; }

        public int? PropertyId { get; set; }
    }
}
