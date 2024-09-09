using Microsoft.EntityFrameworkCore;
using IncidentRecorder.Models;

namespace IncidentRecorder.Data
{
    public class IncidentContext : DbContext
    {
        public IncidentContext(DbContextOptions<IncidentContext> options) : base(options)
        {
        }

        // Mark DbSet properties as virtual to allow mocking
        public virtual DbSet<Patient> Patients { get; set; }
        public virtual DbSet<Incident> Incidents { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<Disease> Diseases { get; set; }
        public virtual DbSet<Symptom> Symptoms { get; set; }

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

            // Add unique index to the combination of City and Country columns
            modelBuilder.Entity<Location>()
                .HasIndex(l => new { l.City, l.Country })
                .IsUnique();
        }
    }
}
