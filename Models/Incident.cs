namespace IncidentRecorder.Models
{
    public class Incident
    {
        public int Id { get; set; }  // Primary key

        // Foreign key for Disease
        public int DiseaseId { get; set; }
        public Disease Disease { get; set; }

        public DateTime DateReported { get; set; }

        // Foreign key for Patient
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        // Foreign key for Location
        public int LocationId { get; set; }
        public Location Location { get; set; }

        // Many-to-many relationship for Symptoms
        public List<Symptom> Symptoms { get; set; }
    }
}
