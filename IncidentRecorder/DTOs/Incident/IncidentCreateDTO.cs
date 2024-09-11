using System.ComponentModel.DataAnnotations;

namespace IncidentRecorder.DTOs.Incident
{
    public class IncidentCreateDTO
    {
        // Disease is required
        [Required(ErrorMessage = "DiseaseId is required.")]
        public required int DiseaseId { get; set; }

        // Optional fields with validation
        [Range(1, int.MaxValue, ErrorMessage = "PatientId must be a positive integer.")]
        public int? PatientId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "LocationId must be a positive integer.")]
        public int? LocationId { get; set; }

        public DateTime? DateReported { get; set; } = DateTime.Now;

        public List<int> SymptomIds { get; set; } = [];
    }
}