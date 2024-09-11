using Microsoft.EntityFrameworkCore;
using IncidentRecorder.Models;

namespace IncidentRecorder.Data
{
    public class IncidentContext(DbContextOptions<IncidentContext> options) : DbContext(options)
    {
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Incident> Incidents { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Disease> Diseases { get; set; }
        public DbSet<Symptom> Symptoms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Many-to-many relationship between Incident and Symptom
            modelBuilder.Entity<Incident>()
                .HasMany(i => i.Symptoms)
                .WithMany()
                .UsingEntity(j => j.ToTable("IncidentSymptom"));

            // Configure required relationship for Disease
            modelBuilder.Entity<Incident>()
                .HasOne(i => i.Disease)
                .WithMany()
                .HasForeignKey(i => i.DiseaseId)
                .IsRequired();

            // Configure optional relationship for Patient
            modelBuilder.Entity<Incident>()
                .HasOne(i => i.Patient)
                .WithMany()
                .HasForeignKey(i => i.PatientId)
                .OnDelete(DeleteBehavior.SetNull);  // Patient can be nullable

            // Configure optional relationship for Location
            modelBuilder.Entity<Incident>()
                .HasOne(i => i.Location)
                .WithMany()
                .HasForeignKey(i => i.LocationId)
                .OnDelete(DeleteBehavior.SetNull);  // Location can be nullable

            // Add unique index to Disease.Name
            modelBuilder.Entity<Disease>()
                .HasIndex(d => d.Name)
                .IsUnique()
                .HasDatabaseName("IX_Unique_Disease_Name");

            // Add unique index to Patient.NIN
            modelBuilder.Entity<Patient>()
                .HasIndex(p => p.NIN)
                .IsUnique()
                .HasDatabaseName("IX_Unique_Patient_NIN");

            // Add unique index to Location.City + Country
            modelBuilder.Entity<Location>()
                .HasIndex(l => new { l.City, l.Country })
                .IsUnique()
                .HasDatabaseName("IX_Unique_Location_City_Country");

            // Add unique index to Symptom.Name
            modelBuilder.Entity<Symptom>()
                .HasIndex(s => s.Name)
                .IsUnique()
                .HasDatabaseName("IX_Unique_Symptom_Name");
        }
    }
}
