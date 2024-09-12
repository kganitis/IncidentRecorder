using IncidentRecorder.Controllers;
using IncidentRecorder.Models;
using IncidentRecorder.DTOs.Incident;
using Microsoft.AspNetCore.Mvc;

namespace IncidentRecorder.Tests.Unit
{
    public class IncidentControllerTests : BaseTest
    {
        // Test: Get all incidents
        [Fact]
        public async Task GetIncidents_ReturnsIncidentDTOList()
        {
            var context = GetInMemoryDbContext("TestDb1");
            var controller = new IncidentController(context);

            var result = await controller.GetIncidents();

            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var incidents = Assert.IsType<List<IncidentReadDTO>>(actionResult.Value);
            Assert.Equal(2, incidents.Count);
            Assert.Equal("COVID-19", incidents[0].DiseaseName);
            Assert.Equal("Kostas Ganitis", incidents[0].PatientName);
            Assert.Equal("Athens, Greece", incidents[0].Location);
            Assert.Equal("Cough", incidents[0].Symptoms?[0]);
            Assert.Equal("Fever", incidents[0].Symptoms?[1]);
            Assert.Equal("Gastroenteritis", incidents[1].DiseaseName);
            Assert.Equal("Efthymios Alepis", incidents[1].PatientName);
            Assert.Equal("Piraeus, Greece", incidents[1].Location);
            Assert.Equal("Nausea", incidents[1].Symptoms?[0]);
        }

        // Test: Get a single incident by ID
        [Fact]
        public async Task GetIncident_ReturnsIncidentDTO_WhenIncidentExists()
        {
            var context = GetInMemoryDbContext("TestDb2");
            var controller = new IncidentController(context);

            var result = await controller.GetIncident(1);

            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var incident = Assert.IsType<IncidentReadDTO>(actionResult.Value);
            Assert.Equal("COVID-19", incident.DiseaseName);
        }

        // Test: Get a single incident by ID (not found)
        [Fact]
        public async Task GetIncident_ReturnsNotFound_WhenIncidentDoesNotExist()
        {
            var context = GetInMemoryDbContext("TestDb3");
            var controller = new IncidentController(context);

            var result = await controller.GetIncident(999);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        // Test: Create a new incident
        [Fact]
        public async Task PostIncident_ReturnsCreatedAtAction_WhenIncidentIsCreated()
        {
            var context = GetInMemoryDbContext("TestDb4");
            var controller = new IncidentController(context);

            var newIncident = new IncidentCreateDTO
            {
                DiseaseId = 1,
                PatientId = 1,
                LocationId = 1,
                DateReported = DateTime.Now,
                SymptomIds = [1]
            };

            var result = await controller.PostIncident(newIncident);

            var actionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdIncident = Assert.IsType<IncidentReadDTO>(actionResult.Value);
            Assert.Equal("COVID-19", createdIncident.DiseaseName);
        }

        // Test: Update an existing incident
        [Fact]
        public async Task PutIncident_ReturnsNoContent_WhenIncidentIsUpdated()
        {
            var context = GetInMemoryDbContext("TestDb5");
            var controller = new IncidentController(context);

            var updateIncident = new IncidentUpdateDTO
            {
                DiseaseId = 1,
                PatientId = 1,
                LocationId = 1,
                DateReported = DateTime.Now.AddDays(-1)
            };

            var result = await controller.PutIncident(1, updateIncident);

            Assert.IsType<NoContentResult>(result);
        }

        // Test: Update a non-existent incident
        [Fact]
        public async Task PutIncident_ReturnsNotFound_WhenIncidentDoesNotExist()
        {
            var context = GetInMemoryDbContext("TestDb6");
            var controller = new IncidentController(context);

            var updateIncident = new IncidentUpdateDTO
            {
                DiseaseId = 1
            };

            var result = await controller.PutIncident(999, updateIncident);

            Assert.IsType<NotFoundResult>(result);
        }

        // Test: Delete an existing incident
        [Fact]
        public async Task DeleteIncident_ReturnsNoContent_WhenIncidentIsDeleted()
        {
            var context = GetInMemoryDbContext("TestDb7");
            var controller = new IncidentController(context);

            var result = await controller.DeleteIncident(1);

            Assert.IsType<NoContentResult>(result);
        }

        // Test: Delete a non-existent incident
        [Fact]
        public async Task DeleteIncident_ReturnsNotFound_WhenIncidentDoesNotExist()
        {
            var context = GetInMemoryDbContext("TestDb8");
            var controller = new IncidentController(context);

            var result = await controller.DeleteIncident(999);

            Assert.IsType<NotFoundResult>(result);
        }

        // Test: Get all incidents (lightweight version)
        [Fact]
        public async Task GetIncidentsList_ReturnsIncidentListDTOList()
        {
            var context = GetInMemoryDbContext("TestDb9");
            var controller = new IncidentController(context);

            var result = await controller.GetIncidentsList();

            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var incidents = Assert.IsType<List<IncidentListDTO>>(actionResult.Value);
            Assert.Equal(2, incidents.Count);
        }

        // Test: Get incident details (full version)
        [Fact]
        public async Task GetIncidentDetails_ReturnsIncidentDetailsDTO_WhenIncidentExists()
        {
            var context = GetInMemoryDbContext("TestDb10");
            var controller = new IncidentController(context);

            var result = await controller.GetIncidentDetails(1);

            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var incident = Assert.IsType<IncidentDetailsDTO>(actionResult.Value);
            Assert.Equal("COVID-19", incident.DiseaseName);
        }
    }
}
