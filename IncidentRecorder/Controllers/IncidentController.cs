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
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IncidentReadDTO>> PostIncident([FromBody] IncidentCreateDTO incidentCreateDto)
        {
            // Validate DiseaseId
            if (incidentCreateDto.DiseaseId == 0)
            {
                return BadRequest("DiseaseId is required.");
            }
            if (!await _context.Diseases.AnyAsync(d => d.Id == incidentCreateDto.DiseaseId))
            {
                return NotFound("DiseaseId must exist.");
            }
            
            // Validate optional foreign keys (PatientId, LocationId)
            if (incidentCreateDto.PatientId.HasValue && !await _context.Patients.AnyAsync(p => p.Id == incidentCreateDto.PatientId.Value))
            {
                return NotFound("PatientId must exist.");
            }

            if (incidentCreateDto.LocationId.HasValue && !await _context.Locations.AnyAsync(l => l.Id == incidentCreateDto.LocationId.Value))
            {
                return NotFound("LocationId must exist.");
            }

            // Validate SymptomIds
            if (incidentCreateDto.SymptomIds != null && incidentCreateDto.SymptomIds.Any())
            {
                var validSymptomIds = await _context.Symptoms.Select(s => s.Id).ToListAsync();
                var invalidSymptomIds = incidentCreateDto.SymptomIds.Except(validSymptomIds).ToList();

                if (invalidSymptomIds.Any())
                {
                    return NotFound($"SymptomIds must exist: {string.Join(", ", invalidSymptomIds)}");
                }
            }

            // Create the incident
            var incident = new Incident
            {
                DiseaseId = incidentCreateDto.DiseaseId,
                Disease = (await _context.Diseases.FindAsync(incidentCreateDto.DiseaseId))!,
                PatientId = incidentCreateDto.PatientId,
                LocationId = incidentCreateDto.LocationId,
                DateReported = incidentCreateDto.DateReported ?? DateTime.Now,
                Symptoms = incidentCreateDto.SymptomIds != null && incidentCreateDto.SymptomIds.Any()
                    ? await _context.Symptoms.Where(s => incidentCreateDto.SymptomIds.Contains(s.Id)).ToListAsync()
                    : []
            };

            _context.Incidents.Add(incident);
            await _context.SaveChangesAsync();

            // Map the created incident to a Read DTO
            Patient? patient = await _context.Patients.FindAsync(incident.PatientId);
            Location? location = await _context.Locations.FindAsync(incident.LocationId);
            var incidentReadDto = new IncidentReadDTO
            {
                Id = incident.Id,
                DiseaseName = incident.Disease.Name,
                PatientName = patient != null ? $"{patient.FirstName} {patient.LastName}" : "Unknown",
                Location = location != null ? $"{location.City}, {location.Country}" : "Unknown",
                DateReported = incident.DateReported,
                Symptoms = incident.Symptoms.Select(s => s.Name).ToList()
            };

            return CreatedAtAction(nameof(GetIncident), new { id = incident.Id }, incidentReadDto);
        }

        // Update incident
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutIncident(int id, [FromBody] IncidentUpdateDTO incidentUpdateDto)
        {
            // Validate foreign keys exist
            if (incidentUpdateDto.DiseaseId.HasValue && !await _context.Diseases.AnyAsync(d => d.Id == incidentUpdateDto.DiseaseId))
            {
                return NotFound("DisaseId must exist.");
            }
            if (incidentUpdateDto.PatientId.HasValue && !await _context.Patients.AnyAsync(p => p.Id == incidentUpdateDto.PatientId.Value))
            {
                return NotFound("PatientId must exist.");
            }
            if (incidentUpdateDto.LocationId.HasValue && !await _context.Locations.AnyAsync(l => l.Id == incidentUpdateDto.LocationId.Value))
            {
                return NotFound("LocationId must exist.");
            }
            if (incidentUpdateDto.SymptomIds?.Any() == true)
            {
                var invalidSymptomIds = incidentUpdateDto.SymptomIds.Except(await _context.Symptoms.Select(s => s.Id).ToListAsync()).ToList();
                if (invalidSymptomIds.Count != 0)
                {
                    return NotFound($"SymptomIds must exist: {string.Join(", ", invalidSymptomIds)}");
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

        private bool IncidentExists(int id)
        {
            return _context.Incidents.Any(e => e.Id == id);
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
    }
}
