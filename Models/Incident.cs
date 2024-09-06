namespace IncidentRecorder.Models
{
    public class Incident
    {
        public int Id { get; set; }  // Primary key
        public string Disease { get; set; }
        public DateTime DateReported { get; set; }
        public string Patient { get; set; }
        public string Location { get; set; }
        public string Symptoms { get; set; }
    }
}
