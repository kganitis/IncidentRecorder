namespace IncidentRecorder.Models
{
    public class Incident
    {
        public int Id { get; set; }

        // Disease is required
        public int DiseaseId { get; set; }
        public Disease Disease { get; set; }

        // Optional fields
        public int? PatientId { get; set; }
        public Patient? Patient { get; set; }

        public int? LocationId { get; set; }
        public Location? Location { get; set; }

        public DateTime DateReported { get; set; } = DateTime.Now;

        public List<Symptom> Symptoms { get; set; } = new List<Symptom>();
    }
}