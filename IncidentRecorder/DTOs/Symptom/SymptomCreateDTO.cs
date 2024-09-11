using System.ComponentModel.DataAnnotations;

namespace IncidentRecorder.DTOs.Symptom
{
    public class SymptomCreateDTO
    {
        [Required(ErrorMessage = "Name is required")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public required string Description { get; set; }
    }
}
