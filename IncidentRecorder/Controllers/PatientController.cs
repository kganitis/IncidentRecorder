using Microsoft.AspNetCore.Mvc;
using IncidentRecorder.Data;
using IncidentRecorder.Models;
using IncidentRecorder.DTOs.Patient;
using Microsoft.EntityFrameworkCore;

namespace IncidentRecorder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IncidentContext _context;

        public PatientController(IncidentContext context)
        {
            _context = context;
        }

        // Get all patients
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PatientDTO>>> GetPatients()
        {
            var patients = await _context.Patients.ToListAsync();

            // Map to DTOs
            var patientDtos = patients.Select(p => new PatientDTO
            {
                Id = p.Id,
                NIN = p.NIN,
                FirstName = p.FirstName,
                LastName = p.LastName,
                DateOfBirth = p.DateOfBirth,
                Gender = p.Gender,
                ContactInfo = p.ContactInfo
            }).ToList();

            return Ok(patientDtos);
        }

        // Get a single patient by id
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

            // Map to DTO
            var patientDto = new PatientDTO
            {
                Id = patient.Id,
                NIN = patient.NIN,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                DateOfBirth = patient.DateOfBirth,
                Gender = patient.Gender,
                ContactInfo = patient.ContactInfo
            };

            return Ok(patientDto);
        }

        // Create a new patient
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<PatientDTO>> PostPatient([FromBody] PatientCreateDTO patientDto)
        {
            // Check if a patient with the same NIN already exists
            if (await _context.Patients.AnyAsync(p => p.NIN == patientDto.NIN))
            {
                return BadRequest("A patient with the same NIN already exists.");
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

            // Map to DTO
            var createdPatientDto = new PatientDTO
            {
                Id = patient.Id,
                NIN = patient.NIN,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                DateOfBirth = patient.DateOfBirth,
                Gender = patient.Gender,
                ContactInfo = patient.ContactInfo
            };

            return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, createdPatientDto);
        }

        // Update an existing patient
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutPatient(int id, [FromBody] PatientUpdateDTO patientDto)
        {
            // Check if a patient with the same NIN already exists
            if (await _context.Patients.AnyAsync(p => p.NIN == patientDto.NIN))
            {
                return BadRequest("A patient with the same NIN already exists.");
            }

            var patient = await _context.Patients.FindAsync(id);

            if (patient == null)
            {
                return NotFound();
            }

            // Update only provided fields
            if (!string.IsNullOrEmpty(patientDto.NIN))
            {
                patient.NIN = patientDto.NIN;
            }

            if (!string.IsNullOrEmpty(patientDto.FirstName))
            {
                patient.FirstName = patientDto.FirstName;
            }

            if (!string.IsNullOrEmpty(patientDto.LastName))
            {
                patient.LastName = patientDto.LastName;
            }

            if (patientDto.DateOfBirth.HasValue)
            {
                patient.DateOfBirth = patientDto.DateOfBirth.Value;
            }

            if (!string.IsNullOrEmpty(patientDto.Gender))
            {
                patient.Gender = patientDto.Gender;
            }

            if (!string.IsNullOrEmpty(patientDto.ContactInfo))
            {
                patient.ContactInfo = patientDto.ContactInfo;
            }

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
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // Delete a patient
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
    }
}
