using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using IncidentRecorder;
using IncidentRecorder.DTOs.Patient;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using IncidentRecorder.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;

namespace IncidentRecorder.Tests
{
    public class PatientControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;

        public PatientControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<IncidentContext>));
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }
                    services.AddDbContext<IncidentContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestDb");
                    });
                });
            });

            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task GetAllPatients_ReturnsSuccessStatusCode()
        {
            // Act: Send an HTTP GET request to the API
            var response = await _client.GetAsync("/api/Patient");

            // Assert: Check if the status code is 200 OK
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task CreatePatient_ReturnsCreatedResponse()
        {
            // Arrange: Create a new PatientCreateDTO for the request
            var newPatient = new PatientCreateDTO
            {
                FirstName = "Jane",
                LastName = "Doe",
                DateOfBirth = new System.DateTime(1990, 5, 14),
                ContactInfo = "jane.doe@example.com",
                Gender = "Female"
            };

            var content = new StringContent(JsonConvert.SerializeObject(newPatient), Encoding.UTF8, "application/json");

            // Act: Send an HTTP POST request to create a new patient
            var response = await _client.PostAsync("/api/Patient", content);

            // Assert: Check if the status code is 201 Created
            response.EnsureSuccessStatusCode();
            Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task GetPatientById_ReturnsPatient_WhenPatientExists()
        {
            // Arrange: First, create a new patient
            var newPatient = new PatientCreateDTO
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = new System.DateTime(1990, 5, 14),
                ContactInfo = "john.doe@example.com",
                Gender = "Male"
            };

            var content = new StringContent(JsonConvert.SerializeObject(newPatient), Encoding.UTF8, "application/json");

            // Act: Create a patient using POST and retrieve the ID from the response
            var postResponse = await _client.PostAsync("/api/Patient", content);
            postResponse.EnsureSuccessStatusCode();

            // Extract the location header from the POST response, which contains the new resource URL
            var locationHeader = postResponse.Headers.Location.ToString();

            // Act: Fetch the newly created patient using the returned location
            var getResponse = await _client.GetAsync(locationHeader);
            getResponse.EnsureSuccessStatusCode();

            var jsonResponse = await getResponse.Content.ReadAsStringAsync();
            var returnedPatient = JsonConvert.DeserializeObject<PatientDTO>(jsonResponse);

            // Assert: Verify that the returned patient's details match the created patient
            Assert.Equal("John", returnedPatient.FirstName);
            Assert.Equal("Doe", returnedPatient.LastName);
        }

    }
}
