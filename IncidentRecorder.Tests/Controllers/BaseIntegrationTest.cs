using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using IncidentRecorder.Data;
using Microsoft.EntityFrameworkCore;
using IncidentRecorder.Models;

namespace IncidentRecorder.Tests.Integration
{
    public class BaseIntegrationTest: IClassFixture<WebApplicationFactory<Program>>
    {
        protected readonly HttpClient _client;
        protected readonly WebApplicationFactory<Program> _factory;

        public BaseIntegrationTest(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Replace the real database with an in-memory one for testing
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<IncidentContext>));

                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    services.AddDbContext<IncidentContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestDb_" + ToString()); // Unique in-memory database for each test
                    });

                    // Create a new scope to seed the database
                    var sp = services.BuildServiceProvider();
                    using var scope = sp.CreateScope();
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<IncidentContext>();
                    var logger = scopedServices.GetRequiredService<ILogger<BaseIntegrationTest>>();

                    // Ensure the database is created
                    db.Database.EnsureCreated();

                    try
                    {
                        // Seed the database with test data
                        SeedTestData(db);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "An error occurred seeding the test database.");
                    }
                });
            });

            _client = _factory.CreateClient();
        }

        // Seed initial data into the in-memory database with required fields
        private void SeedTestData(IncidentContext context)
        {
            context.Incidents.Add(new Incident
            {
                Id = 1,
                Disease = new Disease { Id = 1, Name = "COVID-19", Description = "Coronavirus Disease" },
                Patient = new Patient { Id = 1, FirstName = "John", LastName = "Doe", ContactInfo = "john.doe@example.com", Gender = "Male" },
                Location = new Location { Id = 1, City = "New York", Country = "USA" },
                DateReported = DateTime.Now,
                Symptoms = new List<Symptom> { new Symptom { Id = 1, Name = "Cough", Description = "Persistent cough" } }
            });
            context.Incidents.Add(new Incident
            {
                Id = 2,
                Disease = new Disease { Id = 2, Name = "Gastroenteritis", Description = "Inflammation of the stomach and intestines" },
                Patient = new Patient { Id = 2, FirstName = "Alex", LastName = "Smith", ContactInfo = "alex.smith@healthmail.com", Gender = "Male" },
                Location = new Location { Id = 2, City = "Toronto", Country = "Canada" },
                DateReported = DateTime.Now,
                Symptoms = new List<Symptom> { new Symptom { Id = 2, Name = "Nausea", Description = "Feeling of sickness with an inclination to vomit" } }
            });
            context.Incidents.Add(new Incident
            {
                Id = 3,
                Disease = new Disease { Id = 3, Name = "Malaria", Description = "Mosquito-borne infectious disease" },
                Patient = new Patient { Id = 3, FirstName = "Maria", LastName = "Gonzalez", ContactInfo = "maria.gonzalez@medmail.com", Gender = "Female" },
                Location = new Location { Id = 3, City = "Madrid", Country = "Spain" },
                DateReported = DateTime.Now,
                Symptoms = new List<Symptom> { new Symptom { Id = 3, Name = "Chills", Description = "Feeling of coldness despite a fever" } }
            });
            context.Incidents.Add(new Incident
            {
                Id = 4,
                Disease = new Disease { Id = 4, Name = "Tuberculosis", Description = "Bacterial infection that mainly affects the lungs" },
                Patient = new Patient { Id = 4, FirstName = "John", LastName = "Doe", ContactInfo = "john.doe@medemail.com", Gender = "Male" },
                Location = new Location { Id = 4, City = "London", Country = "UK" },
                DateReported = DateTime.Now,
                Symptoms = new List<Symptom> { new Symptom { Id = 4, Name = "Cough", Description = "Persistent cough for more than 3 weeks" } }
            });
            context.Incidents.Add(new Incident
            {
                Id = 5,
                Disease = new Disease { Id = 5, Name = "Dengue Fever", Description = "Mosquito-borne viral infection" },
                Patient = new Patient { Id = 5, FirstName = "Emma", LastName = "Brown", ContactInfo = "emma.brown@health.com", Gender = "Female" },
                Location = new Location { Id = 5, City = "Sydney", Country = "Australia" },
                DateReported = DateTime.Now,
                Symptoms = new List<Symptom> { new Symptom { Id = 5, Name = "Joint Pain", Description = "Severe pain in muscles and joints" } }
            });
            context.Incidents.Add(new Incident
            {
                Id = 6,
                Disease = new Disease { Id = 6, Name = "Chickenpox", Description = "Highly contagious viral infection causing an itchy rash" },
                Patient = new Patient { Id = 6, FirstName = "Liam", LastName = "O'Reilly", ContactInfo = "liam.oreilly@medservice.com", Gender = "Male" },
                Location = new Location { Id = 6, City = "Dublin", Country = "Ireland" },
                DateReported = DateTime.Now,
                Symptoms = new List<Symptom> { new Symptom { Id = 6, Name = "Rash", Description = "Red, itchy skin rash with blisters" } }
            });

            context.SaveChanges();
        }
    }
}
