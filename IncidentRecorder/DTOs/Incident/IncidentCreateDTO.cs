public class IncidentCreateDTO
{
    // Disease is required
    public int DiseaseId { get; set; }

    // Optional fields
    public int? PatientId { get; set; }  // Nullable
    public int? LocationId { get; set; }  // Nullable
    public DateTime? DateReported { get; set; }  // Nullable

    public List<int> SymptomIds { get; set; } = new List<int>();  // Optional, default to an empty list
}
