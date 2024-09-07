using IncidentRecorder.Controllers;
using IncidentRecorder.DTOs.Patient;
using Microsoft.AspNetCore.Mvc;

namespace IncidentRecorder.Tests.Unit
{
    public class PatientControllerTests : BaseTest
    {
        // Test: Get a patient that does not exist
        [Fact]
        public async Task GetPatient_ReturnsNotFound_WhenPatientDoesNotExist()
        {
            var context = GetInMemoryDbContext("TestDb1");
            var controller = new PatientController(context);

            var result = await controller.GetPatient(999);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        // Test: Get an existing patient
        [Fact]
        public async Task GetPatient_ReturnsPatientDTO_WhenPatientExists()
        {
            var context = GetInMemoryDbContext("TestDb2");
            var controller = new PatientController(context);

            var result = await controller.GetPatient(1);

            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var patientDTO = Assert.IsType<PatientDTO>(actionResult.Value);
            Assert.Equal("John", patientDTO.FirstName);
        }

        // Test: Create a new patient
        [Fact]
        public async Task PostPatient_ReturnsCreatedAtAction_WhenPatientIsCreated()
        {
            var context = GetInMemoryDbContext("TestDb3");
            var controller = new PatientController(context);

            // Arrange: Create a new patient
            var newPatient = new PatientCreateDTO
            {
                FirstName = "Jane",
                LastName = "Doe",
                DateOfBirth = new System.DateTime(1990, 5, 14),
                ContactInfo = "jane.doe@example.com",
                Gender = "Female"
            };

            // Act
            var result = await controller.PostPatient(newPatient);

            // Assert
            var actionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdPatient = Assert.IsType<PatientDTO>(actionResult.Value);
            Assert.Equal("Jane", createdPatient.FirstName);
        }

        // Test: Update an existing patient
        [Fact]
        public async Task PutPatient_ReturnsNoContent_WhenPatientIsUpdated()
        {
            var context = GetInMemoryDbContext("TestDb4");
            var controller = new PatientController(context);

            // Act: Update an existing patient
            var updatePatient = new PatientUpdateDTO
            {
                FirstName = "Johnathan"
            };

            var result = await controller.PutPatient(1, updatePatient);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        // Test: Update a non-existent patient
        [Fact]
        public async Task PutPatient_ReturnsNotFound_WhenPatientDoesNotExist()
        {
            var context = GetInMemoryDbContext("TestDb5");
            var controller = new PatientController(context);

            var updatePatient = new PatientUpdateDTO
            {
                FirstName = "Johnathan"
            };

            var result = await controller.PutPatient(999, updatePatient);

            Assert.IsType<NotFoundResult>(result);
        }

        // Test: Delete an existing patient
        [Fact]
        public async Task DeletePatient_ReturnsNoContent_WhenPatientIsDeleted()
        {
            var context = GetInMemoryDbContext("TestDb6");
            var controller = new PatientController(context);

            var result = await controller.DeletePatient(1);

            Assert.IsType<NoContentResult>(result);
        }

        // Test: Delete a non-existent patient
        [Fact]
        public async Task DeletePatient_ReturnsNotFound_WhenPatientDoesNotExist()
        {
            var context = GetInMemoryDbContext("TestDb7");
            var controller = new PatientController(context);

            var result = await controller.DeletePatient(999);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
