namespace IncidentRecorder.DTOs.Incident
{
    public class IncidentCreateDTO
    {
        public int DiseaseId { get; set; }
        public int PatientId { get; set; }
        public int LocationId { get; set; }
        public DateTime DateReported { get; set; }
        public List<int> SymptomIds { get; set; }
    }
}
