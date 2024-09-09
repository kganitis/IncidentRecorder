using IncidentRecorder.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IncidentRecorder.Data;
using IncidentRecorder.DTOs.Incident;

namespace IncidentRecorder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IncidentController : ControllerBase
    {
        private readonly IncidentContext _context;

        public IncidentController(IncidentContext context)
        {
            _context = context;
        }

        // Get all incidents with DTOs
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<IncidentReadDTO>>> GetIncidents()
        {
            var incidents = await _context.Incidents
                .Include(i => i.Disease)
                .Include(i => i.Patient)
                .Include(i => i.Location)
                .Include(i => i.Symptoms)
                .ToListAsync();

            var incidentDtos = incidents.Select(incident => new IncidentReadDTO
            {
                Id = incident.Id,
                DiseaseName = incident.Disease.Name,
                PatientName = $"{incident.Patient?.FirstName} {incident.Patient?.LastName}",
                Location = $"{incident.Location?.City}, {incident.Location?.Country}",
                DateReported = incident.DateReported,
                Symptoms = incident.Symptoms.Select(s => s.Name).ToList()
            }).ToList();

            return Ok(incidentDtos);
        }

        // Get a specific incident by id with DTO
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IncidentReadDTO>> GetIncident(int id)
        {
            var incident = await _context.Incidents
                .Include(i => i.Disease)
                .Include(i => i.Patient)
                .Include(i => i.Location)
                .Include(i => i.Symptoms)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (incident == null)
            {
                return NotFound();
            }

            var incidentDto = new IncidentReadDTO
            {
                Id = incident.Id,
                DiseaseName = incident.Disease.Name,
                PatientName = $"{incident.Patient?.FirstName} {incident.Patient?.LastName}",
                Location = $"{incident.Location?.City}, {incident.Location?.Country}",
                DateReported = incident.DateReported,
                Symptoms = incident.Symptoms.Select(s => s.Name).ToList()
            };

            return Ok(incidentDto);
        }

        // Create a new incident
        [HttpPost("create")]
        public async Task<ActionResult<IncidentReadDTO>> PostIncident([FromBody] IncidentCreateDTO incidentCreateDto)
        {
            // Validate DiseaseId
            if (incidentCreateDto.DiseaseId == 0 || !await _context.Diseases.AnyAsync(d => d.Id == incidentCreateDto.DiseaseId))
            {
                return BadRequest("DiseaseId is required and must exist.");
            }

            // Validate optional foreign keys (PatientId, LocationId)
            if (incidentCreateDto.PatientId.HasValue && !await _context.Patients.AnyAsync(p => p.Id == incidentCreateDto.PatientId.Value))
            {
                return BadRequest("Invalid PatientId.");
            }

            if (incidentCreateDto.LocationId.HasValue && !await _context.Locations.AnyAsync(l => l.Id == incidentCreateDto.LocationId.Value))
            {
                return BadRequest("Invalid LocationId.");
            }

            // Validate SymptomIds
            if (incidentCreateDto.SymptomIds != null && incidentCreateDto.SymptomIds.Any())
            {
                var validSymptomIds = await _context.Symptoms.Select(s => s.Id).ToListAsync();
                var invalidSymptomIds = incidentCreateDto.SymptomIds.Except(validSymptomIds).ToList();

                if (invalidSymptomIds.Any())
                {
                    return BadRequest($"Invalid SymptomIds: {string.Join(", ", invalidSymptomIds)}");
                }
            }

            // Create the incident
            var incident = new Incident
            {
                DiseaseId = incidentCreateDto.DiseaseId,
                PatientId = incidentCreateDto.PatientId,
                LocationId = incidentCreateDto.LocationId,
                DateReported = incidentCreateDto.DateReported ?? DateTime.Now,
                Symptoms = incidentCreateDto.SymptomIds != null && incidentCreateDto.SymptomIds.Any()
                    ? await _context.Symptoms.Where(s => incidentCreateDto.SymptomIds.Contains(s.Id)).ToListAsync()
                    : new List<Symptom>()
            };

            _context.Incidents.Add(incident);
            await _context.SaveChangesAsync();

            // Map the created incident to a Read DTO
            var incidentReadDto = new IncidentReadDTO
            {
                Id = incident.Id,
                DiseaseName = (await _context.Diseases.FindAsync(incident.DiseaseId))?.Name,
                PatientName = (await _context.Patients.FindAsync(incident.PatientId)) != null
                    ? $"{(await _context.Patients.FindAsync(incident.PatientId)).FirstName} {(await _context.Patients.FindAsync(incident.PatientId)).LastName}"
                    : "Unknown",
                Location = (await _context.Locations.FindAsync(incident.LocationId)) != null
                    ? $"{(await _context.Locations.FindAsync(incident.LocationId)).City}, {(await _context.Locations.FindAsync(incident.LocationId)).Country}"
                    : "Unknown",
                DateReported = incident.DateReported,
                Symptoms = incident.Symptoms.Select(s => s.Name).ToList()
            };

            return CreatedAtAction(nameof(GetIncident), new { id = incident.Id }, incidentReadDto);
        }

        // Update incident
        [HttpPut("{id}")]
        public async Task<IActionResult> PutIncident(int id, [FromBody] IncidentUpdateDTO incidentUpdateDto)
        {
            // Validate foreign keys exist
            if (incidentUpdateDto.DiseaseId.HasValue && !await _context.Diseases.AnyAsync(d => d.Id == incidentUpdateDto.DiseaseId))
            {
                return BadRequest("Invalid DiseaseId.");
            }
            if (incidentUpdateDto.PatientId.HasValue && !await _context.Patients.AnyAsync(p => p.Id == incidentUpdateDto.PatientId.Value))
            {
                return BadRequest("Invalid PatientId.");
            }
            if (incidentUpdateDto.LocationId.HasValue && !await _context.Locations.AnyAsync(l => l.Id == incidentUpdateDto.LocationId.Value))
            {
                return BadRequest("Invalid LocationId.");
            }
            if (incidentUpdateDto.SymptomIds?.Any() == true)
            {
                var invalidSymptomIds = incidentUpdateDto.SymptomIds.Except(await _context.Symptoms.Select(s => s.Id).ToListAsync()).ToList();
                if (invalidSymptomIds.Any())
                {
                    return BadRequest($"Invalid SymptomIds: {string.Join(", ", invalidSymptomIds)}");
                }
            }

            // Find the incident
            var incident = await _context.Incidents
                .Include(i => i.Symptoms)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (incident == null)
            {
                return NotFound();
            }

            // Update fields from DTO
            incident.DiseaseId = incidentUpdateDto.DiseaseId ?? incident.DiseaseId;
            incident.PatientId = incidentUpdateDto.PatientId ?? incident.PatientId;
            incident.LocationId = incidentUpdateDto.LocationId ?? incident.LocationId;
            incident.DateReported = incidentUpdateDto.DateReported ?? incident.DateReported;

            // Update symptoms if provided
            if (incidentUpdateDto.SymptomIds != null)
            {
                incident.Symptoms = await _context.Symptoms
                    .Where(s => incidentUpdateDto.SymptomIds.Contains(s.Id))
                    .ToListAsync();
            }

            _context.Entry(incident).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IncidentExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }


        // Delete an incident
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIncident(int id)
        {
            var incident = await _context.Incidents.FindAsync(id);
            if (incident == null)
            {
                return NotFound();
            }

            _context.Incidents.Remove(incident);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool IncidentExists(int id)
        {
            return _context.Incidents.Any(e => e.Id == id);
        }

        // Get all incidents (lightweight version for listing)
        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<IncidentListDTO>>> GetIncidentsList()
        {
            var incidents = await _context.Incidents
                .Include(i => i.Disease)
                .Include(i => i.Patient)
                .Include(i => i.Location)
                .ToListAsync();

            var incidentDtos = incidents.Select(incident => new IncidentListDTO
            {
                Id = incident.Id,
                DiseaseName = incident.Disease.Name,
                PatientName = $"{incident.Patient?.FirstName} {incident.Patient?.LastName}",
                Location = $"{incident.Location?.City}, {incident.Location?.Country}",
                DateReported = incident.DateReported
            }).ToList();

            return Ok(incidentDtos);
        }

        // Get incident details (full version)
        [HttpGet("details/{id}")]
        public async Task<ActionResult<IncidentDetailsDTO>> GetIncidentDetails(int id)
        {
            var incident = await _context.Incidents
                .Include(i => i.Disease)
                .Include(i => i.Patient)
                .Include(i => i.Location)
                .Include(i => i.Symptoms)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (incident == null)
            {
                return NotFound();
            }

            var incidentDto = new IncidentDetailsDTO
            {
                Id = incident.Id,
                DiseaseName = incident.Disease.Name,
                DiseaseDescription = incident.Disease.Description,
                PatientName = $"{incident.Patient?.FirstName} {incident.Patient?.LastName}",
                PatientDateOfBirth = incident.Patient.DateOfBirth,
                PatientContactInfo = incident.Patient.ContactInfo,
                Location = $"{incident.Location?.City}, {incident.Location?.Country}",
                DateReported = incident.DateReported,
                Symptoms = incident.Symptoms.Select(s => s.Name).ToList()
            };

            return Ok(incidentDto);
        }

    }

}
