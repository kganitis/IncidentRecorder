namespace IncidentRecorder.Models
{
    public class Incident
    {
        public int Id { get; set; }

        public int DiseaseId { get; set; }
        public Disease Disease { get; set; }

        public DateTime DateReported { get; set; }

        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        public int LocationId { get; set; }
        public Location Location { get; set; }

        public List<Symptom> Symptoms { get; set; }
    }
}
