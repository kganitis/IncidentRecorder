using System.ComponentModel.DataAnnotations;

namespace IncidentRecorder.DTOs.Symptom
{
    /// <summary>
    /// Data Transfer Object for creating a new symptom.
    /// </summary>
    public class SymptomCreateDTO
    {
        /// <summary>
        /// The name of the symptom.
        /// </summary>
        /// <example>Cough</example>
        [Required(ErrorMessage = "Name is required")]
        public required string Name { get; set; }

        /// <summary>
        /// The description of the symptom.
        /// </summary>
        /// <example>Persistent dry cough</example>
        [Required(ErrorMessage = "Description is required")]
        public required string Description { get; set; }
    }
}
