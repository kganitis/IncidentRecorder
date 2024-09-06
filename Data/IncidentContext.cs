using Microsoft.EntityFrameworkCore;
using IncidentRecorder.Models;

namespace IncidentRecorder.Data
{
    public class IncidentContext : DbContext
    {
        public IncidentContext(DbContextOptions<IncidentContext> options) : base(options) { }

        public DbSet<Incident> Incidents { get; set; }
        public DbSet<Location> Locations { get; set; }
    }
}
