namespace IncidentRecorder.DTOs.Incident
{
    public class IncidentReadDTO
    {
        public int Id { get; set; }
        public string DiseaseName { get; set; }
        public string PatientName { get; set; }
        public string Location { get; set; }
        public DateTime DateReported { get; set; }
        public List<string> Symptoms { get; set; }
    }
}