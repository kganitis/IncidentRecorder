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
        private static void SeedTestData(IncidentContext context)
        {
            context.Incidents.Add(new Incident
            {
                Id = 1,
                DiseaseId = 1,
                Disease = new Disease { Id = 1, Name = "COVID-19", Description = "Coronavirus Disease" },
                Patient = new Patient { Id = 1, NIN = "000000001", FirstName = "Kostas", LastName = "Ganitis", DateOfBirth = new DateTime(1992, 1, 1), ContactInfo = "k.ganitis@gmail.com", Gender = "Male" },
                Location = new Location { Id = 1, City = "Athens", Country = "Greece" },
                DateReported = DateTime.Now,
                Symptoms = [
                    new Symptom { Id = 1, Name = "Cough", Description = "Persistent cough" }, 
                    new Symptom { Id = 3, Name = "Fever", Description = "High body temperature" }]
            });
            context.Incidents.Add(new Incident
            {
                Id = 2,
                DiseaseId = 2,
                Disease = new Disease { Id = 2, Name = "Gastroenteritis", Description = "Inflammation of the stomach and intestines" },
                Patient = new Patient { Id = 2, NIN = "000000002", FirstName = "Efthymios", LastName = "Alepis", DateOfBirth = new DateTime(1980, 2, 2), ContactInfo = "e.alepis@unipi.gr", Gender = "Male" },
                Location = new Location { Id = 2, City = "Piraeus", Country = "Greece" },
                DateReported = DateTime.Now,
                Symptoms = [new Symptom { Id = 2, Name = "Nausea", Description = "Feeling of sickness with an inclination to vomit" }]
            });
            context.SaveChanges();
        }
    }
}
