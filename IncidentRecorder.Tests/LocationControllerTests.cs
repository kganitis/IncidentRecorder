using IncidentRecorder.Controllers;
using IncidentRecorder.DTOs.Location;
using Microsoft.AspNetCore.Mvc;

namespace IncidentRecorder.Tests.Unit
{
    public class LocationControllerTests : BaseTest
    {
        // Test: Get all locations
        [Fact]
        public async Task GetLocations_ReturnsLocationDTOList()
        {
            var context = GetInMemoryDbContext("TestDb1");
            var controller = new LocationController(context);

            var result = await controller.GetLocations();

            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var locations = Assert.IsType<List<LocationDTO>>(actionResult.Value);
            Assert.Single(locations);
            Assert.Equal("New York", locations[0].City);
        }

        // Test: Get a single location by ID
        [Fact]
        public async Task GetLocation_ReturnsLocationDTO_WhenLocationExists()
        {
            var context = GetInMemoryDbContext("TestDb2");
            var controller = new LocationController(context);

            var result = await controller.GetLocation(1);

            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var location = Assert.IsType<LocationDTO>(actionResult.Value);
            Assert.Equal("New York", location.City);
        }

        // Test: Get a single location by ID (not found)
        [Fact]
        public async Task GetLocation_ReturnsNotFound_WhenLocationDoesNotExist()
        {
            var context = GetInMemoryDbContext("TestDb3");
            var controller = new LocationController(context);

            var result = await controller.GetLocation(999);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        // Test: Create a new location
        [Fact]
        public async Task PostLocation_ReturnsCreatedAtAction_WhenLocationIsCreated()
        {
            var context = GetInMemoryDbContext("TestDb4");
            var controller = new LocationController(context);

            var newLocation = new LocationCreateDTO
            {
                City = "Berlin",
                Country = "Germany"
            };

            var result = await controller.PostLocation(newLocation);

            var actionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdLocation = Assert.IsType<LocationDTO>(actionResult.Value);
            Assert.Equal("Berlin", createdLocation.City);
        }

        // Test: Update an existing location
        [Fact]
        public async Task PutLocation_ReturnsNoContent_WhenLocationIsUpdated()
        {
            var context = GetInMemoryDbContext("TestDb5");
            var controller = new LocationController(context);

            var updateLocation = new LocationUpdateDTO
            {
                City = "Berlin"
            };

            var result = await controller.PutLocation(1, updateLocation);

            Assert.IsType<NoContentResult>(result);
        }

        // Test: Update a non-existent location
        [Fact]
        public async Task PutLocation_ReturnsNotFound_WhenLocationDoesNotExist()
        {
            var context = GetInMemoryDbContext("TestDb6");
            var controller = new LocationController(context);

            var updateLocation = new LocationUpdateDTO
            {
                City = "Berlin"
            };

            var result = await controller.PutLocation(999, updateLocation);

            Assert.IsType<NotFoundResult>(result);
        }

        // Test: Delete an existing location
        [Fact]
        public async Task DeleteLocation_ReturnsNoContent_WhenLocationIsDeleted()
        {
            var context = GetInMemoryDbContext("TestDb7");
            var controller = new LocationController(context);

            var result = await controller.DeleteLocation(1);

            Assert.IsType<NoContentResult>(result);
        }

        // Test: Delete a non-existent location
        [Fact]
        public async Task DeleteLocation_ReturnsNotFound_WhenLocationDoesNotExist()
        {
            var context = GetInMemoryDbContext("TestDb8");
            var controller = new LocationController(context);

            var result = await controller.DeleteLocation(999);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
