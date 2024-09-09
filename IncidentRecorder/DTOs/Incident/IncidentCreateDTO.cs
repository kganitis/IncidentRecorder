namespace IncidentRecorder.DTOs.Incident
{
    public class IncidentCreateDTO
    {
        // Disease is required
        public int DiseaseId { get; set; }

        // Optional fields with validation
        public int? PatientId { get; set; }

        public int? LocationId { get; set; }

        public DateTime? DateReported { get; set; }

        public List<int> SymptomIds { get; set; } = new List<int>();
    }
}