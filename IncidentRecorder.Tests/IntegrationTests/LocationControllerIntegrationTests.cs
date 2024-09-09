using System.Net;
using IncidentRecorder.DTOs.Location;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace IncidentRecorder.Tests.Integration
{
    public class LocationControllerIntegrationTests : BaseIntegrationTest
    {
        public LocationControllerIntegrationTests(WebApplicationFactory<Program> factory) : base(factory) { }

        // Test: Get all locations with seeded data
        [Fact]
        public async Task GetLocations_ReturnsOkResult_WithSeededData()
        {
            var response = await _client.GetAsync("/api/location");

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var locations = JsonConvert.DeserializeObject<List<LocationDTO>>(content);

            Assert.NotNull(locations);
            Assert.Contains(locations, l => l.City == "New York" && l.Country == "USA");
        }

        // Test: Get a single location by dynamically capturing ID after creation
        [Fact]
        public async Task GetLocationById_ReturnsOkResult_WhenLocationExists()
        {
            var newLocation = new LocationCreateDTO
            {
                City = "Berlin",
                Country = "Germany"
            };

            var content = new StringContent(JsonConvert.SerializeObject(newLocation), System.Text.Encoding.UTF8, "application/json");
            var postResponse = await _client.PostAsync("/api/location", content);
            postResponse.EnsureSuccessStatusCode();
            var createdContent = await postResponse.Content.ReadAsStringAsync();
            var createdLocation = JsonConvert.DeserializeObject<LocationDTO>(createdContent);
            var createdId = createdLocation.Id;

            var getResponse = await _client.GetAsync($"/api/location/{createdId}");

            getResponse.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var getContent = await getResponse.Content.ReadAsStringAsync();
            var location = JsonConvert.DeserializeObject<LocationDTO>(getContent);

            Assert.NotNull(location);
            Assert.Equal(createdId, location.Id);
            Assert.Equal("Berlin", location.City);
            Assert.Equal("Germany", location.Country);
        }

        // Test: Post a new location and verify creation
        [Fact]
        public async Task PostLocation_CreatesNewLocation()
        {
            var newLocation = new LocationCreateDTO
            {
                City = "Paris",
                Country = "France"
            };

            var content = new StringContent(JsonConvert.SerializeObject(newLocation), System.Text.Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/location", content);

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var responseContent = await response.Content.ReadAsStringAsync();
            var createdLocation = JsonConvert.DeserializeObject<LocationDTO>(responseContent);

            Assert.NotNull(createdLocation);
            Assert.Equal("Paris", createdLocation.City);
            Assert.Equal("France", createdLocation.Country);
        }

        // Test: Update an existing location
        [Fact]
        public async Task PutLocation_UpdatesExistingLocation()
        {
            var newLocation = new LocationCreateDTO
            {
                City = "Rome",
                Country = "Italy"
            };

            var postContent = new StringContent(JsonConvert.SerializeObject(newLocation), System.Text.Encoding.UTF8, "application/json");
            var postResponse = await _client.PostAsync("/api/location", postContent);
            postResponse.EnsureSuccessStatusCode();

            var postCreatedContent = await postResponse.Content.ReadAsStringAsync();
            var createdLocation = JsonConvert.DeserializeObject<LocationDTO>(postCreatedContent);
            var createdId = createdLocation.Id;

            var updatedLocation = new LocationUpdateDTO
            {
                City = "Milan",
                Country = "Italy"
            };

            var updateContent = new StringContent(JsonConvert.SerializeObject(updatedLocation), System.Text.Encoding.UTF8, "application/json");

            var putResponse = await _client.PutAsync($"/api/location/{createdId}", updateContent);

            Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);

            var getResponse = await _client.GetAsync($"/api/location/{createdId}");
            var getContent = await getResponse.Content.ReadAsStringAsync();
            var updatedLocationResult = JsonConvert.DeserializeObject<LocationDTO>(getContent);

            Assert.NotNull(updatedLocationResult);
            Assert.Equal("Milan", updatedLocationResult.City);
            Assert.Equal("Italy", updatedLocationResult.Country);
        }

        // Test: Delete an existing location
        [Fact]
        public async Task DeleteLocation_DeletesExistingLocation()
        {
            var newLocation = new LocationCreateDTO
            {
                City = "Tokyo",
                Country = "Japan"
            };

            var postContent = new StringContent(JsonConvert.SerializeObject(newLocation), System.Text.Encoding.UTF8, "application/json");
            var postResponse = await _client.PostAsync("/api/location", postContent);
            postResponse.EnsureSuccessStatusCode();

            var postCreatedContent = await postResponse.Content.ReadAsStringAsync();
            var createdLocation = JsonConvert.DeserializeObject<LocationDTO>(postCreatedContent);
            var createdId = createdLocation.Id;

            var deleteResponse = await _client.DeleteAsync($"/api/location/{createdId}");

            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            var getResponse = await _client.GetAsync($"/api/location/{createdId}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        // New Tests for Error Handling

        // Test: Create Location with Missing Required Fields (400 Bad Request)
        [Fact]
        public async Task PostLocation_ReturnsBadRequest_WhenRequiredFieldsAreMissing()
        {
            var invalidLocation = new LocationCreateDTO
            {
                Country = "France"  // Missing City
            };

            var content = new StringContent(JsonConvert.SerializeObject(invalidLocation), System.Text.Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/location", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        // Test: Create Location with Invalid Data Type (400 Bad Request)
        [Fact]
        public async Task PostLocation_ReturnsBadRequest_WhenInvalidDataTypeIsProvided()
        {
            var invalidLocation = new
            {
                City = 12345,  // Invalid data type (should be a string)
                Country = "France"
            };

            var content = new StringContent(JsonConvert.SerializeObject(invalidLocation), System.Text.Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/location", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        // Test: Get Non-existent Location (404 Not Found)
        [Fact]
        public async Task GetLocationById_ReturnsNotFound_WhenLocationDoesNotExist()
        {
            var response = await _client.GetAsync("/api/location/999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        // Test: Update Non-existent Location (404 Not Found)
        [Fact]
        public async Task PutLocation_ReturnsNotFound_WhenLocationDoesNotExist()
        {
            var updateLocation = new LocationUpdateDTO
            {
                City = "Non-existent City",
                Country = "Non-existent Country"
            };

            var content = new StringContent(JsonConvert.SerializeObject(updateLocation), System.Text.Encoding.UTF8, "application/json");
            var response = await _client.PutAsync("/api/location/999", content);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        // Test: Delete Non-existent Location (404 Not Found)
        [Fact]
        public async Task DeleteLocation_ReturnsNotFound_WhenLocationDoesNotExist()
        {
            var response = await _client.DeleteAsync("/api/location/999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
