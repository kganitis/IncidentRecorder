namespace IncidentRecorder.DTOs.Disease
{
    /// <summary>
    /// DTO for updating a disease.
    /// </summary>
    public class DiseaseUpdateDTO
    {
        /// <summary>
        /// Name of the disease. Must be unique.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Description of the disease.
        /// </summary>
        public string? Description { get; set; }
    }
}
