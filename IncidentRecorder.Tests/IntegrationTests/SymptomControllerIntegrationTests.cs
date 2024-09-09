using Xunit;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using IncidentRecorder.DTOs.Symptom;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using IncidentRecorder.Tests.Integration;

namespace IncidentRecorder.Tests.Integration
{
    public class SymptomControllerIntegrationTests : BaseIntegrationTest
    {
        public SymptomControllerIntegrationTests(WebApplicationFactory<Program> factory) : base(factory) { }

        // Test: Get all symptoms with seeded data
        [Fact]
        public async Task GetSymptoms_ReturnsOkResult_WithSeededData()
        {
            // Act
            var response = await _client.GetAsync("/api/symptom");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var symptoms = JsonConvert.DeserializeObject<List<SymptomDTO>>(content);

            // Verify the seeded symptom is returned (assuming "Cough" exists)
            Assert.NotNull(symptoms);
            Assert.Contains(symptoms, s => s.Name == "Cough" && s.Description == "Persistent cough");
        }

        // Test: Get a single symptom by dynamically capturing ID after creation
        [Fact]
        public async Task GetSymptomById_ReturnsOkResult_WhenSymptomExists()
        {
            // Arrange: Create a new symptom to get its ID
            var newSymptom = new SymptomCreateDTO
            {
                Name = "Fever",
                Description = "High body temperature"
            };

            var content = new StringContent(JsonConvert.SerializeObject(newSymptom), System.Text.Encoding.UTF8, "application/json");

            var postResponse = await _client.PostAsync("/api/symptom", content);
            postResponse.EnsureSuccessStatusCode();
            var createdContent = await postResponse.Content.ReadAsStringAsync();
            var createdSymptom = JsonConvert.DeserializeObject<SymptomDTO>(createdContent);
            var createdId = createdSymptom.Id;

            // Act: Fetch the symptom by the captured ID
            var getResponse = await _client.GetAsync($"/api/symptom/{createdId}");

            // Assert
            getResponse.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var getContent = await getResponse.Content.ReadAsStringAsync();
            var symptom = JsonConvert.DeserializeObject<SymptomDTO>(getContent);

            // Verify the symptom details
            Assert.NotNull(symptom);
            Assert.Equal(createdId, symptom.Id);
            Assert.Equal("Fever", symptom.Name);
            Assert.Equal("High body temperature", symptom.Description);
        }

        // Test: Post a new symptom and verify creation
        [Fact]
        public async Task PostSymptom_CreatesNewSymptom()
        {
            // Arrange
            var newSymptom = new SymptomCreateDTO
            {
                Name = "Headache",
                Description = "Pain in head or neck"
            };

            var content = new StringContent(JsonConvert.SerializeObject(newSymptom), System.Text.Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/symptom", content);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            // Deserialize the created symptom to verify the content
            var responseContent = await response.Content.ReadAsStringAsync();
            var createdSymptom = JsonConvert.DeserializeObject<SymptomDTO>(responseContent);

            Assert.NotNull(createdSymptom);
            Assert.Equal("Headache", createdSymptom.Name);
            Assert.Equal("Pain in head or neck", createdSymptom.Description);
        }

        // Test: Update an existing symptom
        [Fact]
        public async Task PutSymptom_UpdatesExistingSymptom()
        {
            // Arrange: Create a new symptom to get its ID
            var newSymptom = new SymptomCreateDTO
            {
                Name = "Fever",
                Description = "High body temperature"
            };

            var postContent = new StringContent(JsonConvert.SerializeObject(newSymptom), System.Text.Encoding.UTF8, "application/json");
            var postResponse = await _client.PostAsync("/api/symptom", postContent);
            postResponse.EnsureSuccessStatusCode();

            var postCreatedContent = await postResponse.Content.ReadAsStringAsync();
            var createdSymptom = JsonConvert.DeserializeObject<SymptomDTO>(postCreatedContent);
            var createdId = createdSymptom.Id;

            // Arrange: Prepare update data
            var updatedSymptom = new SymptomUpdateDTO
            {
                Name = "Mild Fever",
                Description = "Moderate high body temperature"
            };

            var updateContent = new StringContent(JsonConvert.SerializeObject(updatedSymptom), System.Text.Encoding.UTF8, "application/json");

            // Act: Update the symptom
            var putResponse = await _client.PutAsync($"/api/symptom/{createdId}", updateContent);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);

            // Verify that the symptom was updated by fetching it again
            var getResponse = await _client.GetAsync($"/api/symptom/{createdId}");
            var getContent = await getResponse.Content.ReadAsStringAsync();
            var updatedSymptomResult = JsonConvert.DeserializeObject<SymptomDTO>(getContent);

            Assert.NotNull(updatedSymptomResult);
            Assert.Equal("Mild Fever", updatedSymptomResult.Name);
            Assert.Equal("Moderate high body temperature", updatedSymptomResult.Description);
        }

        // Test: Delete an existing symptom
        [Fact]
        public async Task DeleteSymptom_DeletesExistingSymptom()
        {
            // Arrange: Create a new symptom to get its ID
            var newSymptom = new SymptomCreateDTO
            {
                Name = "Nausea",
                Description = "Feeling of sickness"
            };

            var postContent = new StringContent(JsonConvert.SerializeObject(newSymptom), System.Text.Encoding.UTF8, "application/json");
            var postResponse = await _client.PostAsync("/api/symptom", postContent);
            postResponse.EnsureSuccessStatusCode();

            var postCreatedContent = await postResponse.Content.ReadAsStringAsync();
            var createdSymptom = JsonConvert.DeserializeObject<SymptomDTO>(postCreatedContent);
            var createdId = createdSymptom.Id;

            // Act: Delete the symptom
            var deleteResponse = await _client.DeleteAsync($"/api/symptom/{createdId}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            // Verify that the symptom no longer exists
            var getResponse = await _client.GetAsync($"/api/symptom/{createdId}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task PostSymptom_ReturnsBadRequest_WhenRequiredFieldsAreMissing()
        {
            // Arrange: Missing the required Name field
            var invalidSymptom = new SymptomCreateDTO
            {
                Description = "Feeling of discomfort"
            };

            var content = new StringContent(JsonConvert.SerializeObject(invalidSymptom), System.Text.Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/symptom", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PostSymptom_ReturnsBadRequest_WhenInvalidDataTypeIsProvided()
        {
            // Arrange: Providing an invalid type for Name (e.g., an integer instead of a string)
            var invalidSymptom = new
            {
                Name = 12345,  // Invalid data type
                Description = "Feeling of discomfort"
            };

            var content = new StringContent(JsonConvert.SerializeObject(invalidSymptom), System.Text.Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/symptom", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetSymptomById_ReturnsNotFound_WhenSymptomDoesNotExist()
        {
            // Act: Try to get a symptom with a non-existent ID
            var response = await _client.GetAsync("/api/symptom/999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task PutSymptom_ReturnsNotFound_WhenSymptomDoesNotExist()
        {
            // Arrange: Prepare update data for a non-existent symptom
            var updateSymptom = new SymptomUpdateDTO
            {
                Name = "Non-existent Symptom",
                Description = "This symptom does not exist"
            };

            var content = new StringContent(JsonConvert.SerializeObject(updateSymptom), System.Text.Encoding.UTF8, "application/json");

            // Act: Try to update a non-existent symptom
            var response = await _client.PutAsync("/api/symptom/999", content);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteSymptom_ReturnsNotFound_WhenSymptomDoesNotExist()
        {
            // Act: Try to delete a non-existent symptom
            var response = await _client.DeleteAsync("/api/symptom/999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

    }
}
