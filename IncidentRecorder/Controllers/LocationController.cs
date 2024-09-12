using Microsoft.AspNetCore.Mvc;
using IncidentRecorder.Data;
using IncidentRecorder.Models;
using IncidentRecorder.DTOs.Location;
using Microsoft.EntityFrameworkCore;

namespace IncidentRecorder.Controllers
{
    /// <summary>
    /// API dontroller for managing locations.
    /// </summary>
    /// <param name="context"></param>
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController(IncidentContext context) : ControllerBase
    {
        private readonly IncidentContext _context = context;

        /// <summary>
        /// Retrieves all locations from the system.
        /// </summary>
        /// <returns>A list of location DTOs.</returns>
        /// <response code="200">Returns the list of locations.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<LocationDTO>>> GetLocations()
        {
            var locations = await _context.Locations.ToListAsync();
            var locationDtos = locations.Select(MapToDto).ToList();
            return Ok(locationDtos);
        }

        /// <summary>
        /// Retrieves a specific location by its ID.
        /// </summary>
        /// <param name="id">The ID of the location to retrieve.</param>
        /// <returns>A location DTO.</returns>
        /// <response code="200">Returns the location.</response>
        /// <response code="404">If the location is not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LocationDTO>> GetLocation(int id)
        {
            var location = await _context.Locations.FindAsync(id);
            if (location == null)
            {
                return NotFound();
            }
            return Ok(MapToDto(location));
        }

        /// <summary>
        /// Creates a new location in the system.
        /// </summary>
        /// <param name="locationDto">The details of the location to create.</param>
        /// <returns>The created location DTO.</returns>
        /// <response code="201">Returns the newly created location.</response>
        /// <response code="409">If a location with the same city and country already exists.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<LocationDTO>> PostLocation([FromBody] LocationCreateDTO locationDto)
        {
            if (await _context.Locations.AnyAsync(l => l.City == locationDto.City && l.Country == locationDto.Country))
            {
                return Conflict("A location with the same city and country name already exists.");
            }

            var location = new Location
            {
                City = locationDto.City,
                Country = locationDto.Country
            };

            _context.Locations.Add(location);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLocation), new { id = location.Id }, MapToDto(location));
        }

        /// <summary>
        /// Updates an existing location.
        /// </summary>
        /// <param name="id">The ID of the location to update.</param>
        /// <param name="locationDto">The updated location details.</param>
        /// <response code="204">No content, the location was successfully updated.</response>
        /// <response code="404">If the location is not found.</response>
        /// <response code="409">If a location with the same city and country already exists.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> PutLocation(int id, [FromBody] LocationUpdateDTO locationDto)
        {
            if (await _context.Locations.AnyAsync(l => l.City == locationDto.City && l.Country == locationDto.Country && l.Id != id))
            {
                return Conflict("A location with the same city and country name already exists.");
            }

            var location = await _context.Locations.FindAsync(id);
            if (location == null)
            {
                return NotFound();
            }

            location.City = !string.IsNullOrEmpty(locationDto.City) ? locationDto.City : location.City;
            location.Country = !string.IsNullOrEmpty(locationDto.Country) ? locationDto.Country : location.Country;

            _context.Entry(location).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LocationExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        /// <summary>
        /// Deletes a specific location by ID.
        /// </summary>
        /// <param name="id">The ID of the location to delete.</param>
        /// <response code="204">The location was successfully deleted.</response>
        /// <response code="404">If the location is not found.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteLocation(int id)
        {
            var location = await _context.Locations.FindAsync(id);
            if (location == null)
            {
                return NotFound();
            }

            _context.Locations.Remove(location);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LocationExists(int id)
        {
            return _context.Locations.Any(l => l.Id == id);
        }

        private LocationDTO MapToDto(Location location) => new()
        {
            Id = location.Id,
            City = location.City,
            Country = location.Country
        };
    }
}
