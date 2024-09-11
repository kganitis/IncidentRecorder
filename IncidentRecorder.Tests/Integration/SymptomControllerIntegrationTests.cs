using System.Net;
using IncidentRecorder.DTOs.Symptom;
using Microsoft.AspNetCore.Mvc.Testing;

namespace IncidentRecorder.Tests.Integration
{
    public class SymptomControllerIntegrationTests(WebApplicationFactory<Program> factory) : BaseIntegrationTest(factory)
    {
        private const string SymptomApiUrl = "/api/symptom";

        [Fact]
        public async Task GetSymptoms_ReturnsOkResult_WithSeededData()
        {
            // Act
            var response = await _client.GetAsync(SymptomApiUrl);

            // Assert
            response.EnsureSuccessStatusCode();
            var symptoms = await DeserializeResponse<List<SymptomDTO>>(response);
            Assert.NotNull(symptoms);

            var expectedSymptoms = new[]
            {
                new { Id = 1, Name = "Cough", Description = "Persistent cough" },
                // TODO check for all the symptoms
                new { Id = 6, Name = "Rash", Description = "Red, itchy skin rash with blisters" }
            };

            foreach (var expected in expectedSymptoms)
            {
                Assert.Contains(symptoms, i => i.Id == expected.Id && i.Name == expected.Name && i.Description == expected.Description);
            }
        }

        [Theory]
        [InlineData(1, "Cough")]
        [InlineData(2, "Nausea")]
        [InlineData(3, "Chills")]
        [InlineData(4, "Coughing up blood")]
        [InlineData(5, "Joint Pain")]
        [InlineData(6, "Rash")]
        public async Task GetSymptomById_ReturnsOkResult_WhenSymptomExists(int id, string name)
        {
            // Act
            var response = await _client.GetAsync($"{SymptomApiUrl}/{id}");

            // Assert
            var symptom = await DeserializeResponse<SymptomDTO>(response);
            Assert.NotNull(symptom);
            Assert.Equal(id, symptom.Id);
            Assert.Equal(name, symptom.Name);
        }

        [Fact]
        public async Task PostSymptom_CreatesNewSymptom()
        {
            // Arrange: Prepare new symptom data
            var newSymptom = new SymptomCreateDTO { Name = "Taste Loss", Description = "Loss of taste sensation" };

            // Act
            var response = await _client.PostAsync(SymptomApiUrl, CreateContent(newSymptom));

            // Assert
            response.EnsureSuccessStatusCode();
            var createdSymptom = await DeserializeResponse<SymptomDTO>(response);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(createdSymptom);
            Assert.Equal("Taste Loss", createdSymptom.Name);
            Assert.Equal("Loss of taste sensation", createdSymptom.Description);
        }

        [Fact]
        public async Task PutSymptom_UpdatesExistingSymptom()
        {
            // Arrange: Create a new symptom to update
            var newSymptom = new SymptomCreateDTO { Name = "Test Symptom", Description = "Test Description" };

            // Get the ID of the newly created symptom
            var postResponse = await _client.PostAsync(SymptomApiUrl, CreateContent(newSymptom));
            postResponse.EnsureSuccessStatusCode();
            var createdSymptom = await DeserializeResponse<SymptomDTO>(postResponse);
            var createdId = createdSymptom.Id;

            // Arrange: Prepare updated symptom data
            var updatedSymptom = new SymptomUpdateDTO { Name = "Updated Symptom", Description = "Updated Description" };

            // Act
            var putResponse = await _client.PutAsync($"{SymptomApiUrl}/{createdId}", CreateContent(updatedSymptom));

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);

            // Verify that the symptom was updated by fetching it again
            var getResponse = await _client.GetAsync($"{SymptomApiUrl}/{createdId}");
            var updatedSymptomResult = await DeserializeResponse<SymptomDTO>(getResponse);

            Assert.NotNull(updatedSymptomResult);
            Assert.Equal("Updated Symptom", updatedSymptomResult.Name);
            Assert.Equal("Updated Description", updatedSymptomResult.Description);

            // Delete the updated symptom to clean up
            await _client.DeleteAsync($"{SymptomApiUrl}/{createdId}");
        }

        [Fact]
        public async Task DeleteSymptom_DeletesExistingSymptom()
        {
            // Arrange: Create a new symptom to get its ID
            var newSymptom = new SymptomCreateDTO { Name = "Test Symptom", Description = "Test Description" };

            var postResponse = await _client.PostAsync(SymptomApiUrl, CreateContent(newSymptom));
            postResponse.EnsureSuccessStatusCode();
            var createdSymptom = await DeserializeResponse<SymptomDTO>(postResponse);
            var createdId = createdSymptom.Id;

            // Act
            var deleteResponse = await _client.DeleteAsync($"{SymptomApiUrl}/{createdId}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            // Verify that the symptom no longer exists
            var getResponse = await _client.GetAsync($"{SymptomApiUrl}/{createdId}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task PostSymptom_ReturnsBadRequest_WhenRequiredFieldsAreMissing()
        {
            // Arrange: Create a new symptom with missing name
            var invalidSymptom = "{\"description\":\"High fever\"}";

            var content = new StringContent(invalidSymptom, System.Text.Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync(SymptomApiUrl, content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PostSymptom_ReturnsBadRequest_WhenInvalidDataTypeIsProvided()
        {
            // Arrange: Provide an integer for the name which should be a string
            var invalidSymptom = new { Name = 12345, Description = "Invalid name type" }; // Invalid data type

            // Act
            var response = await _client.PostAsync(SymptomApiUrl, CreateContent(invalidSymptom));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PutSymptom_ReturnsBadRequest_WhenInvalidDataTypeIsProvided()
        {
            // Arrange: Provide an invalid payload with an integer for the name where a string is expected
            var invalidSymptom = "{ \"name\": 12345, \"description\": \"Invalid name type\" }";

            var content = new StringContent(invalidSymptom, System.Text.Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PutAsync($"{SymptomApiUrl}/1", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetSymptomById_ReturnsNotFound_WhenSymptomDoesNotExist()
        {
            // Act: Try to get a non-existing symptom
            var response = await _client.GetAsync($"{SymptomApiUrl}/999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task PutSymptom_ReturnsNotFound_WhenSymptomDoesNotExist()
        {
            // Arrange
            var updateSymptom = new SymptomUpdateDTO { };

            // Act: Try to update a non-existing symptom
            var response = await _client.PutAsync($"{SymptomApiUrl}/999", CreateContent(updateSymptom));

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteSymptom_ReturnsNotFound_WhenSymptomDoesNotExist()
        {
            // Act: Try to delete a non-existing symptom
            var response = await _client.DeleteAsync($"{SymptomApiUrl}/999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetSymptom_ReturnsBadRequest_WithInvalidID()
        {
            // Act: Send the request with an invalid ID in the URL (e.g., a string instead of an integer)
            var response = await _client.GetAsync($"{SymptomApiUrl}/invalid-id");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PutSymptom_ReturnsBadRequest_WithInvalidID()
        {
            // Arrange
            var updateSymptom = new SymptomUpdateDTO { };

            // Act: Send the request with an invalid ID in the URL (e.g., a string instead of an integer)
            var response = await _client.PutAsync($"{SymptomApiUrl}/invalid-id", CreateContent(updateSymptom));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task DeleteSymptom_ReturnsBadRequest_WithInvalidID()
        {
            // Act: Send the request with an invalid ID in the URL (e.g., a string instead of an integer)
            var response = await _client.DeleteAsync($"{SymptomApiUrl}/invalid-id");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PostSymptom_ReturnsBadRequest_WhenPayloadIsEmpty()
        {
            // Arrange: Create an empty payload
            var emptyPayload = "{}";

            var content = new StringContent(emptyPayload, System.Text.Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync(SymptomApiUrl, content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PutSymptom_ReturnsNoContent_WhenPayloadIsEmpty()
        {
            // Arrange: Fetch the existing symptom with ID = 1 before the update
            var getResponseBeforeUpdate = await _client.GetAsync($"{SymptomApiUrl}/1");
            getResponseBeforeUpdate.EnsureSuccessStatusCode();

            var symptomBeforeUpdate = await DeserializeResponse<SymptomDTO>(getResponseBeforeUpdate);

            // Act: Send an empty payload to update the symptom
            var emptyPayload = "{}"; // Empty JSON object
            var updateContent = new StringContent(emptyPayload, System.Text.Encoding.UTF8, "application/json");

            var putResponse = await _client.PutAsync($"{SymptomApiUrl}/1", updateContent);

            // Assert: The response should be 204 No Content
            Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);

            // Act: Fetch the symptom again after the update
            var getResponseAfterUpdate = await _client.GetAsync($"{SymptomApiUrl}/1");
            var symptomAfterUpdate = await DeserializeResponse<SymptomDTO>(getResponseAfterUpdate);

            // Assert: The symptom should remain unchanged
            Assert.Equal(symptomBeforeUpdate.Name, symptomAfterUpdate.Name);
            Assert.Equal(symptomBeforeUpdate.Description, symptomAfterUpdate.Description);
        }

        [Fact]
        public async Task PostSymptom_ReturnsConflict_WhenSymptomNameIsNotUnique()
        {
            // Arrange: Create a symptom with an existing name
            var duplicateSymptom = new SymptomCreateDTO
            {
                Name = "Cough",  // Name already exists
                Description = "Symptom with duplicate name"
            };

            // Act: Try to create a duplicate symptom
            var response = await _client.PostAsync(SymptomApiUrl, CreateContent(duplicateSymptom));

            // Assert: Conflict due to uniqueness violation
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task PutSymptom_ReturnsConflict_WhenSymptomNameIsNotUnique()
        {
            // Arrange: Prepare a symptom with an existing name
            var duplicateSymptom = new SymptomUpdateDTO
            {
                Name = "Cough",  // Name already exists
            };

            // Act: Try to update a symptom with the duplicate name
            var response = await _client.PutAsync($"{SymptomApiUrl}/2", CreateContent(duplicateSymptom));

            // Assert: Conflict due to uniqueness violation
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }
    }
}
