namespace Countries.Application.DTOs.Countries.Request
{
    public class CreateCountryDto
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }
}
