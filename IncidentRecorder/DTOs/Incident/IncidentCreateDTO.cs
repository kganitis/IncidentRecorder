using System.ComponentModel.DataAnnotations;

namespace IncidentRecorder.DTOs.Incident
{
    /// <summary>
    /// Data Transfer Object (DTO) for creating a new incident.
    /// </summary>
    public class IncidentCreateDTO
    {
        /// <summary>
        /// The ID of the disease associated with the incident. This field is required.
        /// </summary>
        [Required(ErrorMessage = "DiseaseId is required.")]
        public required int DiseaseId { get; set; }

        /// <summary>
        /// The optional ID of the patient involved in the incident.
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "PatientId must be a positive integer.")]
        public int? PatientId { get; set; }

        /// <summary>
        /// The optional ID of the location where the incident occurred.
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "LocationId must be a positive integer.")]
        public int? LocationId { get; set; }

        /// <summary>
        /// The date the incident was reported. Defaults to the current date if not provided.
        /// </summary>
        public DateTime? DateReported { get; set; } = DateTime.Now;

        /// <summary>
        /// A list of symptom IDs associated with the incident.
        /// </summary>
        public List<int> SymptomIds { get; set; } = [];
    }
}
