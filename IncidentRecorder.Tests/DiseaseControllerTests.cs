using IncidentRecorder.Controllers;
using IncidentRecorder.DTOs.Disease;
using Microsoft.AspNetCore.Mvc;

namespace IncidentRecorder.Tests.Unit
{
    public class DiseaseControllerTests : BaseTest
    {
        // Test: Get all diseases
        [Fact]
        public async Task GetDiseases_ReturnsDiseaseDTOList()
        {
            var context = GetInMemoryDbContext("TestDb1");
            var controller = new DiseaseController(context);

            var result = await controller.GetDiseases();

            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var diseases = Assert.IsType<List<DiseaseDTO>>(actionResult.Value);
            Assert.Single(diseases);
            Assert.Equal("COVID-19", diseases[0].Name);
        }

        // Test: Get a single disease by ID
        [Fact]
        public async Task GetDisease_ReturnsDiseaseDTO_WhenDiseaseExists()
        {
            var context = GetInMemoryDbContext("TestDb2");
            var controller = new DiseaseController(context);

            var result = await controller.GetDisease(1);

            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var disease = Assert.IsType<DiseaseDTO>(actionResult.Value);
            Assert.Equal("COVID-19", disease.Name);
        }

        // Test: Get a single disease by ID (not found)
        [Fact]
        public async Task GetDisease_ReturnsNotFound_WhenDiseaseDoesNotExist()
        {
            var context = GetInMemoryDbContext("TestDb3");
            var controller = new DiseaseController(context);

            var result = await controller.GetDisease(999);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        // Test: Create a new disease
        [Fact]
        public async Task PostDisease_ReturnsCreatedAtAction_WhenDiseaseIsCreated()
        {
            var context = GetInMemoryDbContext("TestDb4");
            var controller = new DiseaseController(context);

            var newDisease = new DiseaseDTO
            {
                Name = "COVID-19",
                Description = "Coronavirus disease"
            };

            var result = await controller.PostDisease(newDisease);

            var actionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdDisease = Assert.IsType<DiseaseDTO>(actionResult.Value);
            Assert.Equal("COVID-19", createdDisease.Name);
        }

        // Test: Update an existing disease
        [Fact]
        public async Task PutDisease_ReturnsNoContent_WhenDiseaseIsUpdated()
        {
            var context = GetInMemoryDbContext("TestDb5");
            var controller = new DiseaseController(context);

            var updateDisease = new DiseaseUpdateDTO
            {
                Name = "Influenza"
            };

            var result = await controller.PutDisease(1, updateDisease);

            Assert.IsType<NoContentResult>(result);
        }

        // Test: Update a non-existent disease
        [Fact]
        public async Task PutDisease_ReturnsNotFound_WhenDiseaseDoesNotExist()
        {
            var context = GetInMemoryDbContext("TestDb6");
            var controller = new DiseaseController(context);

            var updateDisease = new DiseaseUpdateDTO
            {
                Name = "Ebola"
            };

            var result = await controller.PutDisease(999, updateDisease);

            Assert.IsType<NotFoundResult>(result);
        }

        // Test: Delete an existing disease
        [Fact]
        public async Task DeleteDisease_ReturnsNoContent_WhenDiseaseIsDeleted()
        {
            var context = GetInMemoryDbContext("TestDb7");
            var controller = new DiseaseController(context);

            var result = await controller.DeleteDisease(1);

            Assert.IsType<NoContentResult>(result);
        }

        // Test: Delete a non-existent disease
        [Fact]
        public async Task DeleteDisease_ReturnsNotFound_WhenDiseaseDoesNotExist()
        {
            var context = GetInMemoryDbContext("TestDb8");
            var controller = new DiseaseController(context);

            var result = await controller.DeleteDisease(999);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
