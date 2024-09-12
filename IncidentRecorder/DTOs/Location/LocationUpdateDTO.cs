namespace IncidentRecorder.DTOs.Location
{
    /// <summary>
    /// Data Transfer Object for updating an existing Location.
    /// </summary>
    public class LocationUpdateDTO
    {
        /// <summary>
        /// The updated name of the city (optional).
        /// </summary>
        /// <example>Thessaloniki</example>
        public string? City { get; set; }

        /// <summary>
        /// The updated name of the country (optional).
        /// </summary>
        /// <example>Greece</example>
        public string? Country { get; set; }
    }
}
