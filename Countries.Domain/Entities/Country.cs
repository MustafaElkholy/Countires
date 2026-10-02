namespace Countries.Domain.Entities
{
    public class Country
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;

        public ICollection<City> Cities { get; set; } = new List<City>();
    }
}
