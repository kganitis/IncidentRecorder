namespace IncidentRecorder.DTOs.Incident
{
    public class IncidentListDTO
    {
        public int Id { get; set; }
        public required string DiseaseName { get; set; }
        public string? PatientName { get; set; }
        public string? Location { get; set; }
        public DateTime DateReported { get; set; }
    }
}