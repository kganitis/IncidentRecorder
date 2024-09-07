using IncidentRecorder.Controllers;
using IncidentRecorder.DTOs.Symptom;
using Microsoft.AspNetCore.Mvc;

namespace IncidentRecorder.Tests.Unit
{
    public class SymptomControllerTests : BaseTest
    {
        // Test: Get all symptoms
        [Fact]
        public async Task GetSymptoms_ReturnsSymptomDTOList()
        {
            var context = GetInMemoryDbContext("TestDb1");
            var controller = new SymptomController(context);

            var result = await controller.GetSymptoms();

            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var symptoms = Assert.IsType<List<SymptomDTO>>(actionResult.Value);
            Assert.Single(symptoms);
            Assert.Equal("Cough", symptoms[0].Name);
        }

        // Test: Get a single symptom by ID
        [Fact]
        public async Task GetSymptom_ReturnsSymptomDTO_WhenSymptomExists()
        {
            var context = GetInMemoryDbContext("TestDb2");
            var controller = new SymptomController(context);

            var result = await controller.GetSymptom(1);

            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var symptom = Assert.IsType<SymptomDTO>(actionResult.Value);
            Assert.Equal("Cough", symptom.Name);
        }

        // Test: Get a single symptom by ID (not found)
        [Fact]
        public async Task GetSymptom_ReturnsNotFound_WhenSymptomDoesNotExist()
        {
            var context = GetInMemoryDbContext("TestDb3");
            var controller = new SymptomController(context);

            var result = await controller.GetSymptom(999);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        // Test: Create a new symptom
        [Fact]
        public async Task PostSymptom_ReturnsCreatedAtAction_WhenSymptomIsCreated()
        {
            var context = GetInMemoryDbContext("TestDb4");
            var controller = new SymptomController(context);

            var newSymptom = new SymptomCreateDTO
            {
                Name = "Cough",
                Description = "Persistent cough"
            };

            var result = await controller.PostSymptom(newSymptom);

            var actionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdSymptom = Assert.IsType<SymptomDTO>(actionResult.Value);
            Assert.Equal("Cough", createdSymptom.Name);
        }

        // Test: Update an existing symptom
        [Fact]
        public async Task PutSymptom_ReturnsNoContent_WhenSymptomIsUpdated()
        {
            var context = GetInMemoryDbContext("TestDb5");
            var controller = new SymptomController(context);

            var updateSymptom = new SymptomUpdateDTO
            {
                Name = "Fever",
                Description = "Mild fever"
            };

            var result = await controller.PutSymptom(1, updateSymptom);

            Assert.IsType<NoContentResult>(result);
        }

        // Test: Update a non-existent symptom
        [Fact]
        public async Task PutSymptom_ReturnsNotFound_WhenSymptomDoesNotExist()
        {
            var context = GetInMemoryDbContext("TestDb6");
            var controller = new SymptomController(context);

            var updateSymptom = new SymptomUpdateDTO
            {
                Name = "Headache",
                Description = "Severe headache"
            };

            var result = await controller.PutSymptom(999, updateSymptom);

            Assert.IsType<NotFoundResult>(result);
        }

        // Test: Delete an existing symptom
        [Fact]
        public async Task DeleteSymptom_ReturnsNoContent_WhenSymptomIsDeleted()
        {
            var context = GetInMemoryDbContext("TestDb7");
            var controller = new SymptomController(context);

            var result = await controller.DeleteSymptom(1);

            Assert.IsType<NoContentResult>(result);
        }

        // Test: Delete a non-existent symptom
        [Fact]
        public async Task DeleteSymptom_ReturnsNotFound_WhenSymptomDoesNotExist()
        {
            var context = GetInMemoryDbContext("TestDb8");
            var controller = new SymptomController(context);

            var result = await controller.DeleteSymptom(999);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
