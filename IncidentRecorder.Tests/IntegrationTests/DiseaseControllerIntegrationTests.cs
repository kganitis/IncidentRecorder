using System.Net;
using IncidentRecorder.DTOs.Disease;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using IncidentRecorder.Models;
using IncidentRecorder.DTOs.Incident;

namespace IncidentRecorder.Tests.Integration
{
    public class DiseaseControllerIntegrationTests : BaseIntegrationTest
    {
        private const string DiseaseApiUrl = "/api/disease";

        public DiseaseControllerIntegrationTests(WebApplicationFactory<Program> factory) : base(factory) { }

        private async Task<T> DeserializeResponse<T>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }

        private StringContent CreateContent(object data) => new StringContent(JsonConvert.SerializeObject(data), System.Text.Encoding.UTF8, "application/json");

        [Fact]
        public async Task GetDiseases_ReturnsOkResult_WithSeededData()
        {
            // Act
            var response = await _client.GetAsync(DiseaseApiUrl);

            // Assert
            response.EnsureSuccessStatusCode();
            var diseases = await DeserializeResponse<List<DiseaseDTO>>(response);
            Assert.NotNull(diseases);

            var expectedDiseases = new[]
            {
                new { Id = 1, Name = "COVID-19", Description = "Coronavirus Disease" },
                // TODO check for all the diseases
                new { Id = 6, Name = "Chickenpox", Description = "Highly contagious viral infection causing an itchy rash" }
            };

            foreach (var expected in expectedDiseases)
            {
                Assert.Contains(diseases, i => i.Id == expected.Id && i.Name == expected.Name && i.Description == expected.Description);
            }
        }

        [Theory]
        [InlineData(1, "COVID-19")]
        [InlineData(2, "Gastroenteritis")]
        [InlineData(3, "Malaria")]
        [InlineData(4, "Tuberculosis")]
        [InlineData(5, "Dengue Fever")]
        [InlineData(6, "Chickenpox")]
        public async Task GetDiseaseById_ReturnsOkResult_WhenDiseaseExists(int id, string name)
        {
            // Act
            var response = await _client.GetAsync($"{DiseaseApiUrl}/{id}");

            // Assert
            var disease = await DeserializeResponse<DiseaseDTO>(response);
            Assert.NotNull(disease);
            Assert.Equal(id, disease.Id);
            Assert.Equal(name, disease.Name);
        }

        [Fact]
        public async Task PostDisease_CreatesNewDisease()
        {
            // Arrange: Prepare new disease data
            var newDisease = new DiseaseCreateDTO { Name = "Flu H1N1", Description = "H1N1 virus" };

            // Act
            var response = await _client.PostAsync(DiseaseApiUrl, CreateContent(newDisease));

            // Assert
            response.EnsureSuccessStatusCode();
            var createdDisease = await DeserializeResponse<DiseaseDTO>(response);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(createdDisease);
            Assert.Equal("Flu H1N1", createdDisease.Name);
            Assert.Equal("H1N1 virus", createdDisease.Description);
        }

        [Fact]
        public async Task PutDisease_UpdatesExistingDisease()
        {
            var id = 2;

            // Arrange: Prepare updated disease data
            var updatedDisease = new DiseaseUpdateDTO { Name = "Updated Flu", Description = "Updated flu description" };

            // Act
            var putResponse = await _client.PutAsync($"{DiseaseApiUrl}/{id}", CreateContent(updatedDisease));

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);

            // Verify that the disease was updated by fetching it again
            var getResponse = await _client.GetAsync($"{DiseaseApiUrl}/{id}");
            var updatedDiseaseResult = await DeserializeResponse<DiseaseDTO>(getResponse);

            Assert.NotNull(updatedDiseaseResult);
            Assert.Equal("Updated Flu", updatedDiseaseResult.Name);
            Assert.Equal("Updated flu description", updatedDiseaseResult.Description);
        }

        [Fact]
        public async Task DeleteDisease_DeletesExistingDisease()
        {
            // Arrange: Create a new disease to get its ID
            var newDisease = new DiseaseCreateDTO { Name = "Test Disease", Description = "Test Description" };

            var postResponse = await _client.PostAsync(DiseaseApiUrl, CreateContent(newDisease));
            postResponse.EnsureSuccessStatusCode();
            var createdDisease = await DeserializeResponse<DiseaseDTO>(postResponse);
            var createdId = createdDisease.Id;

            // Act
            var deleteResponse = await _client.DeleteAsync($"{DiseaseApiUrl}/{createdId}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            // Verify that the disease no longer exists
            var getResponse = await _client.GetAsync($"{DiseaseApiUrl}/{createdId}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task PostDisease_ReturnsBadRequest_WhenRequiredFieldsAreMissing()
        {
            // Arrange: Create a new disease with missing name
            var invalidDisease = new DiseaseCreateDTO { Description = "Missing Name" };

            // Act
            var response = await _client.PostAsync(DiseaseApiUrl, CreateContent(invalidDisease));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PostDisease_ReturnsBadRequest_WhenInvalidDataTypeIsProvided()
        {
            // Arrange: Provide an integer for the name which should be a string
            var invalidDisease = new { Name = 12345, Description = "Invalid name type" }; // Invalid data type

            // Act
            var response = await _client.PostAsync(DiseaseApiUrl, CreateContent(invalidDisease));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PutDisease_ReturnsBadRequest_WhenInvalidDataTypeIsProvided()
        {
            // Arrange: Provide an invalid payload with an integer for the name where a string is expected
            var invalidDisease = "{ \"name\": 12345 }";  // Invalid JSON for Name (should be a string)

            var content = new StringContent(invalidDisease, System.Text.Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PutAsync($"{DiseaseApiUrl}/1", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetDiseaseById_ReturnsNotFound_WhenDiseaseDoesNotExist()
        {
            // Act: Try to get a non-existing disease
            var response = await _client.GetAsync($"{DiseaseApiUrl}/999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task PutDisease_ReturnsNotFound_WhenDiseaseDoesNotExist()
        {
            // Arrange
            var updateDisease = new DiseaseUpdateDTO { };

            // Act: Try to update a non-existing disease
            var response = await _client.PutAsync($"{DiseaseApiUrl}/999", CreateContent(updateDisease));

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteDisease_ReturnsNotFound_WhenDiseaseDoesNotExist()
        {
            // Act: Try to delete a non-existing disease
            var response = await _client.DeleteAsync($"{DiseaseApiUrl}/999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetDisease_ReturnsBadRequest_WithInvalidID()
        {
            // Act: Send the request with an invalid ID in the URL (e.g., a string instead of an integer)
            var response = await _client.GetAsync($"{DiseaseApiUrl}/invalid-id");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PutDisease_ReturnsBadRequest_WithInvalidID()
        {
            // Arrange
            var updateDisease = new DiseaseUpdateDTO { };

            // Act: Send the request with an invalid ID in the URL (e.g., a string instead of an integer)
            var response = await _client.PutAsync($"{DiseaseApiUrl}/invalid-id", CreateContent(updateDisease));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task DeleteDisease_ReturnsBadRequest_WithInvalidID()
        {
            // Act: Send the request with an invalid ID in the URL (e.g., a string instead of an integer)
            var response = await _client.DeleteAsync($"{DiseaseApiUrl}/invalid-id");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PostDisease_ReturnsBadRequest_WhenPayloadIsEmpty()
        {
            // Arrange: Create an empty payload
            var emptyPayload = "{}";

            var content = new StringContent(emptyPayload, System.Text.Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync(DiseaseApiUrl, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains("The Name field is required.", responseContent);
            Assert.Contains("The Description field is required.", responseContent);
        }

        [Fact]
        public async Task PutDisease_ReturnsNoContent_WhenPayloadIsEmpty()
        {
            // Arrange: Fetch the existing incident with ID = 1 before the update
            var getResponseBeforeUpdate = await _client.GetAsync($"{DiseaseApiUrl}/1");
            getResponseBeforeUpdate.EnsureSuccessStatusCode();

            var diseaseBeforeUpdate = await DeserializeResponse<DiseaseDTO>(getResponseBeforeUpdate);

            // Act: Send an empty payload to update the incident
            var emptyPayload = "{}"; // Empty JSON object
            var updateContent = new StringContent(emptyPayload, System.Text.Encoding.UTF8, "application/json");

            var putResponse = await _client.PutAsync($"{DiseaseApiUrl}/1", updateContent);

            // Assert: The response should be 204 No Content
            Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);

            // Act: Fetch the incident again after the update
            var getResponseAfterUpdate = await _client.GetAsync($"{DiseaseApiUrl}/1");
            var diseaseAfterUpdate = await DeserializeResponse<DiseaseDTO>(getResponseAfterUpdate);

            // Assert: The incident should remain unchanged
            Assert.Equal(diseaseBeforeUpdate.Name, diseaseAfterUpdate.Name);
            Assert.Equal(diseaseBeforeUpdate.Description, diseaseAfterUpdate.Description);
        }

        [Fact]
        public async Task PostDisease_ReturnsBadRequest_WhenDiseaseNameIsNotUnique()
        {
            // Arrange: Create a disease with an existing name
            var duplicateDisease = new DiseaseCreateDTO
            {
                Name = "COVID-19",  // Name already exists
                Description = "Disease with duplicate name"
            };

            // Act: Try to create a duplicate disease
            var response = await _client.PostAsync(DiseaseApiUrl, CreateContent(duplicateDisease));

            // Assert: BadRequest due to uniqueness violation
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PutDisease_ReturnsBadRequest_WhenDiseaseNameIsNotUnique()
        {
            // Arrange: Prepare a disease with an existing name
            var duplicateDisease = new DiseaseUpdateDTO
            {
                Name = "COVID-19",  // Name already exists
            };

            // Act: Try to update a disease with the duplicate name
            var response = await _client.PutAsync($"{DiseaseApiUrl}/2", CreateContent(duplicateDisease));

            // Assert: BadRequest due to uniqueness violation
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
