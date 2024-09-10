namespace IncidentRecorder.Models
{
    public class Patient
    {
        public int Id { get; set; }
        public string NIN { get; set; }  // National Identification Number
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string ContactInfo { get; set; }
    }
}
