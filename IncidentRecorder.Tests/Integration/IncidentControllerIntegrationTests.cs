using System.Net;
using IncidentRecorder.DTOs.Incident;
using IncidentRecorder.Models;
using Microsoft.AspNetCore.Mvc.Testing;

namespace IncidentRecorder.Tests.Integration
{
    public class IncidentControllerIntegrationTests(WebApplicationFactory<Program> factory) : BaseIntegrationTest(factory)
    {
        private const string IncidentApiUrl = "/api/incident";

        private void AssertIncident(IncidentReadDTO actual, int id, string diseaseName, string patientName, string location, List<string> symptoms)
        {
            Assert.NotNull(actual);
            Assert.Equal(id, actual.Id);
            Assert.Equal(diseaseName, actual.DiseaseName);
            Assert.Equal(patientName, actual.PatientName);
            Assert.Equal(location, actual.Location);
            Assert.NotNull(actual.Symptoms);
            Assert.Equal(symptoms, actual.Symptoms);

        }

        [Fact]
        public async Task GetIncidents_ReturnsOkResult_WithSeededData()
        {
            // Act
            var response = await _client.GetAsync($"{IncidentApiUrl}/all");

            // Assert
            response.EnsureSuccessStatusCode();
            var incidents = await DeserializeResponse<List<IncidentReadDTO>>(response);
            Assert.NotNull(incidents);
            for (int i = 0; i < SeededIncidents.Count; i++)
            {
                var actual = incidents[i];
                var expected = SeededIncidents[i];

                AssertIncident(actual,
                   expected.Id,
                   expected.Disease.Name,
                   $"{expected.Patient?.FirstName} {expected.Patient?.LastName}",
                   $"{expected.Location?.City}, {expected.Location?.Country}",
                   expected.Symptoms.Select(s => s.Name).ToList());
            }
        }

        [Fact]
        public async Task GetIncidentById_ReturnsOkResult_WhenIncidentExists()
        {
            // Act
            var response = await _client.GetAsync($"{IncidentApiUrl}/{SeededIncidents[0].Id}");

            // Assert
            response.EnsureSuccessStatusCode();

            var actual = await DeserializeResponse<IncidentReadDTO>(response);
            var expected = SeededIncidents[0];

            AssertIncident(actual,
                expected.Id,
                expected.Disease.Name,
                $"{expected.Patient?.FirstName} {expected.Patient?.LastName}",
                $"{expected.Location?.City}, {expected.Location?.Country}",
                expected.Symptoms.Select(s => s.Name).ToList());
        }

        [Fact]
        public async Task PostIncident_CreatesNewIncident()
        {
            // Arrange: Prepare new incident data (copying the first seeded incident)
            var newIncident = new IncidentCreateDTO
            {
                DiseaseId = 1,
                PatientId = 1,
                LocationId = 1,
                DateReported = DateTime.Now,
                SymptomIds = [1]
            };

            // Act
            var response = await _client.PostAsync($"{IncidentApiUrl}/create", CreateContent(newIncident));

            // Assert: The expected incident should be the same as the first seeded incident, but with a new ID
            response.EnsureSuccessStatusCode();

            var createdIncident = await DeserializeResponse<IncidentReadDTO>(response);
            var expectedId = createdIncident.Id;

            AssertIncident(createdIncident,
                expectedId,
                SeededDiseases[0].Name,
                $"{SeededPatients[0].FirstName} {SeededPatients[0].LastName}",
                $"{SeededLocations[0].City}, {SeededLocations[0].Country}",
                [SeededSymptoms[0].Name]);
        }

        [Fact]
        public async Task PutIncident_UpdatesExistingIncident()
        {
            // Arrange: Create a new incident to update
            var newIncident = new IncidentCreateDTO
            {
                DiseaseId = 1,
                PatientId = 2,
                LocationId = 3,
                DateReported = DateTime.Now,
                SymptomIds = [4]
            };

            // Get the ID of the newly created incident
            var postResponse = await _client.PostAsync($"{IncidentApiUrl}/create", CreateContent(newIncident));
            postResponse.EnsureSuccessStatusCode();
            var createdIncident = await DeserializeResponse<IncidentReadDTO>(postResponse);
            var createdId = createdIncident.Id;

            // Arrange: Prepare updated incident data
            var updatedIncident = new IncidentUpdateDTO
            {
                DiseaseId = 1,
                PatientId = 1,
                LocationId = 1,
                DateReported = DateTime.Now.AddDays(-1),
                SymptomIds = [1, 2]
            };

            // Act
            var putResponse = await _client.PutAsync($"{IncidentApiUrl}/{createdId}", CreateContent(updatedIncident));

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);

            // Verify that the incident was updated by fetching it again
            var getResponse = await _client.GetAsync($"{IncidentApiUrl}/{createdId}");
            var updatedIncidentResult = await DeserializeResponse<IncidentReadDTO>(getResponse);

            AssertIncident(updatedIncidentResult,
               createdId,
               SeededDiseases[0].Name,
               $"{SeededPatients[0].FirstName} {SeededPatients[0].LastName}",
               $"{SeededLocations[0].City}, {SeededLocations[0].Country}",
               [SeededSymptoms[0].Name, SeededSymptoms[1].Name]);

            // Delete the updated incident to clean up
            await _client.DeleteAsync($"{IncidentApiUrl}/{createdId}");
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
                SymptomIds = [4]
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
            var invalidIncident = "{\"patientId\":2,\"locationId\":2,\"dateReported\":\"2024-09-08T10:00:00Z\",\"symptomIds\":[2]}";

            var content = new StringContent(invalidIncident, System.Text.Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync($"{IncidentApiUrl}/create", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [InlineData(999, 1, 1, new[] { 1 })]
        [InlineData(1, 999, 1, new[] { 1 })]
        [InlineData(1, 1, 999, new[] { 1 })]
        [InlineData(1, 1, 1, new[] { 999 })]
        public async Task PostIncident_ReturnsNotFound_WhenNonExistingForeignKeyIsProvided(int diseaseId, int patientId, int locationId, int[] symptomIds)
        {
            // Arrange: Create an incident with non-existing foreign key values
            var invalidIncident = new IncidentCreateDTO
            {
                DiseaseId = diseaseId,
                PatientId = patientId,
                LocationId = locationId,
                DateReported = DateTime.Now,
                SymptomIds = [.. symptomIds]
            };

            // Act
            var response = await _client.PostAsync($"{IncidentApiUrl}/create", CreateContent(invalidIncident));

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
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

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
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
