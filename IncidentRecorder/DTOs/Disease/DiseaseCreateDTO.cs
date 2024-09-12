using System.ComponentModel.DataAnnotations;

namespace IncidentRecorder.DTOs.Disease
{
    /// <summary>
    /// DTO for creating a disease.
    /// </summary>
    public class DiseaseCreateDTO
    {
        /// <summary>
        /// Name of the disease. Must be unique.
        /// </summary>
        [Required (ErrorMessage = "Name is required")]
        public required string Name { get; set; }

        /// <summary>
        /// Description of the disease.
        /// </summary>
        [Required(ErrorMessage = "Description is required")]
        public required string Description { get; set; }
    }
}
