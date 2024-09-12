namespace IncidentRecorder.DTOs.Location
{
    /// <summary>
    /// Data Transfer Object for returning Location details.
    /// </summary>
    public class LocationDTO
    {
        /// <summary>
        /// The unique identifier of the location.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of the city where the location is.
        /// </summary>
        /// <example>Athens</example>
        public string? City { get; set; }

        /// <summary>
        /// The name of the country where the location is.
        /// </summary>
        /// <example>Greece</example>
        public string? Country { get; set; }
    }
}
