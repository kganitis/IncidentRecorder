using System.ComponentModel.DataAnnotations;

namespace IncidentRecorder.DTOs.Patient
{
    public class PatientCreateDTO
    {
        [Required(ErrorMessage = "NIN is required")]
        public required string NIN { get; set; }

        [Required(ErrorMessage = "First name is required")]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "Date of birth is required")]
        public required DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public required string Gender { get; set; }

        [Required(ErrorMessage = "Contact info is required")]
        public required string ContactInfo { get; set; }
    }
}
