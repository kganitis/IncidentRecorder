namespace IncidentRecorder.Models
{
    public class Symptom
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
    }
}
