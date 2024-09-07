using System.Net;
using IncidentRecorder.DTOs.Location;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using IncidentRecorder.Tests.Integration;

namespace IncidentRecorder.Tests.IntegrationTests
{
    public class LocationControllerIntegrationTests : BaseIntegrationTest
    {
        public LocationControllerIntegrationTests(WebApplicationFactory<Program> factory) : base(factory) { }

        // Test: Get all locations with seeded data
        [Fact]
        public async Task GetLocations_ReturnsOkResult_WithSeededData()
        {
            // Act
            var response = await _client.GetAsync("/api/location");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var locations = JsonConvert.DeserializeObject<List<LocationDTO>>(content);

            // Verify the seeded location is returned
            Assert.NotNull(locations);
            Assert.Contains(locations, l => l.City == "New York" && l.Country == "USA");
        }

        // Test: Get a single location by dynamically capturing ID after creation
        [Fact]
        public async Task GetLocationById_ReturnsOkResult_WhenLocationExists()
        {
            // Arrange: Create a new location to get its ID
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

            // Act: Fetch the location by the captured ID
            var getResponse = await _client.GetAsync($"/api/location/{createdId}");

            // Assert
            getResponse.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var getContent = await getResponse.Content.ReadAsStringAsync();
            var location = JsonConvert.DeserializeObject<LocationDTO>(getContent);

            // Verify the location details
            Assert.NotNull(location);
            Assert.Equal(createdId, location.Id);
            Assert.Equal("Berlin", location.City);
            Assert.Equal("Germany", location.Country);
        }

        // Test: Post a new location and verify creation
        [Fact]
        public async Task PostLocation_CreatesNewLocation()
        {
            // Arrange
            var newLocation = new LocationCreateDTO
            {
                City = "Paris",
                Country = "France"
            };

            var content = new StringContent(JsonConvert.SerializeObject(newLocation), System.Text.Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/location", content);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            // Deserialize the created location to verify the content
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
            // Arrange: Create a new location to get its ID
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

            // Arrange: Prepare update data
            var updatedLocation = new LocationUpdateDTO
            {
                City = "Milan",
                Country = "Italy"
            };

            var updateContent = new StringContent(JsonConvert.SerializeObject(updatedLocation), System.Text.Encoding.UTF8, "application/json");

            // Act: Update the location
            var putResponse = await _client.PutAsync($"/api/location/{createdId}", updateContent);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);

            // Verify that the location was updated by fetching it again
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
            // Arrange: Create a new location to get its ID
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

            // Act: Delete the location
            var deleteResponse = await _client.DeleteAsync($"/api/location/{createdId}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            // Verify that the location no longer exists
            var getResponse = await _client.GetAsync($"/api/location/{createdId}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }
    }
}
