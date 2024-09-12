namespace IncidentRecorder.DTOs.Disease
{
    /// <summary>
    /// Data Transfer Object for a Disease entity.
    /// </summary>
    public class DiseaseDTO
    {
        /// <summary>
        /// The unique identifier of the disease.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of the disease. This should be a unique string representing the disease.
        /// </summary>
        /// <example>COVID-19</example>
        public string? Name { get; set; }

        /// <summary>
        /// A brief description of the disease, explaining its nature and symptoms.
        /// </summary>
        /// <example>Coronavirus Disease</example>
        public string? Description { get; set; }
    }
}
