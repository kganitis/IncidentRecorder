using IncidentRecorder.Data;
using IncidentRecorder.Models;
using Microsoft.EntityFrameworkCore;

namespace IncidentRecorder.Tests.Unit
{
    public abstract class BaseTest
    {
        // Helper method to create a new context with a unique in-memory database
        protected IncidentContext GetInMemoryDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<IncidentContext>()
                          .UseInMemoryDatabase(databaseName: dbName + ToString()) // Ensure a unique database per test
                          .Options;

            var context = new IncidentContext(options);

            SeedTestData(context);

            return context;
        }

        // Seed initial data into the in-memory database with required fields
        private void SeedTestData(IncidentContext context)
        {
            context.Incidents.Add(new Incident
            {
                Id = 1,
                DiseaseId = 1,
                Disease = new Disease { Id = 1, Name = "COVID-19", Description = "Coronavirus Disease" },
                Patient = new Patient { Id = 1, NIN = "000000001", FirstName = "John", LastName = "Doe", DateOfBirth = new DateTime(1960, 1, 1), ContactInfo = "john.doe@example.com", Gender = "Male" },
                Location = new Location { Id = 1, City = "New York", Country = "USA" },
                DateReported = DateTime.Now,
                Symptoms = new List<Symptom> { new Symptom { Id = 1, Name = "Cough", Description = "Persistent cough" } }
            });
            context.SaveChanges();
        }
    }
}
