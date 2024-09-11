using Microsoft.AspNetCore.Mvc;
using IncidentRecorder.Data;
using IncidentRecorder.Models;
using IncidentRecorder.DTOs.Disease;
using Microsoft.EntityFrameworkCore;

namespace IncidentRecorder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiseaseController(IncidentContext context) : ControllerBase
    {
        private readonly IncidentContext _context = context;

        // Get all diseases
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DiseaseDTO>>> GetDiseases()
        {
            var diseases = await _context.Diseases.ToListAsync();
            var diseaseDtos = diseases.Select(MapToDto).ToList();
            return Ok(diseaseDtos);
        }

        // Get a single disease by id
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DiseaseDTO>> GetDisease(int id)
        {
            var disease = await _context.Diseases.FindAsync(id);
            if (disease == null)
            {
                return NotFound();
            }
            return Ok(MapToDto(disease));
        }

        // Create a new disease
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<DiseaseDTO>> PostDisease([FromBody] DiseaseCreateDTO diseaseDto)
        {
            if (await _context.Diseases.AnyAsync(d => d.Name == diseaseDto.Name))
            {
                return Conflict("A disease with the same name already exists.");
            }

            var disease = new Disease
            {
                Name = diseaseDto.Name,
                Description = diseaseDto.Description
            };

            _context.Diseases.Add(disease);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDisease), new { id = disease.Id }, MapToDto(disease));
        }

        // Update an existing disease
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> PutDisease(int id, [FromBody] DiseaseUpdateDTO diseaseDto)
        {
            if (await _context.Diseases.AnyAsync(d => d.Name == diseaseDto.Name && d.Id != id))
            {
                return Conflict("A disease with the same name already exists.");
            }

            var disease = await _context.Diseases.FindAsync(id);
            if (disease == null)
            {
                return NotFound();
            }

            disease.Name = !string.IsNullOrEmpty(diseaseDto.Name) ? diseaseDto.Name : disease.Name;
            disease.Description = !string.IsNullOrEmpty(diseaseDto.Description) ? diseaseDto.Description : disease.Description;

            _context.Entry(disease).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DiseaseExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // Delete a disease
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteDisease(int id)
        {
            var disease = await _context.Diseases.FindAsync(id);
            if (disease == null)
            {
                return NotFound();
            }

            _context.Diseases.Remove(disease);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DiseaseExists(int id) => _context.Diseases.Any(e => e.Id == id);

        private DiseaseDTO MapToDto(Disease disease) => new()
        {
            Id = disease.Id,
            Name = disease.Name,
            Description = disease.Description
        };
    }
}
