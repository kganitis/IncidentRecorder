using Microsoft.AspNetCore.Mvc;
using IncidentRecorder.Data;
using IncidentRecorder.Models;
using IncidentRecorder.DTOs.Disease;
using Microsoft.EntityFrameworkCore;

namespace IncidentRecorder.Controllers
{
    /// <summary>
    /// API controller for managing diseases.
    /// </summary>
    /// <param name="context"></param>
    [Route("api/[controller]")]
    [ApiController]
    public class DiseaseController(IncidentContext context) : ControllerBase
    {
        private readonly IncidentContext _context = context;

        /// <summary>
        /// Gets all diseases.
        /// </summary>
        /// <returns>A list of all diseases.</returns>
        /// <response code="200">Returns the list of diseases.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DiseaseDTO>>> GetDiseases()
        {
            var diseases = await _context.Diseases.ToListAsync();
            var diseaseDtos = diseases.Select(MapToDto).ToList();
            return Ok(diseaseDtos);
        }

        /// <summary>
        /// Gets a specific disease by ID.
        /// </summary>
        /// <param name="id">The ID of the disease to retrieve.</param>
        /// <returns>The disease entity.</returns>
        /// <response code="200">Returns the disease.</response>
        /// <response code="404">If the disease is not found.</response>
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

        /// <summary>
        /// Creates a new disease.
        /// </summary>
        /// <param name="diseaseDto">The disease entity to create.</param>
        /// <returns>The newly created disease.</returns>
        /// <response code="201">Returns the newly created disease.</response>
        /// <response code="409">If a disease with the same name already exists.</response>
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

        /// <summary>
        /// Updates an existing disease.
        /// </summary>
        /// <param name="id">The ID of the disease to update.</param>
        /// <param name="diseaseDto">The updated disease data.</param>
        /// <response code="204">If the update is successful.</response>
        /// <response code="404">If the disease is not found.</response>
        /// <response code="409">If a disease with the same name already exists.</response>
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

        /// <summary>
        /// Deletes a disease by ID.
        /// </summary>
        /// <param name="id">The ID of the disease to delete.</param>
        /// <response code="204">If the deletion is successful.</response>
        /// <response code="404">If the disease is not found.</response>
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
