using System.Net;
using IncidentRecorder.DTOs.Disease;
using Microsoft.AspNetCore.Mvc.Testing;

namespace IncidentRecorder.Tests.Integration
{
    public class DiseaseControllerIntegrationTests(WebApplicationFactory<Program> factory) : BaseIntegrationTest(factory)
    {
        private const string DiseaseApiUrl = "/api/disease";

        private void AssertDisease(DiseaseDTO disease, int id, string name, string description)
        {
            Assert.NotNull(disease);
            Assert.Equal(id, disease.Id);
            Assert.Equal(name, disease.Name);
            Assert.Equal(description, disease.Description);
        }

        [Fact]
        public async Task GetDiseases_ReturnsOkResult_WithSeededData()
        {
            // Act
            var response = await _client.GetAsync(DiseaseApiUrl);

            // Assert
            response.EnsureSuccessStatusCode();
            var diseases = await DeserializeResponse<List<DiseaseDTO>>(response);
            Assert.NotNull(diseases);

            for (int i = 0; i < SeededDiseases.Count; i++)
            {
                var actual = diseases[i];
                var expected = SeededDiseases[i];
                AssertDisease(actual, expected.Id, expected.Name, expected.Description);
            }
        }

        [Fact]
        public async Task GetDiseaseById_ReturnsOkResult_WhenDiseaseExists()
        {
            // Act
            var response = await _client.GetAsync($"{DiseaseApiUrl}/{SeededDiseases[0].Id}");

            // Assert
            var actual = await DeserializeResponse<DiseaseDTO>(response);
            var expected = SeededDiseases[0];
            AssertDisease(actual, expected.Id, expected.Name, expected.Description);
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
            var expectedId = createdDisease.Id;
            AssertDisease(createdDisease, expectedId, newDisease.Name, newDisease.Description);
        }

        [Fact]
        public async Task PutDisease_UpdatesExistingDisease()
        {
            // Arrange: Create a new disease to update
            var newDisease = new DiseaseCreateDTO { Name = "Test Flu", Description = "Test flu description" };

            // Get the ID of the newly created disease
            var postResponse = await _client.PostAsync(DiseaseApiUrl, CreateContent(newDisease));
            postResponse.EnsureSuccessStatusCode();
            var createdDisease = await DeserializeResponse<DiseaseDTO>(postResponse);
            var createdId = createdDisease.Id;

            // Arrange: Prepare updated disease data
            var updatedDisease = new DiseaseUpdateDTO { Name = "Updated Flu", Description = "Updated flu description" };

            // Act
            var putResponse = await _client.PutAsync($"{DiseaseApiUrl}/{createdId}", CreateContent(updatedDisease));

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);

            // Verify that the disease was updated by fetching it again
            var getResponse = await _client.GetAsync($"{DiseaseApiUrl}/{createdId}");
            var updatedDiseaseResult = await DeserializeResponse<DiseaseDTO>(getResponse);

            AssertDisease(updatedDiseaseResult, createdId, updatedDisease.Name, updatedDisease.Description);

            // Delete the updated disease to clean up
            await _client.DeleteAsync($"{DiseaseApiUrl}/{createdId}");
        }

        [Fact]
        public async Task DeleteDisease_DeletesExistingDisease()
        {
            // Arrange: Create a new disease to delete
            var newDisease = new DiseaseCreateDTO { Name = "Test Disease", Description = "Test Description" };

            var postResponse = await _client.PostAsync(DiseaseApiUrl, CreateContent(newDisease));
            postResponse.EnsureSuccessStatusCode();
            var createdDisease = await DeserializeResponse<DiseaseDTO>(postResponse);
            var createdId = createdDisease.Id;

            // Act: Delete the newly created disease
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
            var invalidDisease = "{\"description\": \"Missing name\" }";

            var content = new StringContent(invalidDisease, System.Text.Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync(DiseaseApiUrl, content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PostDisease_ReturnsBadRequest_WhenInvalidDataTypeIsProvided()
        {
            // Arrange: Provide an integer for the name which should be a string
            var invalidDisease = new { Name = 12345, Description = "Invalid name type" };

            // Act
            var response = await _client.PostAsync(DiseaseApiUrl, CreateContent(invalidDisease));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PutDisease_ReturnsBadRequest_WhenInvalidDataTypeIsProvided()
        {
            // Arrange: Provide an invalid payload with an integer for the name where a string is expected
            var invalidDisease = "{ \"name\": 12345, \"description\": \"Invalid name type\" }";

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

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PutDisease_ReturnsNoContent_WhenPayloadIsEmpty()
        {
            // Arrange: Fetch the existing disease with ID = 1 before the update
            var getResponseBeforeUpdate = await _client.GetAsync($"{DiseaseApiUrl}/1");
            getResponseBeforeUpdate.EnsureSuccessStatusCode();

            var diseaseBeforeUpdate = await DeserializeResponse<DiseaseDTO>(getResponseBeforeUpdate);

            // Act: Send an empty payload to update the disease
            var emptyPayload = "{}"; // Empty JSON object
            var updateContent = new StringContent(emptyPayload, System.Text.Encoding.UTF8, "application/json");

            var putResponse = await _client.PutAsync($"{DiseaseApiUrl}/1", updateContent);

            // Assert: The response should be 204 No Content
            Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);

            // Act: Fetch the disease again after the update
            var getResponseAfterUpdate = await _client.GetAsync($"{DiseaseApiUrl}/1");
            var diseaseAfterUpdate = await DeserializeResponse<DiseaseDTO>(getResponseAfterUpdate);

            // Assert: The disease should remain unchanged
            Assert.Equal(diseaseBeforeUpdate.Name, diseaseAfterUpdate.Name);
            Assert.Equal(diseaseBeforeUpdate.Description, diseaseAfterUpdate.Description);
        }

        [Fact]
        public async Task PostDisease_ReturnsConflict_WhenDiseaseNameIsNotUnique()
        {
            // Arrange: Create a disease with an existing name
            var duplicateDisease = new DiseaseCreateDTO
            {
                Name = "COVID-19",  // Name already exists
                Description = "Disease with duplicate name"
            };

            // Act: Try to create a duplicate disease
            var response = await _client.PostAsync(DiseaseApiUrl, CreateContent(duplicateDisease));

            // Assert: Conflict due to uniqueness violation
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task PutDisease_ReturnsConflict_WhenDiseaseNameIsNotUnique()
        {
            // Arrange: Prepare a disease with an existing name
            var duplicateDisease = new DiseaseUpdateDTO
            {
                Name = "COVID-19",  // Name already exists
            };

            // Act: Try to update a disease with the duplicate name
            var response = await _client.PutAsync($"{DiseaseApiUrl}/2", CreateContent(duplicateDisease));

            // Assert: Conflict due to uniqueness violation
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }
    }
}
