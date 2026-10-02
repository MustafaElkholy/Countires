namespace Countries.Application.DTOs.Cities.Response
{
    public class GetCityDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CountryId { get; set; }
        public string CountryName { get; set; } = string.Empty;
    }
}
