namespace IncidentRecorder.DTOs.Incident
{
    public class IncidentDetailsDTO
    {
        public int Id { get; set; }
        public string DiseaseName { get; set; }
        public string DiseaseDescription { get; set; }  // More details
        public string PatientName { get; set; }
        public DateTime PatientDateOfBirth { get; set; }
        public string PatientContactInfo { get; set; } // Contact details
        public string Location { get; set; }
        public DateTime DateReported { get; set; }
        public List<string> Symptoms { get; set; }
    }
}