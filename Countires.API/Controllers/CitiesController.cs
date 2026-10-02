using Countries.Application.DTOs.Cities.Request;
using Countries.Application.DTOs.Cities.Response;
using Countries.Application.DTOs.Shared;
using Countries.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Countries.API.Controllers
{
    [Route("api/cities")]
    [ApiController]
    public class CitiesController(ICityService cityService) : ControllerBase
    {
        [HttpPost("Create")]
        public async Task<ActionResult<ApiResponse<GetCityDto>>> Create([FromBody] CreateCityDto request, CancellationToken cancellationToken)
        {
            var city = await cityService.CreateAsync(request, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = city.Id },
                ApiResponse<GetCityDto>.Ok(city, "City created successfully."));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<GetCityDto>>> GetById(int id, CancellationToken cancellationToken)
        {
            var city = await cityService.GetByIdAsync(id, cancellationToken);

            return Ok(ApiResponse<GetCityDto>.Ok(city));
        }

        // Get All cities endpoint But with search by city name and country id, with pagination
        [HttpPost("search")]
        public async Task<ActionResult<ApiResponse<APIResponsePagedResult<GetCityDto>>>> Search([FromBody] GetCitiesRequestDto request, CancellationToken cancellationToken)
        {
            var cities = await cityService.GetPagedAsync(request, cancellationToken);

            return Ok(ApiResponse<APIResponsePagedResult<GetCityDto>>.Ok(cities));
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse<GetCityDto>>> Update(int id, [FromBody] UpdateCityDto request, CancellationToken cancellationToken)
        {
            var city = await cityService.UpdateAsync(
                id, request, cancellationToken);

            return Ok(ApiResponse<GetCityDto>.Ok(city, "City updated successfully."));
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<object?>>> Delete(int id, CancellationToken cancellationToken)
        {
            await cityService.DeleteAsync(id, cancellationToken);

            return Ok(ApiResponse<object?>.Ok(null, "City deleted successfully."));
        }
    }
}
