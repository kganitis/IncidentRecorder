﻿using Microsoft.AspNetCore.Mvc;
using IncidentRecorder.Data;
using IncidentRecorder.Models;
using IncidentRecorder.DTOs.Location;
using Microsoft.EntityFrameworkCore;

namespace IncidentRecorder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController(IncidentContext context) : ControllerBase
    {
        private readonly IncidentContext _context = context;

        // Get all locations
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<LocationDTO>>> GetLocations()
        {
            var locations = await _context.Locations.ToListAsync();
            var locationDtos = locations.Select(MapToDto).ToList();
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
            return Ok(MapToDto(location));
        }

        // Create a new location
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

        // Update an existing location
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> PutLocation(int id, [FromBody] LocationUpdateDTO locationDto)
        {
            // Check for duplicate location
            if (await _context.Locations.AnyAsync(l => l.City == locationDto.City && l.Country == locationDto.Country && l.Id != id))
            {
                return Conflict("A location with the same city and country name already exists.");
            }

            var location = await _context.Locations.FindAsync(id);
            if (location == null)
            {
                return NotFound();
            }

            // Update only provided fields
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
