using System.Net;
using IncidentRecorder.DTOs.Disease;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using IncidentRecorder.Tests.Integration;

namespace IncidentRecorder.Tests.IntegrationTests
{
    public class DiseaseControllerIntegrationTests : BaseIntegrationTest
    {
        public DiseaseControllerIntegrationTests(WebApplicationFactory<Program> factory) : base(factory) { }

        // Test: Get all diseases with seeded data
        [Fact]
        public async Task GetDiseases_ReturnsOkResult_WithSeededData()
        {
            // Act
            var response = await _client.GetAsync("/api/disease");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var diseases = JsonConvert.DeserializeObject<List<DiseaseDTO>>(content);

            // Verify the seeded disease is returned
            Assert.NotNull(diseases);
            Assert.Equal("COVID-19", diseases[0].Name);
        }

        // Test: Get a single disease by dynamically capturing ID after creation
        [Fact]
        public async Task GetDiseaseById_ReturnsOkResult_WhenDiseaseExists()
        {
            // Arrange: Create a new disease to get its ID
            var newDisease = new DiseaseDTO
            {
                Name = "Flu",
                Description = "Seasonal flu virus"
            };

            var content = new StringContent(JsonConvert.SerializeObject(newDisease), System.Text.Encoding.UTF8, "application/json");

            var postResponse = await _client.PostAsync("/api/disease", content);
            postResponse.EnsureSuccessStatusCode();
            var createdContent = await postResponse.Content.ReadAsStringAsync();
            var createdDisease = JsonConvert.DeserializeObject<DiseaseDTO>(createdContent);
            var createdId = createdDisease.Id;

            // Act: Fetch the disease by the captured ID
            var getResponse = await _client.GetAsync($"/api/disease/{createdId}");

            // Assert
            getResponse.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var getContent = await getResponse.Content.ReadAsStringAsync();
            var disease = JsonConvert.DeserializeObject<DiseaseDTO>(getContent);

            // Verify the disease details
            Assert.NotNull(disease);
            Assert.Equal(createdId, disease.Id);
            Assert.Equal("Flu", disease.Name);
        }

        // Test: Post a new disease and verify creation
        [Fact]
        public async Task PostDisease_CreatesNewDisease()
        {
            // Arrange
            var newDisease = new DiseaseDTO
            {
                Name = "Flu",
                Description = "Seasonal flu virus"
            };

            var content = new StringContent(JsonConvert.SerializeObject(newDisease), System.Text.Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/disease", content);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            // Deserialize the created disease to verify the content
            var responseContent = await response.Content.ReadAsStringAsync();
            var createdDisease = JsonConvert.DeserializeObject<DiseaseDTO>(responseContent);

            Assert.NotNull(createdDisease);
            Assert.Equal("Flu", createdDisease.Name);
        }

        // Test: Update an existing disease
        [Fact]
        public async Task PutDisease_UpdatesExistingDisease()
        {
            // Arrange: Create a new disease to get its ID
            var newDisease = new DiseaseDTO
            {
                Name = "Flu",
                Description = "Seasonal flu virus"
            };

            var postContent = new StringContent(JsonConvert.SerializeObject(newDisease), System.Text.Encoding.UTF8, "application/json");
            var postResponse = await _client.PostAsync("/api/disease", postContent);
            postResponse.EnsureSuccessStatusCode();

            var postCreatedContent = await postResponse.Content.ReadAsStringAsync();
            var createdDisease = JsonConvert.DeserializeObject<DiseaseDTO>(postCreatedContent);
            var createdId = createdDisease.Id;

            // Arrange: Prepare update data
            var updatedDisease = new DiseaseUpdateDTO
            {
                Name = "Updated Flu",
                Description = "Updated flu description"
            };

            var updateContent = new StringContent(JsonConvert.SerializeObject(updatedDisease), System.Text.Encoding.UTF8, "application/json");

            // Act: Update the disease
            var putResponse = await _client.PutAsync($"/api/disease/{createdId}", updateContent);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);

            // Verify that the disease was updated by fetching it again
            var getResponse = await _client.GetAsync($"/api/disease/{createdId}");
            var getContent = await getResponse.Content.ReadAsStringAsync();
            var updatedDiseaseResult = JsonConvert.DeserializeObject<DiseaseDTO>(getContent);

            Assert.NotNull(updatedDiseaseResult);
            Assert.Equal("Updated Flu", updatedDiseaseResult.Name);
            Assert.Equal("Updated flu description", updatedDiseaseResult.Description);
        }

        // Test: Delete an existing disease
        [Fact]
        public async Task DeleteDisease_DeletesExistingDisease()
        {
            // Arrange: Create a new disease to get its ID
            var newDisease = new DiseaseDTO
            {
                Name = "Test Disease",
                Description = "Test Description"
            };

            var postContent = new StringContent(JsonConvert.SerializeObject(newDisease), System.Text.Encoding.UTF8, "application/json");
            var postResponse = await _client.PostAsync("/api/disease", postContent);
            postResponse.EnsureSuccessStatusCode();

            var postCreatedContent = await postResponse.Content.ReadAsStringAsync();
            var createdDisease = JsonConvert.DeserializeObject<DiseaseDTO>(postCreatedContent);
            var createdId = createdDisease.Id;

            // Act: Delete the disease
            var deleteResponse = await _client.DeleteAsync($"/api/disease/{createdId}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            // Verify that the disease no longer exists
            var getResponse = await _client.GetAsync($"/api/disease/{createdId}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }
    }
}
