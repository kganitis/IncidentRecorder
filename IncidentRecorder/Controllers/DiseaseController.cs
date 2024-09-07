using Microsoft.AspNetCore.Mvc;
using IncidentRecorder.Data;
using IncidentRecorder.Models;
using IncidentRecorder.DTOs.Disease;
using Microsoft.EntityFrameworkCore;

namespace IncidentRecorder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiseaseController : ControllerBase
    {
        private readonly IncidentContext _context;

        public DiseaseController(IncidentContext context)
        {
            _context = context;
        }

        // Get all diseases
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DiseaseDTO>>> GetDiseases()
        {
            var diseases = await _context.Diseases.ToListAsync();

            // Map entities to DTOs
            var diseaseDtos = diseases.Select(d => new DiseaseDTO
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description
            }).ToList();

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

            // Map entity to DTO
            var diseaseDto = new DiseaseDTO
            {
                Id = disease.Id,
                Name = disease.Name,
                Description = disease.Description
            };

            return Ok(diseaseDto);
        }

        // Create a new disease
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<DiseaseDTO>> PostDisease([FromBody] DiseaseDTO diseaseDto)
        {
            var disease = new Disease
            {
                Name = diseaseDto.Name,
                Description = diseaseDto.Description
            };

            _context.Diseases.Add(disease);
            await _context.SaveChangesAsync();

            diseaseDto.Id = disease.Id;  // Set the new ID for the response

            return CreatedAtAction(nameof(GetDisease), new { id = diseaseDto.Id }, diseaseDto);
        }

        // Update an existing disease
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutDisease(int id, [FromBody] DiseaseUpdateDTO diseaseDto)
        {
            var disease = await _context.Diseases.FindAsync(id);

            if (disease == null)
            {
                return NotFound();
            }

            // Update only provided fields
            if (!string.IsNullOrEmpty(diseaseDto.Name))
            {
                disease.Name = diseaseDto.Name;
            }

            if (!string.IsNullOrEmpty(diseaseDto.Description))
            {
                disease.Description = diseaseDto.Description;
            }

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
                else
                {
                    throw;
                }
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

        private bool DiseaseExists(int id)
        {
            return _context.Diseases.Any(e => e.Id == id);
        }
    }
}
