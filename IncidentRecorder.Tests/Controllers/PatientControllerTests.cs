using Xunit;
using IncidentRecorder.Controllers;
using IncidentRecorder.Models;
using IncidentRecorder.DTOs.Patient;
using Microsoft.AspNetCore.Mvc;
using IncidentRecorder.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace IncidentRecorder.Tests.Controllers
{
    public class PatientControllerTests
    {
        // Helper method to create a new context with a unique in-memory database
        private IncidentContext GetInMemoryDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<IncidentContext>()
                          .UseInMemoryDatabase(databaseName: dbName) // Ensure a unique database per test
                          .Options;

            var context = new IncidentContext(options);

            // Seed initial data into the in-memory database with required fields
            context.Patients.Add(new Patient
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                ContactInfo = "john.doe@example.com",
                Gender = "Male"
            });
            context.SaveChanges();

            return context;
        }

        [Fact]
        public async Task GetPatient_ReturnsNotFound_WhenPatientDoesNotExist()
        {
            // Arrange: create a new in-memory context with a unique database name
            var context = GetInMemoryDbContext("TestDb1");
            var controller = new PatientController(context);

            // Act: try to retrieve a patient with an ID that does not exist
            var result = await controller.GetPatient(999);

            // Assert: check that a NotFoundResult is returned
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetPatient_ReturnsPatientDTO_WhenPatientExists()
        {
            // Arrange: create a new in-memory context with a unique database name
            var context = GetInMemoryDbContext("TestDb2");
            var controller = new PatientController(context);

            // Act: retrieve an existing patient
            var result = await controller.GetPatient(1);

            // Assert: check that the result contains the expected PatientDTO
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var patientDTO = Assert.IsType<PatientDTO>(actionResult.Value);
            Assert.Equal("John", patientDTO.FirstName);
        }
    }
}
