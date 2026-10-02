namespace Countries.Application.DTOs.Cities.Request
{
    public class CreateCityDto
    {
        public string Name { get; set; } = string.Empty;
        public int CountryId { get; set; }
    }
}
