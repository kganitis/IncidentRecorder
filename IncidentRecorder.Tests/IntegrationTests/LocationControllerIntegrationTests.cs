using System.Net;
using IncidentRecorder.DTOs.Location;
using Microsoft.AspNetCore.Mvc.Testing;

namespace IncidentRecorder.Tests.Integration
{
    public class LocationControllerIntegrationTests : BaseIntegrationTest
    {
        private const string LocationApiUrl = "/api/location";

        public LocationControllerIntegrationTests(WebApplicationFactory<Program> factory) : base(factory) { }

        [Fact]
        public async Task GetLocations_ReturnsOkResult_WithSeededData()
        {
            // Act
            var response = await _client.GetAsync(LocationApiUrl);

            // Assert
            response.EnsureSuccessStatusCode();
            var locations = await DeserializeResponse<List<LocationDTO>>(response);
            Assert.NotNull(locations);

            var expectedLocations = new[]
            {
                new { Id = 1, City = "New York", Country = "USA" },
                // TODO check for all the locations
                new { Id = 6, City = "Dublin", Country = "Ireland" }
            };

            foreach (var expected in expectedLocations)
            {
                Assert.Contains(locations, i => i.Id == expected.Id && i.City == expected.City && i.Country == expected.Country);
            }
        }

        [Theory]
        [InlineData(1, "New York, USA")]
        [InlineData(2, "Toronto, Canada")]
        [InlineData(3, "Madrid, Spain")]
        [InlineData(4, "London, UK")]
        [InlineData(5, "Sydney, Australia")]
        [InlineData(6, "Dublin, Ireland")]
        public async Task GetLocationById_ReturnsOkResult_WhenLocationExists(int id, string cityCountry)
        {
            // Act
            var response = await _client.GetAsync($"{LocationApiUrl}/{id}");

            // Assert
            var location = await DeserializeResponse<LocationDTO>(response);
            Assert.NotNull(location);
            Assert.Equal(id, location.Id);
            Assert.Equal(cityCountry, $"{location.City}, {location.Country}");
        }

        [Fact]
        public async Task PostLocation_CreatesNewLocation()
        {
            // Arrange: Prepare new location data
            var newLocation = new LocationCreateDTO { City = "Tokyo", Country = "Japan" };

            // Act
            var response = await _client.PostAsync(LocationApiUrl, CreateContent(newLocation));

            // Assert
            response.EnsureSuccessStatusCode();
            var createdLocation = await DeserializeResponse<LocationDTO>(response);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(createdLocation);
            Assert.Equal("Tokyo", createdLocation.City);
            Assert.Equal("Japan", createdLocation.Country);
        }

        [Fact]
        public async Task PutLocation_UpdatesExistingLocation()
        {
            // Arrange: Create a new location to update
            var newLocation = new LocationCreateDTO { City = "Test City", Country = "Test Country" };

            // Get the ID of the newly created location
            var postResponse = await _client.PostAsync(LocationApiUrl, CreateContent(newLocation));
            postResponse.EnsureSuccessStatusCode();
            var createdLocation = await DeserializeResponse<LocationDTO>(postResponse);
            var createdId = createdLocation.Id;

            // Arrange: Prepare updated location data
            var updatedLocation = new LocationUpdateDTO { City = "Updated City", Country = "Updated Country" };

            // Act
            var putResponse = await _client.PutAsync($"{LocationApiUrl}/{createdId}", CreateContent(updatedLocation));

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);

            // Verify that the location was updated by fetching it again
            var getResponse = await _client.GetAsync($"{LocationApiUrl}/{createdId}");
            var updatedLocationResult = await DeserializeResponse<LocationDTO>(getResponse);

            Assert.NotNull(updatedLocationResult);
            Assert.Equal("Updated City", updatedLocationResult.City);
            Assert.Equal("Updated Country", updatedLocationResult.Country);

            // Delete the updated location to clean up
            var deleteResponse = await _client.DeleteAsync($"{LocationApiUrl}/{createdId}");
            deleteResponse.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task DeleteLocation_DeletesExistingLocation()
        {
            // Arrange: Create a new location to delete
            var newLocation = new LocationCreateDTO { City = "Test City", Country = "Test Country" };

            // Get the ID of the newly created location
            var postResponse = await _client.PostAsync(LocationApiUrl, CreateContent(newLocation));
            postResponse.EnsureSuccessStatusCode();
            var createdLocation = await DeserializeResponse<LocationDTO>(postResponse);
            var createdId = createdLocation.Id;

            // Act
            var deleteResponse = await _client.DeleteAsync($"{LocationApiUrl}/{createdId}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            // Verify that the location no longer exists
            var getResponse = await _client.GetAsync($"{LocationApiUrl}/{createdId}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task PostLocation_ReturnsBadRequest_WhenRequiredFieldsAreMissing()
        {
            // Arrange: Create new locations with missing fields
            var invalidLocation1 = new LocationCreateDTO { Country = "Missing City" };
            var invalidLocation2 = new LocationCreateDTO { City = "Missing Country" };

            // Act
            var response1 = await _client.PostAsync(LocationApiUrl, CreateContent(invalidLocation1));
            var response2 = await _client.PostAsync(LocationApiUrl, CreateContent(invalidLocation2));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response1.StatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, response2.StatusCode);
        }

        [Fact]
        public async Task PostLocation_ReturnsBadRequest_WhenInvalidDataTypeIsProvided()
        {
            // Arrange: Provide an integer where a string is expected
            var invalidLocation1 = new { City = 12345, Country = "Invalid city type" };
            var invalidLocation2 = new { City = "Invalid country type", Country = 12345 };

            // Act
            var response1 = await _client.PostAsync(LocationApiUrl, CreateContent(invalidLocation1));
            var response2 = await _client.PostAsync(LocationApiUrl, CreateContent(invalidLocation2));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response1.StatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, response2.StatusCode);
        }

        [Fact]
        public async Task PutLocation_ReturnsBadRequest_WhenInvalidDataTypeIsProvided()
        {
            // Arrange: Provide an invalid payload with an integer where a string is expected
            var invalidLocation1 = "{ \"city\": 12345, \"country\": \"Invalid city type\"}";
            var invalidLocation2 = "{ \"city\": \"Invalid country type\", \"country\": 12345 }";

            var content1 = new StringContent(invalidLocation1, System.Text.Encoding.UTF8, "application/json");
            var content2 = new StringContent(invalidLocation2, System.Text.Encoding.UTF8, "application/json");

            // Act
            var response1 = await _client.PutAsync($"{LocationApiUrl}/1", content1);
            var response2 = await _client.PutAsync($"{LocationApiUrl}/2", content2);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response1.StatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, response2.StatusCode);
        }

        [Fact]
        public async Task GetLocationById_ReturnsNotFound_WhenLocationDoesNotExist()
        {
            // Act: Try to get a non-existing location
            var response = await _client.GetAsync($"{LocationApiUrl}/999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task PutLocation_ReturnsNotFound_WhenLocationDoesNotExist()
        {
            // Arrange
            var updateLocation = new LocationUpdateDTO { };

            // Act: Try to update a non-existing location
            var response = await _client.PutAsync($"{LocationApiUrl}/999", CreateContent(updateLocation));

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteLocation_ReturnsNotFound_WhenLocationDoesNotExist()
        {
            // Act: Try to delete a non-existing location
            var response = await _client.DeleteAsync($"{LocationApiUrl}/999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetLocation_ReturnsBadRequest_WithInvalidID()
        {
            // Act: Send the request with an invalid ID in the URL (e.g., a string instead of an integer)
            var response = await _client.GetAsync($"{LocationApiUrl}/invalid-id");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PutLocation_ReturnsBadRequest_WithInvalidID()
        {
            // Arrange
            var updateLocation = new LocationUpdateDTO { };

            // Act: Send the request with an invalid ID in the URL (e.g., a string instead of an integer)
            var response = await _client.PutAsync($"{LocationApiUrl}/invalid-id", CreateContent(updateLocation));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task DeleteLocation_ReturnsBadRequest_WithInvalidID()
        {
            // Act: Send the request with an invalid ID in the URL (e.g., a string instead of an integer)
            var response = await _client.DeleteAsync($"{LocationApiUrl}/invalid-id");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PostLocation_ReturnsBadRequest_WhenPayloadIsEmpty()
        {
            // Arrange: Create an empty payload
            var emptyPayload = "{}";

            var content = new StringContent(emptyPayload, System.Text.Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync(LocationApiUrl, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains("The City field is required.", responseContent);
            Assert.Contains("The Country field is required.", responseContent);
        }

        [Fact]
        public async Task PutLocation_ReturnsNoContent_WhenPayloadIsEmpty()
        {
            // Arrange: Fetch an existing location
            var getResponseBeforeUpdate = await _client.GetAsync($"{LocationApiUrl}/1");
            getResponseBeforeUpdate.EnsureSuccessStatusCode();

            var locationBeforeUpdate = await DeserializeResponse<LocationDTO>(getResponseBeforeUpdate);

            // Act: Send an empty payload to update the location
            var emptyPayload = "{}"; // Empty JSON object
            var updateContent = new StringContent(emptyPayload, System.Text.Encoding.UTF8, "application/json");

            var putResponse = await _client.PutAsync($"{LocationApiUrl}/1", updateContent);

            // Assert: The response should be 204 No Content
            Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);

            // Act: Fetch the location again after the update
            var getResponseAfterUpdate = await _client.GetAsync($"{LocationApiUrl}/1");
            var locationAfterUpdate = await DeserializeResponse<LocationDTO>(getResponseAfterUpdate);

            // Assert: The location should remain unchanged
            Assert.Equal(locationBeforeUpdate.City, locationAfterUpdate.City);
            Assert.Equal(locationBeforeUpdate.Country, locationAfterUpdate.Country);
        }

        [Fact]
        public async Task PostLocation_ReturnsConflict_WhenLocationCityIsNotUnique()
        {
            // Arrange: Create an existing location
            var duplicateLocation = new LocationCreateDTO { City = "New York",  Country = "USA" };

            // Act: Try to create a duplicate location
            var response = await _client.PostAsync(LocationApiUrl, CreateContent(duplicateLocation));

            // Assert: Conflict due to uniqueness violation
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task PutLocation_ReturnsConflict_WhenLocationCityIsNotUnique()
        {
            // Arrange: Prepare an existing location
            var duplicateLocation = new LocationUpdateDTO { City = "New York", Country = "USA" };

            // Act: Try to update a location with the duplicate data
            var response = await _client.PutAsync($"{LocationApiUrl}/2", CreateContent(duplicateLocation));

            // Assert: Conflict due to uniqueness violation
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }
    }
}
