namespace IncidentRecorder.DTOs.Incident
{
    public class IncidentUpdateDTO
    {
        public int? DiseaseId { get; set; }
        public int? PatientId { get; set; }
        public int? LocationId { get; set; }
        public DateTime? DateReported { get; set; }
        public List<int>? SymptomIds { get; set; }
    }
}
