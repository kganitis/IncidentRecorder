using Microsoft.AspNetCore.Mvc;
using IncidentRecorder.Data;
using IncidentRecorder.Models;
using IncidentRecorder.DTOs.Patient;
using Microsoft.EntityFrameworkCore;

namespace IncidentRecorder.Controllers
{
    /// <summary>
    /// API dontroller for managing patients.
    /// </summary>
    /// <param name="context"></param>
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController(IncidentContext context) : ControllerBase
    {
        private readonly IncidentContext _context = context;

        /// <summary>
        /// Retrieves all patients from the system.
        /// </summary>
        /// <returns>A list of patient DTOs.</returns>
        /// <response code="200">Returns the list of patients.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PatientDTO>>> GetPatients()
        {
            var patients = await _context.Patients.ToListAsync();
            var patientDtos = patients.Select(MapToDto).ToList();
            return Ok(patientDtos);
        }

        /// <summary>
        /// Retrieves a specific patient by their ID.
        /// </summary>
        /// <param name="id">The ID of the patient to retrieve.</param>
        /// <returns>A patient DTO.</returns>
        /// <response code="200">Returns the requested patient.</response>
        /// <response code="404">If the patient is not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PatientDTO>> GetPatient(int id)
        {
            var patient = await _context.Patients.FindAsync(id);

            if (patient == null)
            {
                return NotFound();
            }

            return Ok(MapToDto(patient));
        }

        /// <summary>
        /// Creates a new patient.
        /// </summary>
        /// <param name="patientDto">The patient details to create.</param>
        /// <returns>The created patient DTO.</returns>
        /// <response code="201">Returns the newly created patient.</response>
        /// <response code="409">If a patient with the same NIN already exists.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<PatientDTO>> PostPatient([FromBody] PatientCreateDTO patientDto)
        {
            // Check if a patient with the same NIN already exists
            if (await _context.Patients.AnyAsync(p => p.NIN == patientDto.NIN))
            {
                return Conflict("A patient with the same NIN already exists.");
            }

            var patient = new Patient
            {
                NIN = patientDto.NIN,
                FirstName = patientDto.FirstName,
                LastName = patientDto.LastName,
                DateOfBirth = patientDto.DateOfBirth,
                Gender = patientDto.Gender,
                ContactInfo = patientDto.ContactInfo
            };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, MapToDto(patient));
        }

        /// <summary>
        /// Updates an existing patient by ID.
        /// </summary>
        /// <param name="id">The ID of the patient to update.</param>
        /// <param name="patientDto">The updated patient details.</param>
        /// <response code="204">No content, the patient was successfully updated.</response>
        /// <response code="404">If the patient is not found.</response>
        /// <response code="409">If a patient with the same NIN already exists, excluding the current patient.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> PutPatient(int id, [FromBody] PatientUpdateDTO patientDto)
        {
            // Check if a patient with the same NIN already exists, excluding the current patient
            if (await _context.Patients.AnyAsync(p => p.NIN == patientDto.NIN && p.Id != id))
            {
                return Conflict("A patient with the same NIN already exists.");
            }

            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }

            // Update only provided fields
            patient.NIN = patientDto.NIN ?? patient.NIN;
            patient.FirstName = !string.IsNullOrEmpty(patientDto.FirstName) ? patientDto.FirstName : patient.FirstName;
            patient.LastName = !string.IsNullOrEmpty(patientDto.LastName) ? patientDto.LastName : patient.LastName;
            patient.DateOfBirth = patientDto.DateOfBirth ?? patient.DateOfBirth;
            patient.Gender = !string.IsNullOrEmpty(patientDto.Gender) ? patientDto.Gender : patient.Gender;
            patient.ContactInfo = !string.IsNullOrEmpty(patientDto.ContactInfo) ? patientDto.ContactInfo : patient.ContactInfo;

            _context.Entry(patient).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PatientExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        /// <summary>
        /// Deletes a patient by ID.
        /// </summary>
        /// <param name="id">The ID of the patient to delete.</param>
        /// <response code="204">The patient was successfully deleted.</response>
        /// <response code="404">If the patient is not found.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePatient(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PatientExists(int id)
        {
            return _context.Patients.Any(e => e.Id == id);
        }

        private PatientDTO MapToDto(Patient patient) => new()
        {
            Id = patient.Id,
            NIN = patient.NIN,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            DateOfBirth = patient.DateOfBirth,
            Gender = patient.Gender,
            ContactInfo = patient.ContactInfo
        };
    }
}
