using System.ComponentModel.DataAnnotations;

namespace IncidentRecorder.DTOs.Patient
{
    /// <summary>
    /// Data Transfer Object for creating a new patient.
    /// </summary>
    public class PatientCreateDTO
    {
        /// <summary>
        /// The National Identification Number (NIN) of the patient.
        /// </summary>
        /// <example>000000001</example>
        [Required(ErrorMessage = "NIN is required")]
        public required string NIN { get; set; }

        /// <summary>
        /// The first name of the patient.
        /// </summary>
        /// <example>Efthymios</example>
        [Required(ErrorMessage = "First name is required")]
        public required string FirstName { get; set; }

        /// <summary>
        /// The last name of the patient.
        /// </summary>
        /// <example>Alepis</example>
        [Required(ErrorMessage = "Last name is required")]
        public required string LastName { get; set; }

        /// <summary>
        /// The patient's date of birth.
        /// </summary>
        /// <example>1985-04-12T00:00:00Z</example>
        [Required(ErrorMessage = "Date of birth is required")]
        public required DateTime DateOfBirth { get; set; }

        /// <summary>
        /// The gender of the patient.
        /// </summary>
        /// <example>Male</example>
        [Required(ErrorMessage = "Gender is required")]
        public required string Gender { get; set; }

        /// <summary>
        /// The patient's contact information.
        /// </summary>
        /// <example>e.alepis@unipi.gr</example>
        [Required(ErrorMessage = "Contact info is required")]
        public required string ContactInfo { get; set; }
    }
}
