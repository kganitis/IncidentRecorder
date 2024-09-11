using System.ComponentModel.DataAnnotations;

namespace IncidentRecorder.DTOs.Incident
{
    public class IncidentUpdateDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "DiseaseId must be a positive integer.")]
        public int? DiseaseId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "PatientId must be a positive integer.")]
        public int? PatientId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "LocationId must be a positive integer.")]
        public int? LocationId { get; set; }

        public DateTime? DateReported { get; set; }

        public List<int>? SymptomIds { get; set; }
    }
}
