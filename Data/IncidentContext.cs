using Microsoft.EntityFrameworkCore;
using IncidentRecorder.Models;

namespace IncidentRecorder.Data
{
    public class IncidentContext : DbContext
    {
        public IncidentContext(DbContextOptions<IncidentContext> options) : base(options)
        {
        }

        public DbSet<Incident> Incidents { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Disease> Diseases { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Symptom> Symptoms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Many-to-many relationship between Incident and Symptom
            modelBuilder.Entity<Incident>()
                .HasMany(i => i.Symptoms)
                .WithMany();

            // Other relationships
            modelBuilder.Entity<Incident>()
                .HasOne(i => i.Disease)
                .WithMany();

            modelBuilder.Entity<Incident>()
                .HasOne(i => i.Patient)
                .WithMany();

            modelBuilder.Entity<Incident>()
                .HasOne(i => i.Location)
                .WithMany();
        }
    }
}
