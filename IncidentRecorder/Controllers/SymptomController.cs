using Microsoft.AspNetCore.Mvc;
using IncidentRecorder.Data;
using IncidentRecorder.Models;
using IncidentRecorder.DTOs.Symptom;
using Microsoft.EntityFrameworkCore;

namespace IncidentRecorder.Controllers
{
    /// <summary>
    /// API controller for managing symptoms.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SymptomController(IncidentContext context) : ControllerBase
    {
        private readonly IncidentContext _context = context;

        /// <summary>
        /// Retrieves all symptoms.
        /// </summary>
        /// <returns>A list of symptom DTOs.</returns>
        /// <response code="200">Returns the list of symptoms.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<SymptomDTO>>> GetSymptoms()
        {
            var symptoms = await _context.Symptoms.ToListAsync();
            var symptomDtos = symptoms.Select(MapToDto).ToList();
            return Ok(symptomDtos);
        }

        /// <summary>
        /// Retrieves a symptom by its ID.
        /// </summary>
        /// <param name="id">The ID of the symptom to retrieve.</param>
        /// <returns>A symptom DTO.</returns>
        /// <response code="200">Returns the requested symptom.</response>
        /// <response code="404">If the symptom is not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SymptomDTO>> GetSymptom(int id)
        {
            var symptom = await _context.Symptoms.FindAsync(id);
            if (symptom == null)
            {
                return NotFound();
            }

            return Ok(MapToDto(symptom));
        }

        /// <summary>
        /// Creates a new symptom.
        /// </summary>
        /// <param name="symptomDto">The symptom details to create.</param>
        /// <returns>The created symptom DTO.</returns>
        /// <response code="201">Returns the newly created symptom.</response>
        /// <response code="409">If a symptom with the same name already exists.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<SymptomDTO>> PostSymptom([FromBody] SymptomCreateDTO symptomDto)
        {
            // Check if a symptom with the same name already exists
            if (await _context.Symptoms.AnyAsync(s => s.Name == symptomDto.Name))
            {
                return Conflict("A symptom with the same name already exists.");
            }

            var symptom = new Symptom
            {
                Name = symptomDto.Name,
                Description = symptomDto.Description
            };

            _context.Symptoms.Add(symptom);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSymptom), new { id = symptom.Id }, MapToDto(symptom));
        }

        /// <summary>
        /// Updates an existing symptom.
        /// </summary>
        /// <param name="id">The ID of the symptom to update.</param>
        /// <param name="symptomDto">The updated symptom details.</param>
        /// <response code="204">No content, the symptom was successfully updated.</response>
        /// <response code="404">If the symptom is not found.</response>
        /// <response code="409">If a symptom with the same name already exists, excluding the current symptom.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> PutSymptom(int id, [FromBody] SymptomUpdateDTO symptomDto)
        {
            // Check if a symptom with the same name already exists, excluding the current symptom
            if (await _context.Symptoms.AnyAsync(s => s.Name == symptomDto.Name && s.Id != id))
            {
                return Conflict("A symptom with the same name already exists.");
            }

            var symptom = await _context.Symptoms.FindAsync(id);
            if (symptom == null)
            {
                return NotFound();
            }

            // Update only provided fields
            symptom.Name = symptomDto.Name ?? symptom.Name;
            symptom.Description = symptomDto.Description ?? symptom.Description;

            _context.Entry(symptom).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SymptomExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        /// <summary>
        /// Deletes a symptom by its ID.
        /// </summary>
        /// <param name="id">The ID of the symptom to delete.</param>
        /// <response code="204">The symptom was successfully deleted.</response>
        /// <response code="404">If the symptom is not found.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteSymptom(int id)
        {
            var symptom = await _context.Symptoms.FindAsync(id);
            if (symptom == null)
            {
                return NotFound();
            }

            _context.Symptoms.Remove(symptom);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SymptomExists(int id)
        {
            return _context.Symptoms.Any(e => e.Id == id);
        }

        private SymptomDTO MapToDto(Symptom symptom) => new()
        {
            Id = symptom.Id,
            Name = symptom.Name,
            Description = symptom.Description
        };
    }
}
