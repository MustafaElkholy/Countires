using Countries.Application.DTOs.Cities.Request;
using Countries.Application.DTOs.Cities.Response;
using Countries.Application.DTOs.Countries.Request;
using Countries.Application.DTOs.Countries.Response;
using Countries.Application.DTOs.Shared;
using Countries.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Countries.API.Controllers
{
    [Route("api/countries")]
    [ApiController]
    public class CountriesController(ICountryService countryService, ICityService cityService) : ControllerBase
    {
        [HttpPost("Create")]
        public async Task<ActionResult<ApiResponse<GetCountryDto>>> Create([FromBody] CreateCountryDto request, CancellationToken cancellationToken)
        {
            var country = await countryService.CreateAsync(
                request, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = country.Id },
                ApiResponse<GetCountryDto>.Ok(country, "Country created successfully."));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<GetCountryDto>>> GetById(int id, CancellationToken cancellationToken)
        {
            var country = await countryService.GetByIdAsync(id, cancellationToken);

            return Ok(ApiResponse<GetCountryDto>.Ok(country));
        }

        [HttpPost("search")]
        public async Task<ActionResult<ApiResponse<APIResponsePagedResult<GetCountryDto>>>> Search([FromBody] GetCountriesRequestDto request, CancellationToken cancellationToken)
        {
            var countries = await countryService.GetPagedAsync(request, cancellationToken);

            return Ok(ApiResponse<APIResponsePagedResult<GetCountryDto>>.Ok(countries));
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse<GetCountryDto>>> Update(int id, [FromBody] UpdateCountryDto request, CancellationToken cancellationToken)
        {
            var country = await countryService.UpdateAsync(id, request, cancellationToken);

            return Ok(ApiResponse<GetCountryDto>.Ok(country, "Country updated successfully."));
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<object?>>> Delete(int id, CancellationToken cancellationToken)
        {
            await countryService.DeleteAsync(id, cancellationToken);

            return Ok(ApiResponse<object?>.Ok(null, "Country deleted successfully."));
        }

        [HttpPost("cities/search")]
        public async Task<ActionResult<ApiResponse<APIResponsePagedResult<GetCityDto>>>> SearchCities([FromBody] GetCitiesByCountryRequestDto request, CancellationToken cancellationToken)
        {
            var cities = await cityService.GetByCountryIdAsync(request, cancellationToken);

            return Ok(ApiResponse<APIResponsePagedResult<GetCityDto>>.Ok(cities));
        }
    }
}
