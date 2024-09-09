using System.Net;
using IncidentRecorder.DTOs.Disease;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace IncidentRecorder.Tests.Integration
{
    public class DiseaseControllerIntegrationTests : BaseIntegrationTest
    {
        public DiseaseControllerIntegrationTests(WebApplicationFactory<Program> factory) : base(factory) { }

        // Test: Get all diseases with seeded data
        [Fact]
        public async Task GetDiseases_ReturnsOkResult_WithSeededData()
        {
            var response = await _client.GetAsync("/api/disease");

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var diseases = JsonConvert.DeserializeObject<List<DiseaseDTO>>(content);

            Assert.NotNull(diseases);
            Assert.Equal("COVID-19", diseases[0].Name);
        }

        // Test: Get a single disease by dynamically capturing ID after creation
        [Fact]
        public async Task GetDiseaseById_ReturnsOkResult_WhenDiseaseExists()
        {
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
            var createdId = createdDisease?.Id;

            var getResponse = await _client.GetAsync($"/api/disease/{createdId}");

            getResponse.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var getContent = await getResponse.Content.ReadAsStringAsync();
            var disease = JsonConvert.DeserializeObject<DiseaseDTO>(getContent);

            Assert.NotNull(disease);
            Assert.Equal(createdId, disease.Id);
            Assert.Equal("Flu", disease.Name);
        }

        // Test: Post a new disease and verify creation
        [Fact]
        public async Task PostDisease_CreatesNewDisease()
        {
            var newDisease = new DiseaseDTO
            {
                Name = "Flu H1N1",
                Description = "H1N1 virus"
            };

            var content = new StringContent(JsonConvert.SerializeObject(newDisease), System.Text.Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/disease", content);

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var responseContent = await response.Content.ReadAsStringAsync();
            var createdDisease = JsonConvert.DeserializeObject<DiseaseDTO>(responseContent);

            Assert.NotNull(createdDisease);
            Assert.Equal("Flu H1N1", createdDisease.Name);
        }

        // Test: Update an existing disease
        [Fact]
        public async Task PutDisease_UpdatesExistingDisease()
        {
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
            var createdId = createdDisease?.Id;

            var updatedDisease = new DiseaseUpdateDTO
            {
                Name = "Updated Flu",
                Description = "Updated flu description"
            };

            var updateContent = new StringContent(JsonConvert.SerializeObject(updatedDisease), System.Text.Encoding.UTF8, "application/json");

            var putResponse = await _client.PutAsync($"/api/disease/{createdId}", updateContent);

            Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);

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
            var createdId = createdDisease?.Id;

            var deleteResponse = await _client.DeleteAsync($"/api/disease/{createdId}");

            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            var getResponse = await _client.GetAsync($"/api/disease/{createdId}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        // New Tests for Error Handling

        // Test: Create Disease with Missing Required Fields (400 Bad Request)
        [Fact]
        public async Task PostDisease_ReturnsBadRequest_WhenRequiredFieldsAreMissing()
        {
            var invalidDisease = new DiseaseDTO
            {
                Description = "Missing Name"
            };

            var content = new StringContent(JsonConvert.SerializeObject(invalidDisease), System.Text.Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/disease", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        // Test: Create Disease with Invalid Data Type (400 Bad Request)
        [Fact]
        public async Task PostDisease_ReturnsBadRequest_WhenInvalidDataTypeIsProvided()
        {
            var invalidDisease = new
            {
                Name = 12345,  // Invalid data type (should be a string)
                Description = "Invalid name type"
            };

            var content = new StringContent(JsonConvert.SerializeObject(invalidDisease), System.Text.Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/disease", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        // Test: Get Non-existent Disease (404 Not Found)
        [Fact]
        public async Task GetDiseaseById_ReturnsNotFound_WhenDiseaseDoesNotExist()
        {
            var response = await _client.GetAsync("/api/disease/999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        // Test: Update Non-existent Disease (404 Not Found)
        [Fact]
        public async Task PutDisease_ReturnsNotFound_WhenDiseaseDoesNotExist()
        {
            var updateDisease = new DiseaseUpdateDTO
            {
                Name = "Non-existent Disease",
                Description = "This disease does not exist"
            };

            var content = new StringContent(JsonConvert.SerializeObject(updateDisease), System.Text.Encoding.UTF8, "application/json");
            var response = await _client.PutAsync("/api/disease/999", content);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        // Test: Delete Non-existent Disease (404 Not Found)
        [Fact]
        public async Task DeleteDisease_ReturnsNotFound_WhenDiseaseDoesNotExist()
        {
            var response = await _client.DeleteAsync("/api/disease/999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
