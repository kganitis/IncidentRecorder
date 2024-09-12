using System.ComponentModel.DataAnnotations;

namespace IncidentRecorder.DTOs.Incident
{
    /// <summary>
    /// Data Transfer Object (DTO) for updating an existing incident.
    /// </summary>
    public class IncidentUpdateDTO
    {
        /// <summary>
        /// The optional ID of the disease associated with the incident.
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "DiseaseId must be a positive integer.")]
        public int? DiseaseId { get; set; }

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
        /// The optional date the incident was reported.
        /// </summary>
        public DateTime? DateReported { get; set; }

        /// <summary>
        /// A list of symptom IDs associated with the incident.
        /// </summary>
        public List<int>? SymptomIds { get; set; }
    }
}
