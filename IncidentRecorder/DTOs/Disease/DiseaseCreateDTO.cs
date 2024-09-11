using System.ComponentModel.DataAnnotations;

namespace IncidentRecorder.DTOs.Disease
{
    public class DiseaseCreateDTO
    {
        [Required (ErrorMessage = "Name is required")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public required string Description { get; set; }
    }
}
