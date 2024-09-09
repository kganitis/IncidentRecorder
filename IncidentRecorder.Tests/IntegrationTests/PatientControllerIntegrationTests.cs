using System.Net;
using IncidentRecorder.DTOs.Patient;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using IncidentRecorder.Tests.Integration;

namespace IncidentRecorder.Tests.Integration
{
    public class PatientControllerIntegrationTests : BaseIntegrationTest
    {
        public PatientControllerIntegrationTests(WebApplicationFactory<Program> factory) : base(factory) { }

        // Test: Get all patients with seeded data
        [Fact]
        public async Task GetPatients_ReturnsOkResult_WithSeededData()
        {
            var response = await _client.GetAsync("/api/patient");

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var patients = JsonConvert.DeserializeObject<List<PatientDTO>>(content);

            Assert.NotNull(patients);
            Assert.Contains(patients, p => p.FirstName == "John" && p.LastName == "Doe" && p.Gender == "Male");
        }

        // Test: Get a single patient by dynamically capturing ID after creation
        [Fact]
        public async Task GetPatientById_ReturnsOkResult_WhenPatientExists()
        {
            var newPatient = new PatientCreateDTO
            {
                FirstName = "Jane",
                LastName = "Doe",
                DateOfBirth = new System.DateTime(1990, 5, 15),
                Gender = "Female",
                ContactInfo = "jane.doe@example.com"
            };

            var content = new StringContent(JsonConvert.SerializeObject(newPatient), System.Text.Encoding.UTF8, "application/json");
            var postResponse = await _client.PostAsync("/api/patient", content);
            postResponse.EnsureSuccessStatusCode();
            var createdContent = await postResponse.Content.ReadAsStringAsync();
            var createdPatient = JsonConvert.DeserializeObject<PatientDTO>(createdContent);
            var createdId = createdPatient.Id;

            var getResponse = await _client.GetAsync($"/api/patient/{createdId}");

            getResponse.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var getContent = await getResponse.Content.ReadAsStringAsync();
            var patient = JsonConvert.DeserializeObject<PatientDTO>(getContent);

            Assert.NotNull(patient);
            Assert.Equal(createdId, patient.Id);
            Assert.Equal("Jane", patient.FirstName);
            Assert.Equal("Doe", patient.LastName);
            Assert.Equal("Female", patient.Gender);
        }

        // Test: Post a new patient and verify creation
        [Fact]
        public async Task PostPatient_CreatesNewPatient()
        {
            var newPatient = new PatientCreateDTO
            {
                FirstName = "Mark",
                LastName = "Smith",
                DateOfBirth = new System.DateTime(1985, 7, 20),
                Gender = "Male",
                ContactInfo = "mark.smith@example.com"
            };

            var content = new StringContent(JsonConvert.SerializeObject(newPatient), System.Text.Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/patient", content);

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var responseContent = await response.Content.ReadAsStringAsync();
            var createdPatient = JsonConvert.DeserializeObject<PatientDTO>(responseContent);

            Assert.NotNull(createdPatient);
            Assert.Equal("Mark", createdPatient.FirstName);
            Assert.Equal("Smith", createdPatient.LastName);
            Assert.Equal("Male", createdPatient.Gender);
        }

        // Test: Update an existing patient
        [Fact]
        public async Task PutPatient_UpdatesExistingPatient()
        {
            var newPatient = new PatientCreateDTO
            {
                FirstName = "Alice",
                LastName = "Brown",
                DateOfBirth = new System.DateTime(1995, 3, 22),
                Gender = "Female",
                ContactInfo = "alice.brown@example.com"
            };

            var postContent = new StringContent(JsonConvert.SerializeObject(newPatient), System.Text.Encoding.UTF8, "application/json");
            var postResponse = await _client.PostAsync("/api/patient", postContent);
            postResponse.EnsureSuccessStatusCode();

            var postCreatedContent = await postResponse.Content.ReadAsStringAsync();
            var createdPatient = JsonConvert.DeserializeObject<PatientDTO>(postCreatedContent);
            var createdId = createdPatient.Id;

            var updatedPatient = new PatientUpdateDTO
            {
                FirstName = "Alicia",
                LastName = "Brown",
                Gender = "Female",
                ContactInfo = "alicia.brown@example.com"
            };

            var updateContent = new StringContent(JsonConvert.SerializeObject(updatedPatient), System.Text.Encoding.UTF8, "application/json");

            var putResponse = await _client.PutAsync($"/api/patient/{createdId}", updateContent);

            Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);

            var getResponse = await _client.GetAsync($"/api/patient/{createdId}");
            var getContent = await getResponse.Content.ReadAsStringAsync();
            var updatedPatientResult = JsonConvert.DeserializeObject<PatientDTO>(getContent);

            Assert.NotNull(updatedPatientResult);
            Assert.Equal("Alicia", updatedPatientResult.FirstName);
            Assert.Equal("alicia.brown@example.com", updatedPatientResult.ContactInfo);
        }

        // Test: Delete an existing patient
        [Fact]
        public async Task DeletePatient_DeletesExistingPatient()
        {
            var newPatient = new PatientCreateDTO
            {
                FirstName = "Chris",
                LastName = "Evans",
                DateOfBirth = new System.DateTime(1980, 6, 13),
                Gender = "Male",
                ContactInfo = "chris.evans@example.com"
            };

            var postContent = new StringContent(JsonConvert.SerializeObject(newPatient), System.Text.Encoding.UTF8, "application/json");
            var postResponse = await _client.PostAsync("/api/patient", postContent);
            postResponse.EnsureSuccessStatusCode();

            var postCreatedContent = await postResponse.Content.ReadAsStringAsync();
            var createdPatient = JsonConvert.DeserializeObject<PatientDTO>(postCreatedContent);
            var createdId = createdPatient.Id;

            var deleteResponse = await _client.DeleteAsync($"/api/patient/{createdId}");

            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            var getResponse = await _client.GetAsync($"/api/patient/{createdId}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        // Test: Create Patient with Missing Required Fields (400 Bad Request)
        [Fact]
        public async Task PostPatient_ReturnsBadRequest_WhenRequiredFieldsAreMissing()
        {
            var invalidPatient = new PatientCreateDTO
            {
                DateOfBirth = new System.DateTime(1990, 5, 15),
                Gender = "Female",
                ContactInfo = "jane.doe@example.com"
            };

            var content = new StringContent(JsonConvert.SerializeObject(invalidPatient), System.Text.Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/patient", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        // Test: Get Non-existent Patient (404 Not Found)
        [Fact]
        public async Task GetPatientById_ReturnsNotFound_WhenPatientDoesNotExist()
        {
            var response = await _client.GetAsync("/api/patient/999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        // Test: Update Non-existent Patient (404 Not Found)
        [Fact]
        public async Task PutPatient_ReturnsNotFound_WhenPatientDoesNotExist()
        {
            var updatePatient = new PatientUpdateDTO
            {
                FirstName = "Non-existent",
                LastName = "Patient",
                Gender = "Female",
                ContactInfo = "non.existent@example.com"
            };

            var content = new StringContent(JsonConvert.SerializeObject(updatePatient), System.Text.Encoding.UTF8, "application/json");
            var response = await _client.PutAsync("/api/patient/999", content);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        // Test: Delete Non-existent Patient (404 Not Found)
        [Fact]
        public async Task DeletePatient_ReturnsNotFound_WhenPatientDoesNotExist()
        {
            var response = await _client.DeleteAsync("/api/patient/999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
