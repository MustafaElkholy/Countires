namespace Countries.Application.DTOs.Cities.Request
{
    public class UpdateCityDto
    {
        public string Name { get; set; } = string.Empty;
        public int CountryId { get; set; }
    }
}
