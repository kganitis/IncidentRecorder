namespace IncidentRecorder.DTOs.Patient
{
    public class PatientCreateDTO
    {
        public required string NIN { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required DateTime DateOfBirth { get; set; }
        public required string Gender { get; set; }
        public required string ContactInfo { get; set; }
    }
}
