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
                .WithMany();

            // Disease is required (non-nullable)
            modelBuilder.Entity<Incident>()
                .HasOne(i => i.Disease)
                .WithMany()
                .HasForeignKey(i => i.DiseaseId)
                .IsRequired();

            // Patient is optional (nullable)
            modelBuilder.Entity<Incident>()
                .HasOne(i => i.Patient)
                .WithMany()
                .HasForeignKey(i => i.PatientId)
                .OnDelete(DeleteBehavior.SetNull);  // Allow nullable foreign key

            // Location is optional (nullable)
            modelBuilder.Entity<Incident>()
                .HasOne(i => i.Location)
                .WithMany()
                .HasForeignKey(i => i.LocationId)
                .OnDelete(DeleteBehavior.SetNull);  // Allow nullable foreign key

            // Add unique index to the Disease.Name column
            modelBuilder.Entity<Disease>()
                .HasIndex(d => d.Name)
                .IsUnique();

            // Add unique index to the Patient.NIN column
            modelBuilder.Entity<Patient>()
                .HasIndex(p => p.NIN)
                .IsUnique();

            // Add unique index to the combination of City and Country columns
            modelBuilder.Entity<Location>()
                .HasIndex(l => new { l.City, l.Country })
                .IsUnique();

            // Add unique index to the Symptom.Name column
            modelBuilder.Entity<Symptom>()
                .HasIndex(d => d.Name)
                .IsUnique();
        }
    }
}
