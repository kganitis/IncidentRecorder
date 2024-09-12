namespace IncidentRecorder.DTOs.Incident
{
    /// <summary>
    /// Data Transfer Object (DTO) for a lightweight listing of incidents.
    /// </summary>
    public class IncidentListDTO
    {
        /// <summary>
        /// The unique identifier of the incident.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of the disease involved in the incident.
        /// </summary>
        public required string DiseaseName { get; set; }

        /// <summary>
        /// The full name of the patient involved in the incident, if available.
        /// </summary>
        public string? PatientName { get; set; }

        /// <summary>
        /// The location of the incident, if available.
        /// </summary>
        public string? Location { get; set; }

        /// <summary>
        /// The date the incident was reported.
        /// </summary>
        public DateTime DateReported { get; set; }
    }
}
