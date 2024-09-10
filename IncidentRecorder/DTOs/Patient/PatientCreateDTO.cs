﻿namespace IncidentRecorder.DTOs.Patient
{
    public class PatientCreateDTO
    {
        public string NIN { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string ContactInfo { get; set; }
    }
}
