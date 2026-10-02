namespace Countries.Domain.Entities
{
    public class City
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int CountryId { get; set; }
        public bool IsDeleted { get; set; } = false;


        public Country Country { get; set; } = null!;
    }
}
