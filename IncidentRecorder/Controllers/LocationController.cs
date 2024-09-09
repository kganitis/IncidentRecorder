using Microsoft.AspNetCore.Mvc;
using IncidentRecorder.Data;
using IncidentRecorder.Models;
using IncidentRecorder.DTOs.Location;
using Microsoft.EntityFrameworkCore;

namespace IncidentRecorder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly IncidentContext _context;

        public LocationController(IncidentContext context)
        {
            _context = context;
        }

        // Get all locations
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<LocationDTO>>> GetLocations()
        {
            var locations = await _context.Locations.ToListAsync();

            // Map to DTOs
            var locationDtos = locations.Select(l => new LocationDTO
            {
                Id = l.Id,
                City = l.City,
                Country = l.Country
            }).ToList();

            return Ok(locationDtos);
        }

        // Get a single location by id
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

            // Map to DTO
            var locationDto = new LocationDTO
            {
                Id = location.Id,
                City = location.City,
                Country = location.Country
            };

            return Ok(locationDto);
        }

        // Create a new location
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<LocationDTO>> PostLocation([FromBody] LocationCreateDTO locationDto)
        {
            // Check if location already exists
            if (await _context.Locations.AnyAsync(l => l.City == locationDto.City && l.Country == locationDto.Country))
            {
                return BadRequest("A location with the same city and country name already exists.");
            }

            var location = new Location
            {
                City = locationDto.City,
                Country = locationDto.Country
            };

            _context.Locations.Add(location);
            await _context.SaveChangesAsync();

            // Map to DTO for response
            var createdLocationDto = new LocationDTO
            {
                Id = location.Id,
                City = location.City,
                Country = location.Country
            };

            return CreatedAtAction(nameof(GetLocation), new { id = location.Id }, createdLocationDto);
        }

        // Update an existing location
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutLocation(int id, [FromBody] LocationUpdateDTO locationDto)
        {
            // Check if location already exists
            if (await _context.Locations.AnyAsync(l => l.City == locationDto.City && l.Country == locationDto.Country))
            {
                return BadRequest("A location with the same city and country name already exists.");
            }

            var location = await _context.Locations.FindAsync(id);

            if (location == null)
            {
                return NotFound();
            }

            // Update only the provided fields
            if (!string.IsNullOrEmpty(locationDto.City))
            {
                location.City = locationDto.City;
            }

            if (!string.IsNullOrEmpty(locationDto.Country))
            {
                location.Country = locationDto.Country;
            }

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
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // Delete a location
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
            return _context.Locations.Any(e => e.Id == id);
        }
    }
}
