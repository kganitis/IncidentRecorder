using System.Net;
using IncidentRecorder.DTOs.Location;
using Microsoft.AspNetCore.Mvc.Testing;

namespace IncidentRecorder.Tests.Integration
{
    public class LocationControllerIntegrationTests(WebApplicationFactory<Program> factory) : BaseIntegrationTest(factory)
    {
        private const string LocationApiUrl = "/api/location";

        private void AssertLocation(LocationDTO location, int id, string city, string country)
        {
            Assert.NotNull(location);
            Assert.Equal(id, location.Id);
            Assert.Equal(city, location.City);
            Assert.Equal(country, location.Country);
        }

        [Fact]
        public async Task GetLocations_ReturnsOkResult_WithSeededData()
        {
            // Act
            var response = await _client.GetAsync(LocationApiUrl);

            // Assert
            response.EnsureSuccessStatusCode();
            var locations = await DeserializeResponse<List<LocationDTO>>(response);
            Assert.NotNull(locations);

            for (int i = 0; i < SeededLocations.Count; i++)
            {
                var actual = locations[i];
                var expected = SeededLocations[i];
                AssertLocation(actual, expected.Id, expected.City, expected.Country);
            }
        }

        [Fact]
        public async Task GetLocationById_ReturnsOkResult_WhenLocationExists()
        {
            // Act
            var response = await _client.GetAsync($"{LocationApiUrl}/{SeededLocations[0].Id}");

            // Assert
            var actual = await DeserializeResponse<LocationDTO>(response);
            var expected = SeededLocations[0];
            AssertLocation(actual, expected.Id, expected.City, expected.Country);
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
            var expectedId = createdLocation.Id;
            AssertLocation(createdLocation, expectedId, newLocation.City, newLocation.Country);
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

            AssertLocation(updatedLocationResult, createdId, updatedLocation.City, updatedLocation.Country);

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
            var invalidLocation1 = "{\"city\":\"Athens\"}";
            var invalidLocation2 = "{\"country\":\"Greece\"}";

            var content1 = new StringContent(invalidLocation1, System.Text.Encoding.UTF8, "application/json");
            var content2 = new StringContent(invalidLocation2, System.Text.Encoding.UTF8, "application/json");

            // Act
            var response1 = await _client.PostAsync(LocationApiUrl, content1);
            var response2 = await _client.PostAsync(LocationApiUrl, content2);

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

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
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
