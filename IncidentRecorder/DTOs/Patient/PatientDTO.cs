namespace IncidentRecorder.DTOs.Patient
{
    /// <summary>
    /// Data Transfer Object for returning patient details.
    /// </summary>
    public class PatientDTO
    {
        /// <summary>
        /// The unique identifier of the patient.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The National Identification Number (NIN) of the patient.
        /// </summary>
        public string? NIN { get; set; }

        /// <summary>
        /// The first name of the patient.
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// The last name of the patient.
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// The patient's date of birth.
        /// </summary>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// The gender of the patient.
        /// </summary>
        public string? Gender { get; set; }

        /// <summary>
        /// The patient's contact information.
        /// </summary>
        public string? ContactInfo { get; set; }
    }
}
