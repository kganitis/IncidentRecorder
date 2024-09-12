using System.ComponentModel.DataAnnotations;

namespace IncidentRecorder.DTOs.Location
{
    /// <summary>
    /// Data Transfer Object for creating a new Location.
    /// </summary>
    public class LocationCreateDTO
    {
        /// <summary>
        /// The name of the city where the location is.
        /// </summary>
        /// <example>Athens</example>
        [Required(ErrorMessage = "City is required")]
        public required string City { get; set; }

        /// <summary>
        /// The name of the country where the location is.
        /// </summary>
        /// <example>Greece</example>
        [Required(ErrorMessage = "Country is required")]
        public required string Country { get; set; }
    }
}
