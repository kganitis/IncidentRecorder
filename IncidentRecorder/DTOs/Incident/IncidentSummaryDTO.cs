namespace IncidentRecorder.DTOs.Incident
{
    /// <summary>
    /// Data Transfer Object (DTO) for summarizing incident data.
    /// </summary>
    public class IncidentSummaryDTO
    {
        /// <summary>
        /// The name of the disease involved in the incidents.
        /// </summary>
        public required string DiseaseName { get; set; }

        /// <summary>
        /// The location where the incidents occurred, if available.
        /// </summary>
        public string? Location { get; set; }

        /// <summary>
        /// The total number of cases for the disease.
        /// </summary>
        public int TotalCases { get; set; }

        /// <summary>
        /// The date of the last reported incident.
        /// </summary>
        public DateTime LastReported { get; set; }
    }
}
