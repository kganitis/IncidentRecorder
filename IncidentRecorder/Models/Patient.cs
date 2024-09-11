namespace IncidentRecorder.Models
{
    public class Patient
    {
        public int Id { get; set; }
        public required string NIN { get; set; }  // National Identification Number
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required DateTime DateOfBirth { get; set; }
        public required string Gender { get; set; }
        public required string ContactInfo { get; set; }
    }
}
