namespace IncidentRecorder.DTOs.Symptom
{
    /// <summary>
    /// Data Transfer Object for updating an existing symptom.
    /// </summary>
    public class SymptomUpdateDTO
    {
        /// <summary>
        /// The name of the symptom.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The description of the symptom.
        /// </summary>
        public string? Description { get; set; }
    }
}
