using System.Net;
using IncidentRecorder.DTOs.Incident;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace IncidentRecorder.Tests.Integration
{
    public class IncidentControllerIntegrationTests : BaseIntegrationTest
    {
        private const string IncidentApiUrl = "/api/incident";

        public IncidentControllerIntegrationTests(WebApplicationFactory<Program> factory) : base(factory) { }

        private async Task<T> DeserializeResponse<T>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }

        private StringContent CreateContent(object data) => new StringContent(JsonConvert.SerializeObject(data), System.Text.Encoding.UTF8, "application/json");

        [Fact]
        public async Task GetIncidents_ReturnsOkResult_WithSeededData()
        {
            // Act
            var response = await _client.GetAsync($"{IncidentApiUrl}/all");

            // Assert
            response.EnsureSuccessStatusCode();
            var incidents = await DeserializeResponse<List<IncidentReadDTO>>(response);
            Assert.NotNull(incidents);

            var expectedIncidents = new[]
            {
                new { Id = 1, DiseaseName = "COVID-19", PatientName = "John Doe" },
                // TODO check for all the incidents
                new { Id = 6, DiseaseName = "Chickenpox", PatientName = "Liam O'Reilly" }
            };

            foreach (var expected in expectedIncidents)
            {
                Assert.Contains(incidents, i => i.Id == expected.Id && i.DiseaseName == expected.DiseaseName && i.PatientName == expected.PatientName);
            }
        }

        [Theory]
        [InlineData(1, "COVID-19", "John Doe", "New York, USA", "Cough")]
        [InlineData(2, "Gastroenteritis", "Alex Smith", "Toronto, Canada", "Nausea")]
        [InlineData(3, "Malaria", "Maria Gonzalez", "Madrid, Spain", "Chills")]
        [InlineData(4, "Tuberculosis", "John Doe", "London, UK", "Persistent Cough")]
        [InlineData(5, "Dengue Fever", "Emma Brown", "Sydney, Australia", "Joint Pain")]
        [InlineData(6, "Chickenpox", "Liam O'Reilly", "Dublin, Ireland", "Rash")]
        public async Task GetIncidentById_ReturnsOkResult_WhenIncidentExists(int id, string diseaseName, string patientName, string location, string firstSymptom)
        {
            // Act
            var response = await _client.GetAsync($"{IncidentApiUrl}/{id}");

            // Assert
            response.EnsureSuccessStatusCode();
            var incident = await DeserializeResponse<IncidentReadDTO>(response);
            Assert.NotNull(incident);
            Assert.Equal(id, incident.Id);
            Assert.Equal(diseaseName, incident.DiseaseName);
            Assert.Equal(patientName, incident.PatientName);
            Assert.Equal(location, incident.Location);
            Assert.Equal(firstSymptom, incident.Symptoms[0]);
        }

        [Fact]
        public async Task PostIncident_CreatesNewIncident()
        {
            // Arrange: Prepare new incident data
            var newIncident = new IncidentCreateDTO
            {
                DiseaseId = 1,
                PatientId = 1,
                LocationId = 1,
                DateReported = DateTime.Now,
                SymptomIds = new List<int> { 1 }
            };

            // Act
            var response = await _client.PostAsync($"{IncidentApiUrl}/create", CreateContent(newIncident));

            // Assert
            response.EnsureSuccessStatusCode();
            var createdIncident = await DeserializeResponse<IncidentReadDTO>(response);
            Assert.NotNull(createdIncident);
            Assert.Equal("COVID-19", createdIncident.DiseaseName);
            Assert.Equal("John Doe", createdIncident.PatientName);
            Assert.Equal("New York, USA", createdIncident.Location);
            Assert.Equal("Cough", createdIncident.Symptoms[0]);
        }

        [Fact]
        public async Task PutIncident_UpdatesExistingIncident()
        {
            var id = 2;

            // Arrange: Prepare updated incident data
            var updatedIncident = new IncidentUpdateDTO
            {
                DiseaseId = 1,
                PatientId = 1,
                LocationId = 1,
                DateReported = DateTime.Now.AddDays(-1),
                SymptomIds = new List<int> { 1, 2 }
            };

            // Act
            var putResponse = await _client.PutAsync($"{IncidentApiUrl}/{id}", CreateContent(updatedIncident));

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);

            // Verify that the incident was updated by fetching it again
            var getResponse = await _client.GetAsync($"{IncidentApiUrl}/{id}");
            var updatedIncidentResult = await DeserializeResponse<IncidentReadDTO>(getResponse);

            Assert.NotNull(updatedIncidentResult);
            Assert.Equal("COVID-19", updatedIncidentResult.DiseaseName);
            Assert.Equal("John Doe", updatedIncidentResult.PatientName);
            Assert.Equal("New York, USA", updatedIncidentResult.Location);
            Assert.Equal("Nausea", updatedIncidentResult.Symptoms[0]);
            Assert.Equal("Cough", updatedIncidentResult.Symptoms[1]);
        }

        [Fact]
        public async Task DeleteIncident_DeletesExistingIncident()
        {
            // Arrange: Create a new incident to get its ID
            var newIncident = new IncidentCreateDTO
            {
                DiseaseId = 1,
                PatientId = 2,
                LocationId = 3,
                DateReported = DateTime.Now,
                SymptomIds = new List<int> { 4 }
            };

            var postResponse = await _client.PostAsync($"{IncidentApiUrl}/create", CreateContent(newIncident));
            postResponse.EnsureSuccessStatusCode();
            var createdIncident = await DeserializeResponse<IncidentReadDTO>(postResponse);
            var createdId = createdIncident.Id;

            // Act
            var deleteResponse = await _client.DeleteAsync($"{IncidentApiUrl}/{createdId}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            // Verify that the incident no longer exists
            var getResponse = await _client.GetAsync($"{IncidentApiUrl}/{createdId}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task PostIncident_ReturnsBadRequest_WhenRequiredFieldsAreMissing()
        {
            // Arrange: Create an incident with missing required fields
            var invalidIncident = new IncidentCreateDTO
            {
                DateReported = DateTime.Now,
                SymptomIds = new List<int> { 1 }
            };

            // Act
            var response = await _client.PostAsync($"{IncidentApiUrl}/create", CreateContent(invalidIncident));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [InlineData(999, 1, 1, new[] { 1 })]
        [InlineData(1, 999, 1, new[] { 1 })]
        [InlineData(1, 1, 999, new[] { 1 })]
        [InlineData(1, 1, 1, new[] { 999 })]
        public async Task PostIncident_ReturnsBadRequest_WhenNonExistingForeignKeyIsProvided(int diseaseId, int patientId, int locationId, int[] symptomIds)
        {
            // Arrange: Create an incident with non-existing foreign key values
            var invalidIncident = new IncidentCreateDTO
            {
                DiseaseId = diseaseId,
                PatientId = patientId,
                LocationId = locationId,
                DateReported = DateTime.Now,
                SymptomIds = symptomIds.ToList()
            };

            // Act
            var response = await _client.PostAsync($"{IncidentApiUrl}/create", CreateContent(invalidIncident));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PostIncident_ReturnsBadRequest_WhenInvalidDataTypeIsProvided()
        {
            // Arrange: Providing a string for DiseaseId which should be an integer
            var invalidIncident = new
            {
                DiseaseId = "invalid",  // Invalid data type
                PatientId = 1,
                LocationId = 1,
                DateReported = DateTime.Now,
                SymptomIds = new List<int> { 1 }
            };

            // Act
            var response = await _client.PostAsync($"{IncidentApiUrl}/create", CreateContent(invalidIncident));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PutIncident_ReturnsBadRequest_WhenInvalidDataTypeIsProvided()
        {
            // Arrange: Create an invalid payload with a string where an integer is expected
            var invalidIncident = "{ \"diseaseId\": \"invalid\" }";  // Invalid JSON for DiseaseId (should be an integer)

            var content = new StringContent(invalidIncident, System.Text.Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PutAsync($"{IncidentApiUrl}/1", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetIncidentById_ReturnsNotFound_WhenIncidentDoesNotExist()
        {
            // Act: Try to get an incident with a non-existent ID
            var response = await _client.GetAsync($"{IncidentApiUrl}/999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task PutIncident_ReturnsNotFound_WhenIncidentDoesNotExist()
        {
            // Arrange
            var updateIncident = new IncidentUpdateDTO { };

            // Act: Try to update a non-existent incident
            var response = await _client.PutAsync($"{IncidentApiUrl}/999", CreateContent(updateIncident));

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteIncident_ReturnsNotFound_WhenIncidentDoesNotExist()
        {
            // Act: Try to delete a non-existent incident
            var response = await _client.DeleteAsync($"{IncidentApiUrl}/999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetIncident_ReturnsBadRequest_WithInvalidID()
        {
            // Act: Send the request with an invalid ID in the URL (e.g., a string instead of an integer)
            var response = await _client.GetAsync($"{IncidentApiUrl}/invalid-id");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PutIncident_ReturnsBadRequest_WithInvalidID()
        {
            // Arrange
            var updateIncident = new IncidentUpdateDTO { };

            // Act: Send the request with an invalid ID in the URL (e.g., a string instead of an integer)
            var response = await _client.PutAsync($"{IncidentApiUrl}/invalid-id", CreateContent(updateIncident));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task DeleteIncident_ReturnsBadRequest_WithInvalidID()
        {
            // Act: Send the request with an invalid ID in the URL (e.g., a string instead of an integer)
            var response = await _client.DeleteAsync($"{IncidentApiUrl}/invalid-id");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PostIncident_ReturnsBadRequest_WhenPayloadIsEmpty()
        {
            // Arrange: Create an empty payload
            var emptyPayload = "{}";

            var content = new StringContent(emptyPayload, System.Text.Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync($"{IncidentApiUrl}/create", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains("DiseaseId is required and must exist", responseContent);
        }

        [Fact]
        public async Task PutIncident_ReturnsNoContent_WhenPayloadIsEmpty()
        {
            // Arrange: Fetch the existing incident with ID = 1 before the update
            var getResponseBeforeUpdate = await _client.GetAsync($"{IncidentApiUrl}/1");
            getResponseBeforeUpdate.EnsureSuccessStatusCode();

            var incidentBeforeUpdate = await DeserializeResponse<IncidentReadDTO>(getResponseBeforeUpdate);

            // Act: Send an empty payload to update the incident
            var emptyPayload = "{}"; // Empty JSON object
            var updateContent = new StringContent(emptyPayload, System.Text.Encoding.UTF8, "application/json");

            var putResponse = await _client.PutAsync($"{IncidentApiUrl}/1", updateContent);

            // Assert: The response should be 204 No Content
            Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);

            // Act: Fetch the incident again after the update
            var getResponseAfterUpdate = await _client.GetAsync($"{IncidentApiUrl}/1");
            var incidentAfterUpdate = await DeserializeResponse<IncidentReadDTO>(getResponseAfterUpdate);

            // Assert: The incident should remain unchanged
            Assert.Equal(incidentBeforeUpdate.DiseaseName, incidentAfterUpdate.DiseaseName);
            Assert.Equal(incidentBeforeUpdate.PatientName, incidentAfterUpdate.PatientName);
            Assert.Equal(incidentBeforeUpdate.Location, incidentAfterUpdate.Location);
            Assert.Equal(incidentBeforeUpdate.DateReported, incidentAfterUpdate.DateReported);
            Assert.Equal(incidentBeforeUpdate.Symptoms, incidentAfterUpdate.Symptoms);
        }
    }
}
