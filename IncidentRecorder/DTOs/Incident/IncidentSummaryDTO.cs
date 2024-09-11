﻿namespace IncidentRecorder.DTOs.Incident
{
    public class IncidentSummaryDTO
    {
        public required string DiseaseName { get; set; }
        public string? Location { get; set; }
        public int TotalCases { get; set; }
        public DateTime LastReported { get; set; }
    }
}