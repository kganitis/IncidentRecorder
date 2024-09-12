namespace IncidentRecorder.DTOs.Symptom
{
    /// <summary>
    /// Data Transfer Object for returning symptom details.
    /// </summary>
    public class SymptomDTO
    {
        /// <summary>
        /// The unique identifier of the symptom.
        /// </summary>
        public int Id { get; set; }

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
