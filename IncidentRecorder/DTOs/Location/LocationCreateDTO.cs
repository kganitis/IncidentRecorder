using System.ComponentModel.DataAnnotations;

namespace IncidentRecorder.DTOs.Location
{
    public class LocationCreateDTO
    {
        [Required (ErrorMessage = "City is required")]
        public required string City { get; set; }

        [Required(ErrorMessage = "Country is required")]
        public required string Country { get; set; }
    }
}
