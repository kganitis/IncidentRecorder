using IncidentRecorder.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IncidentRecorder.Data;
using IncidentRecorder.DTOs.Incident;

namespace IncidentRecorder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IncidentController(IncidentContext context) : ControllerBase
    {
        private readonly IncidentContext _context = context;

        // Get all incidents
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<IncidentReadDTO>>> GetIncidents()
        {
            var incidents = await _context.Incidents
                .Include(i => i.Disease)
                .Include(i => i.Patient)
                .Include(i => i.Location)
                .Include(i => i.Symptoms)
                .ToListAsync();

            var incidentDtos = incidents.Select(MapToReadDto).ToList();

            return Ok(incidentDtos);
        }

        // Get a specific incident by id
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

            return Ok(MapToReadDto(incident));
        }

        // Create a new incident
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IncidentReadDTO>> PostIncident([FromBody] IncidentCreateDTO incidentCreateDto)
        {
            // Validate data
            if (incidentCreateDto.DiseaseId == 0)
            {
                return BadRequest("Invalid DiseaseId");
            }
            var validationError = await ValidateForeignKeys(incidentCreateDto.DiseaseId, incidentCreateDto.PatientId, incidentCreateDto.LocationId, incidentCreateDto.SymptomIds);
            if (validationError != null)
            {
                return NotFound(validationError);
            }

            // Create new incident
            var incident = new Incident
            {
                DiseaseId = incidentCreateDto.DiseaseId,
                Disease = (await _context.Diseases.FindAsync(incidentCreateDto.DiseaseId))!,
                PatientId = incidentCreateDto.PatientId,
                Patient = await _context.Patients.FindAsync(incidentCreateDto.PatientId),
                LocationId = incidentCreateDto.LocationId,
                Location = await _context.Locations.FindAsync(incidentCreateDto.LocationId),
                DateReported = incidentCreateDto.DateReported ?? DateTime.Now,
                Symptoms = incidentCreateDto.SymptomIds != null && incidentCreateDto.SymptomIds.Count > 0
                    ? await _context.Symptoms.Where(s => incidentCreateDto.SymptomIds.Contains(s.Id)).ToListAsync()
                    : []
            };

            // Save new incident
            _context.Incidents.Add(incident);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetIncident), new { id = incident.Id }, MapToReadDto(incident));
        }

        // Update an incident
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutIncident(int id, [FromBody] IncidentUpdateDTO incidentUpdateDto)
        {
            // Validate data
            var validationError = await ValidateForeignKeys(incidentUpdateDto.DiseaseId, incidentUpdateDto.PatientId, incidentUpdateDto.LocationId, incidentUpdateDto.SymptomIds);
            if (validationError != null)
            {
                return NotFound(validationError);
            }

            var incident = await _context.Incidents.Include(i => i.Symptoms).FirstOrDefaultAsync(i => i.Id == id);
            if (incident == null)
            {
                return NotFound();
            }

            // Update fields
            incident.DiseaseId = incidentUpdateDto.DiseaseId ?? incident.DiseaseId;
            incident.PatientId = incidentUpdateDto.PatientId ?? incident.PatientId;
            incident.LocationId = incidentUpdateDto.LocationId ?? incident.LocationId;
            incident.DateReported = incidentUpdateDto.DateReported ?? incident.DateReported;

            if (incidentUpdateDto.SymptomIds != null)
            {
                incident.Symptoms = await _context.Symptoms
                    .Where(s => incidentUpdateDto.SymptomIds.Contains(s.Id))
                    .ToListAsync();
            }

            _context.Entry(incident).State = EntityState.Modified;

            // Save changes catching concurrency exceptions
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
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        // Get all incidents (lightweight version for listing)
        [HttpGet("list")]
        [ProducesResponseType(StatusCodes.Status200OK)]
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
                PatientDateOfBirth = incident.Patient?.DateOfBirth,
                PatientContactInfo = incident.Patient?.ContactInfo,
                Location = $"{incident.Location?.City}, {incident.Location?.Country}",
                DateReported = incident.DateReported,
                Symptoms = incident.Symptoms.Select(s => s.Name).ToList()
            };

            return Ok(incidentDto);
        }

        private async Task<string?> ValidateForeignKeys(int? diseaseId, int? patientId, int? locationId, List<int>? symptomIds)
        {
            if (diseaseId.HasValue && !await _context.Diseases.AnyAsync(d => d.Id == diseaseId))
            {
                return "DisaseId must exist.";
            }

            if (patientId.HasValue && !await _context.Patients.AnyAsync(p => p.Id == patientId.Value))
            {
                return "PatientId must exist.";
            }

            if (locationId.HasValue && !await _context.Locations.AnyAsync(l => l.Id == locationId.Value))
            {
                return "LocationId must exist.";
            }

            if (symptomIds != null && symptomIds.Count > 0)
            {
                var validSymptomIds = await _context.Symptoms.Select(s => s.Id).ToListAsync();
                var invalidSymptomIds = symptomIds.Except(validSymptomIds).ToList();

                if (invalidSymptomIds.Count > 0)
                {
                    return $"SymptomIds must exist: {string.Join(", ", invalidSymptomIds)}";
                }
            }

            return null;
        }

        private bool IncidentExists(int id) => _context.Incidents.Any(e => e.Id == id);

        private IncidentReadDTO MapToReadDto(Incident incident) => new()
        {
            Id = incident.Id,
            DiseaseName = incident.Disease.Name,
            PatientName = incident.Patient != null ? $"{incident.Patient.FirstName} {incident.Patient.LastName}" : null,
            Location = incident.Location != null ? $"{incident.Location.City}, {incident.Location.Country}" : null,
            DateReported = incident.DateReported,
            Symptoms = incident.Symptoms.Select(s => s.Name).ToList()
        };
    }
}
