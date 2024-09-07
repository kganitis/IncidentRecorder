using System.Net;
using IncidentRecorder.DTOs.Patient;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using IncidentRecorder.Tests.Integration;

namespace IncidentRecorder.Tests.IntegrationTests
{
    public class PatientControllerIntegrationTests : BaseIntegrationTest
    {
        public PatientControllerIntegrationTests(WebApplicationFactory<Program> factory) : base(factory) { }

        // Test: Get all patients with seeded data
        [Fact]
        public async Task GetPatients_ReturnsOkResult_WithSeededData()
        {
            // Act
            var response = await _client.GetAsync("/api/patient");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var patients = JsonConvert.DeserializeObject<List<PatientDTO>>(content);

            // Verify the seeded patient is returned (if patient data was seeded)
            Assert.NotNull(patients);
            Assert.Contains(patients, p => p.FirstName == "John" && p.LastName == "Doe" && p.Gender == "Male");
        }

        // Test: Get a single patient by dynamically capturing ID after creation
        [Fact]
        public async Task GetPatientById_ReturnsOkResult_WhenPatientExists()
        {
            // Arrange: Create a new patient to get its ID
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

            // Act: Fetch the patient by the captured ID
            var getResponse = await _client.GetAsync($"/api/patient/{createdId}");

            // Assert
            getResponse.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var getContent = await getResponse.Content.ReadAsStringAsync();
            var patient = JsonConvert.DeserializeObject<PatientDTO>(getContent);

            // Verify the patient details
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
            // Arrange
            var newPatient = new PatientCreateDTO
            {
                FirstName = "Mark",
                LastName = "Smith",
                DateOfBirth = new System.DateTime(1985, 7, 20),
                Gender = "Male",
                ContactInfo = "mark.smith@example.com"
            };

            var content = new StringContent(JsonConvert.SerializeObject(newPatient), System.Text.Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/patient", content);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            // Deserialize the created patient to verify the content
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
            // Arrange: Create a new patient to get its ID
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

            // Arrange: Prepare update data
            var updatedPatient = new PatientUpdateDTO
            {
                FirstName = "Alicia",
                LastName = "Brown",
                Gender = "Female",
                ContactInfo = "alicia.brown@example.com"
            };

            var updateContent = new StringContent(JsonConvert.SerializeObject(updatedPatient), System.Text.Encoding.UTF8, "application/json");

            // Act: Update the patient
            var putResponse = await _client.PutAsync($"/api/patient/{createdId}", updateContent);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);

            // Verify that the patient was updated by fetching it again
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
            // Arrange: Create a new patient to get its ID
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

            // Act: Delete the patient
            var deleteResponse = await _client.DeleteAsync($"/api/patient/{createdId}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            // Verify that the patient no longer exists
            var getResponse = await _client.GetAsync($"/api/patient/{createdId}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }
    }
}
