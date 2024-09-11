namespace IncidentRecorder.DTOs.Incident
{
    public class IncidentDetailsDTO
    {
        public int Id { get; set; }
        public required string DiseaseName { get; set; }
        public required string DiseaseDescription { get; set; }
        public string? PatientName { get; set; }
        public DateTime? PatientDateOfBirth { get; set; }
        public string? PatientContactInfo { get; set; }
        public string? Location { get; set; }
        public DateTime DateReported { get; set; }
        public List<string> Symptoms { get; set; } = [];
    }
}