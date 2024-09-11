using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using IncidentRecorder.Data;
using Microsoft.EntityFrameworkCore;
using IncidentRecorder.Models;
using Newtonsoft.Json;

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
                    ReplaceWithInMemoryDatabase(services);

                    // Create a new scope to seed the database
                    var sp = services.BuildServiceProvider();
                    using var scope = sp.CreateScope();
                    var scopedServices = scope.ServiceProvider;
                    var dbContext = scopedServices.GetRequiredService<IncidentContext>();
                    var logger = scopedServices.GetRequiredService<ILogger<BaseIntegrationTest>>();

                    // Ensure the database is created
                    dbContext.Database.EnsureCreated();

                    try
                    {
                        // Seed the database with test data
                        SeedTestData(dbContext);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "An error occurred seeding the test database.");
                    }
                });
            });

            _client = _factory.CreateClient();
        }

        /// <summary>
        /// Replaces the real database context with an in-memory version for testing.
        /// </summary>
        private void ReplaceWithInMemoryDatabase(IServiceCollection services)
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<IncidentContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<IncidentContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb_" + ToString()); // Ensure unique DB for each test run
            });
        }

        /// <summary>
        /// Deserializes the HTTP response content into the specified type.
        /// </summary>
        protected static async Task<T> DeserializeResponse<T>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<T>(content);

            return result ?? throw new InvalidOperationException("Deserialization returned null.");
        }

        /// <summary>
        /// Helper method to create StringContent from an object.
        /// </summary>
        protected static StringContent CreateContent(object data) =>
            new(JsonConvert.SerializeObject(data), System.Text.Encoding.UTF8, "application/json");

        protected static List<Disease> SeededDiseases =>
        [
            new() { Id = 1, Name = "COVID-19", Description = "Coronavirus Disease" },
            new() { Id = 2, Name = "Gastroenteritis", Description = "Inflammation of the stomach and intestines" },
            new() { Id = 3, Name = "Malaria", Description = "Mosquito-borne infectious disease" },
            new() { Id = 4, Name = "Tuberculosis", Description = "Bacterial infection that mainly affects the lungs" },
            new() { Id = 5, Name = "Dengue Fever", Description = "Mosquito-borne viral infection" },
            new() { Id = 6, Name = "Chickenpox", Description = "Highly contagious viral infection causing an itchy rash" }
        ];

        protected static List<Patient> SeededPatients =>
        [
            new() { Id = 1, NIN = "000000001", FirstName = "John", LastName = "Doe", DateOfBirth = new DateTime(1960, 1, 1), ContactInfo = "john.doe@example.com", Gender = "Male" },
            new() { Id = 2, NIN = "000000002", FirstName = "Alex", LastName = "Smith", DateOfBirth = new DateTime(1970, 2, 2), ContactInfo = "alex.smith@healthmail.com", Gender = "Male" },
            new() { Id = 3, NIN = "000000003", FirstName = "Maria", LastName = "Gonzalez", DateOfBirth = new DateTime(1980, 3, 3), ContactInfo = "maria.gonzalez@medmail.com", Gender = "Female" },
            new() { Id = 4, NIN = "000000004", FirstName = "John", LastName = "Doe", DateOfBirth = new DateTime(1990, 4, 4), ContactInfo = "john.doe@medemail.com", Gender = "Male" },
            new() { Id = 5, NIN = "000000005", FirstName = "Emma", LastName = "Brown", DateOfBirth = new DateTime(2000, 5, 5), ContactInfo = "emma.brown@health.com", Gender = "Female" },
            new() { Id = 6, NIN = "000000006", FirstName = "Liam", LastName = "O'Reilly", DateOfBirth = new DateTime(2010, 6, 6), ContactInfo = "liam.oreilly@medservice.com", Gender = "Male" }
        ];

        protected static List<Location> SeededLocations =>
        [
            new() { Id = 1, City = "New York", Country = "USA" },
            new() { Id = 2, City = "Toronto", Country = "Canada" },
            new() { Id = 3, City = "Madrid", Country = "Spain" },
            new() { Id = 4, City = "London", Country = "UK" },
            new() { Id = 5, City = "Sydney", Country = "Australia" },
            new() { Id = 6, City = "Dublin", Country = "Ireland" }
        ];

        protected static List<Symptom> SeededSymptoms =>
        [
            new() { Id = 1, Name = "Cough", Description = "Persistent cough" },
            new() { Id = 2, Name = "Nausea", Description = "Feeling of sickness with an inclination to vomit" },
            new() { Id = 3, Name = "Chills", Description = "Feeling of coldness despite a fever" },
            new() { Id = 4, Name = "Coughing up blood", Description = "Coughing up blood or bloody mucus" },
            new() { Id = 5, Name = "Joint Pain", Description = "Severe pain in muscles and joints" },
            new() { Id = 6, Name = "Rash", Description = "Red, itchy skin rash with blisters" }
        ];

        protected static List<Incident> SeededIncidents =>
        [
            new() { Id = 1, DiseaseId = 1, Disease = SeededDiseases[0], Patient = SeededPatients[0], Location = SeededLocations[0], DateReported = DateTime.Now, Symptoms = [SeededSymptoms[0]] },
            new() { Id = 2, DiseaseId = 2, Disease = SeededDiseases[1], Patient = SeededPatients[1], Location = SeededLocations[1], DateReported = DateTime.Now, Symptoms = [SeededSymptoms[1]] },
            new() { Id = 3, DiseaseId = 3, Disease = SeededDiseases[2], Patient = SeededPatients[2], Location = SeededLocations[2], DateReported = DateTime.Now, Symptoms = [SeededSymptoms[2]] },
            new() { Id = 4, DiseaseId = 4, Disease = SeededDiseases[3], Patient = SeededPatients[3], Location = SeededLocations[3], DateReported = DateTime.Now, Symptoms = [SeededSymptoms[3]] },
            new() { Id = 5, DiseaseId = 5, Disease = SeededDiseases[4], Patient = SeededPatients[4], Location = SeededLocations[4], DateReported = DateTime.Now, Symptoms = [SeededSymptoms[4]] },
            new() { Id = 6, DiseaseId = 6, Disease = SeededDiseases[5], Patient = SeededPatients[5], Location = SeededLocations[5], DateReported = DateTime.Now, Symptoms = [SeededSymptoms[5]] }
        ];

        // Seed initial data into the in-memory database with required fields
        private static void SeedTestData(IncidentContext dbContext)
        {
            dbContext.Incidents.AddRange(SeededIncidents);
            try
            {
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Seeding test data failed", ex);
            }
        }
    }
}
