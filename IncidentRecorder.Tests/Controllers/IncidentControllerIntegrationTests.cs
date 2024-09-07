using System.Net;
using IncidentRecorder.DTOs.Incident;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using IncidentRecorder.Tests.Integration;

namespace IncidentRecorder.Tests.IntegrationTests
{
    public class IncidentControllerIntegrationTests : BaseIntegrationTest
    {
        public IncidentControllerIntegrationTests(WebApplicationFactory<Program> factory) : base(factory) { }

        // Test: Get all incidents with seeded data
        [Fact]
        public async Task GetIncidents_ReturnsOkResult_WithSeededData()
        {
            // Act
            var response = await _client.GetAsync("/api/incident/all");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var incidents = JsonConvert.DeserializeObject<List<IncidentReadDTO>>(content);

            // Verify the seeded incident is returned (if incident data was seeded)
            Assert.NotNull(incidents);
            Assert.Contains(incidents, i => i.DiseaseName == "COVID-19" && i.PatientName == "John Doe");
        }

        // Test: Get a single incident by dynamically capturing ID after creation
        [Fact]
        public async Task GetIncidentById_ReturnsOkResult_WhenIncidentExists()
        {
            // Arrange: Create a new incident to get its ID
            var newIncident = new IncidentCreateDTO
            {
                DiseaseId = 1,
                PatientId = 1,
                LocationId = 1,
                DateReported = System.DateTime.Now,
                SymptomIds = new List<int> { 1 }
            };

            var content = new StringContent(JsonConvert.SerializeObject(newIncident), System.Text.Encoding.UTF8, "application/json");

            var postResponse = await _client.PostAsync("/api/incident/create", content);
            postResponse.EnsureSuccessStatusCode();
            var createdContent = await postResponse.Content.ReadAsStringAsync();
            var createdIncident = JsonConvert.DeserializeObject<IncidentReadDTO>(createdContent);
            var createdId = createdIncident.Id;

            // Act: Fetch the incident by the captured ID
            var getResponse = await _client.GetAsync($"/api/incident/{createdId}");

            // Assert
            getResponse.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var getContent = await getResponse.Content.ReadAsStringAsync();
            var incident = JsonConvert.DeserializeObject<IncidentReadDTO>(getContent);

            // Verify the incident details
            Assert.NotNull(incident);
            Assert.Equal(createdId, incident.Id);
            Assert.Equal("COVID-19", incident.DiseaseName);
            Assert.Equal("John Doe", incident.PatientName);
        }

        // Test: Post a new incident and verify creation
        [Fact]
        public async Task PostIncident_CreatesNewIncident()
        {
            // Arrange
            var newIncident = new IncidentCreateDTO
            {
                DiseaseId = 1,
                PatientId = 1,
                LocationId = 1,
                DateReported = System.DateTime.Now,
                SymptomIds = new List<int> { 1 }
            };

            var content = new StringContent(JsonConvert.SerializeObject(newIncident), System.Text.Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/incident/create", content);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            // Deserialize the created incident to verify the content
            var responseContent = await response.Content.ReadAsStringAsync();
            var createdIncident = JsonConvert.DeserializeObject<IncidentReadDTO>(responseContent);

            Assert.NotNull(createdIncident);
            Assert.Equal("COVID-19", createdIncident.DiseaseName);
            Assert.Equal("John Doe", createdIncident.PatientName);
        }

        // Test: Update an existing incident
        [Fact]
        public async Task PutIncident_UpdatesExistingIncident()
        {
            // Arrange: Create a new incident to get its ID
            var newIncident = new IncidentCreateDTO
            {
                DiseaseId = 1,
                PatientId = 1,
                LocationId = 1,
                DateReported = System.DateTime.Now,
                SymptomIds = new List<int> { 1 }
            };

            var postContent = new StringContent(JsonConvert.SerializeObject(newIncident), System.Text.Encoding.UTF8, "application/json");
            var postResponse = await _client.PostAsync("/api/incident/create", postContent);
            postResponse.EnsureSuccessStatusCode();

            var postCreatedContent = await postResponse.Content.ReadAsStringAsync();
            var createdIncident = JsonConvert.DeserializeObject<IncidentReadDTO>(postCreatedContent);
            var createdId = createdIncident.Id;

            // Arrange: Prepare update data
            var updatedIncident = new IncidentUpdateDTO
            {
                DiseaseId = 2, // Assume 2 represents a different disease
                PatientId = 1,
                LocationId = 1,
                DateReported = System.DateTime.Now.AddDays(-1)
            };

            var updateContent = new StringContent(JsonConvert.SerializeObject(updatedIncident), System.Text.Encoding.UTF8, "application/json");

            // Act: Update the incident
            var putResponse = await _client.PutAsync($"/api/incident/{createdId}", updateContent);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);

            // Verify that the incident was updated by fetching it again
            var getResponse = await _client.GetAsync($"/api/incident/{createdId}");
            var getContent = await getResponse.Content.ReadAsStringAsync();
            var updatedIncidentResult = JsonConvert.DeserializeObject<IncidentReadDTO>(getContent);

            Assert.NotNull(updatedIncidentResult);
            Assert.Equal("Gastroenteritis", updatedIncidentResult.DiseaseName); // Assuming disease 2 is correct
        }

        // Test: Delete an existing incident
        [Fact]
        public async Task DeleteIncident_DeletesExistingIncident()
        {
            // Arrange: Create a new incident to get its ID
            var newIncident = new IncidentCreateDTO
            {
                DiseaseId = 1,
                PatientId = 2,
                LocationId = 3,
                DateReported = System.DateTime.Now,
                SymptomIds = new List<int> { 4 }
            };

            var postContent = new StringContent(JsonConvert.SerializeObject(newIncident), System.Text.Encoding.UTF8, "application/json");
            var postResponse = await _client.PostAsync("/api/incident/create", postContent);
            postResponse.EnsureSuccessStatusCode();

            var postCreatedContent = await postResponse.Content.ReadAsStringAsync();
            Console.WriteLine("Response Content: " + postCreatedContent);
            var createdIncident = JsonConvert.DeserializeObject<IncidentReadDTO>(postCreatedContent);
            var createdId = createdIncident.Id;

            // Act: Delete the incident
            var deleteResponse = await _client.DeleteAsync($"/api/incident/{createdId}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            // Verify that the incident no longer exists
            var getResponse = await _client.GetAsync($"/api/incident/{createdId}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }
    }
}
