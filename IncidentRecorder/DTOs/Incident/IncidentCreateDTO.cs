namespace IncidentRecorder.DTOs.Incident
{
    public class IncidentCreateDTO
    {
        // Disease is required
        public required int DiseaseId { get; set; }

        // Optional fields with validation
        public int? PatientId { get; set; }

        public int? LocationId { get; set; }

        public DateTime? DateReported { get; set; } = DateTime.Now;

        public List<int> SymptomIds { get; set; } = [];
    }
}