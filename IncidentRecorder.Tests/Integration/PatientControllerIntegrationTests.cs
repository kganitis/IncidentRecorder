using System.Net;
using IncidentRecorder.DTOs.Patient;
using Microsoft.AspNetCore.Mvc.Testing;

namespace IncidentRecorder.Tests.Integration
{
    public class PatientControllerIntegrationTests(WebApplicationFactory<Program> factory) : BaseIntegrationTest(factory)
    {
        private const string PatientApiUrl = "/api/patient";

        private static void AssertPatient(PatientDTO patient, int id, string nin, string firstName, string lastName, DateTime? dateOfBirth, string gender, string contactInfo)
        {
            Assert.NotNull(patient);
            Assert.Equal(id, patient.Id);
            Assert.Equal(nin, patient.NIN);
            Assert.Equal(firstName, patient.FirstName);
            Assert.Equal(lastName, patient.LastName);
            Assert.Equal(dateOfBirth, patient.DateOfBirth);
            Assert.Equal(gender, patient.Gender);
            Assert.Equal(contactInfo, patient.ContactInfo);
        }

        [Fact]
        public async Task GetPatients_ReturnsOkResult_WithSeededData()
        {
            // Act
            var response = await _client.GetAsync(PatientApiUrl);

            // Assert
            response.EnsureSuccessStatusCode();
            var patients = await DeserializeResponse<List<PatientDTO>>(response);
            Assert.NotNull(patients);

            for (int i = 0; i < SeededPatients.Count; i++)
            {
                var actual = patients[i];
                var expected = SeededPatients[i];
                AssertPatient(actual, expected.Id, expected.NIN, expected.FirstName, expected.LastName, expected.DateOfBirth, expected.Gender, expected.ContactInfo);
            }
        }

        [Fact]
        public async Task GetPatientById_ReturnsOkResult_WhenPatientExists()
        {
            // Act
            var response = await _client.GetAsync($"{PatientApiUrl}/{SeededPatients[0].Id}");

            // Assert
            var actual = await DeserializeResponse<PatientDTO>(response);
            var expected = SeededPatients[0];
            AssertPatient(actual, expected.Id, expected.NIN, expected.FirstName, expected.LastName, expected.DateOfBirth, expected.Gender, expected.ContactInfo);
        }

        [Fact]
        public async Task PostPatient_CreatesNewPatient()
        {
            // Arrange: Prepare new patient data
            var newPatient = new PatientCreateDTO
            {
                NIN = "000000007",
                FirstName = "George",
                LastName = "Washington",
                DateOfBirth = new DateTime(1732, 2, 22),
                Gender = "Male",
                ContactInfo = "potus@usa.gov"
            };

            // Act
            var response = await _client.PostAsync(PatientApiUrl, CreateContent(newPatient));

            // Assert
            response.EnsureSuccessStatusCode();
            var createdPatient = await DeserializeResponse<PatientDTO>(response);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            AssertPatient(createdPatient, createdPatient.Id, newPatient.NIN, newPatient.FirstName, newPatient.LastName, newPatient.DateOfBirth, newPatient.Gender, newPatient.ContactInfo);
        }

        [Fact]
        public async Task PutPatient_UpdatesExistingPatient()
        {
            // Arrange: Create a new patient to update
            var newPatient = new PatientCreateDTO
            {
                NIN = "000000008",
                FirstName = "Test FirstName",
                LastName = "Test LastName",
                DateOfBirth = new DateTime(1990, 1, 1),
                Gender = "Male",
                ContactInfo = "test@mail.com"
            };

            // Get the ID of the newly created patient
            var postResponse = await _client.PostAsync(PatientApiUrl, CreateContent(newPatient));
            postResponse.EnsureSuccessStatusCode();
            var createdPatient = await DeserializeResponse<PatientDTO>(postResponse);
            var createdId = createdPatient.Id;

            // Arrange: Prepare updated patient data
            var updatedPatient = new PatientUpdateDTO
            {
                FirstName = "Updated FirstName",
                LastName = "Updated LastName",
                DateOfBirth = new DateTime(1992, 1, 1),
                Gender = "Female",
                ContactInfo = "updated@mail.com"
            };

            // Act
            var putResponse = await _client.PutAsync($"{PatientApiUrl}/{createdId}", CreateContent(updatedPatient));

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);

            // Verify that the patient was updated by fetching it again
            var getResponse = await _client.GetAsync($"{PatientApiUrl}/{createdId}");
            var updatedPatientResult = await DeserializeResponse<PatientDTO>(getResponse);

            AssertPatient(updatedPatientResult, createdId, newPatient.NIN, updatedPatient.FirstName, updatedPatient.LastName, updatedPatient.DateOfBirth, updatedPatient.Gender, updatedPatient.ContactInfo);

            // Delete the updated patient to clean up
            var deleteResponse = await _client.DeleteAsync($"{PatientApiUrl}/{createdId}");
            deleteResponse.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task DeletePatient_DeletesExistingPatient()
        {
            // Arrange: Create a new patient to delete
            var newPatient = new PatientCreateDTO
            {
                NIN = "000000009",
                FirstName = "Test FirstName",
                LastName = "Test LastName",
                DateOfBirth = new DateTime(1990, 1, 1),
                Gender = "Male",
                ContactInfo = "test@mail.com"
            };

            // Get the ID of the newly created patient
            var postResponse = await _client.PostAsync(PatientApiUrl, CreateContent(newPatient));
            postResponse.EnsureSuccessStatusCode();
            var createdPatient = await DeserializeResponse<PatientDTO>(postResponse);
            var createdId = createdPatient.Id;

            // Act
            var deleteResponse = await _client.DeleteAsync($"{PatientApiUrl}/{createdId}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            // Verify that the patient no longer exists
            var getResponse = await _client.GetAsync($"{PatientApiUrl}/{createdId}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task PostPatient_ReturnsBadRequest_WhenRequiredFieldsAreMissing()
        {
            // Arrange: Create new patient with missing fields
            var invalidPatient = "{ \"nin\": 000000010 }";  // Missing required fields

            var content = new StringContent(invalidPatient, System.Text.Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync(PatientApiUrl, content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PostPatient_ReturnsBadRequest_WhenInvalidDataTypeIsProvided()
        {
            // Arrange: Provide an integer where a string is expected
            var invalidPatient1 = new
            {
                NIN = 000000010,  // Invalid data type
                FirstName = "Test FirstName",
                LastName = "Test LastName",
                DateOfBirth = new DateTime(1990, 1, 1),
                Gender = "Male",
                ContactInfo = "test1@mail.com"
            };

            // Arrange: Provide an invalid date time format
            var invalidPatient2 = new
            {
                NIN = "000000010",
                FirstName = "Test FirstName",
                LastName = "Test LastName",
                DateOfBirth = "01/01/2000",  // Invalid date time format
                Gender = "Female",
                ContactInfo = "test2@mail.com"
            };

            // Act
            var response1 = await _client.PostAsync(PatientApiUrl, CreateContent(invalidPatient1));
            var response2 = await _client.PostAsync(PatientApiUrl, CreateContent(invalidPatient2));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response1.StatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, response2.StatusCode);
        }

        [Fact]
        public async Task PutPatient_ReturnsBadRequest_WhenInvalidDataTypeIsProvided()
        {
            // Arrange: Provide an invalid payload with an integer where a string is expected
            var invalidPatient1 =
                "{" +
                "\"nin\": 000000010," +  // Invalid data type
                "\"firstName\": \"Kostas\"," +
                "\"lastName\": \"Ganitis\"," +
                "\"dateOfBirth\": \"1992-02-16T18:00:00Z\"," +
                "\"gender\": \"Male\"," +
                "\"contactInfo\": \"k.ganitis@unipi.gr\"" +
                "}";

            // Arrange: Provide an invalid payload with an invalid date time format
            var invalidPatient2 =
                "{" +
                "\"nin\": 000000010," +
                "\"firstName\": \"Kostas\"," +
                "\"lastName\": \"Ganitis\"," +
                "\"dateOfBirth\": \"16/02/1992\"," +  // Invalid date time format
                "\"gender\": \"Male\"," +
                "\"contactInfo\": \"k.ganitis@unipi.gr\"" +
                "}";

            var content1 = new StringContent(invalidPatient1, System.Text.Encoding.UTF8, "application/json");
            var content2 = new StringContent(invalidPatient2, System.Text.Encoding.UTF8, "application/json");

            // Act
            var response1 = await _client.PutAsync($"{PatientApiUrl}/1", content1);
            var response2 = await _client.PutAsync($"{PatientApiUrl}/1", content2);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response1.StatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, response2.StatusCode);
        }

        [Fact]
        public async Task GetPatientById_ReturnsNotFound_WhenPatientDoesNotExist()
        {
            // Act: Try to get a non-existing patient
            var response = await _client.GetAsync($"{PatientApiUrl}/999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task PutPatient_ReturnsNotFound_WhenPatientDoesNotExist()
        {
            // Arrange
            var updatePatient = new PatientUpdateDTO { };

            // Act: Try to update a non-existing patient
            var response = await _client.PutAsync($"{PatientApiUrl}/999", CreateContent(updatePatient));

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeletePatient_ReturnsNotFound_WhenPatientDoesNotExist()
        {
            // Act: Try to delete a non-existing patient
            var response = await _client.DeleteAsync($"{PatientApiUrl}/999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetPatient_ReturnsBadRequest_WithInvalidID()
        {
            // Act: Send the request with an invalid ID in the URL (e.g., a string instead of an integer)
            var response = await _client.GetAsync($"{PatientApiUrl}/invalid-id");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PutPatient_ReturnsBadRequest_WithInvalidID()
        {
            // Arrange
            var updatePatient = new PatientUpdateDTO { };

            // Act: Send the request with an invalid ID in the URL (e.g., a string instead of an integer)
            var response = await _client.PutAsync($"{PatientApiUrl}/invalid-id", CreateContent(updatePatient));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task DeletePatient_ReturnsBadRequest_WithInvalidID()
        {
            // Act: Send the request with an invalid ID in the URL (e.g., a string instead of an integer)
            var response = await _client.DeleteAsync($"{PatientApiUrl}/invalid-id");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PostPatient_ReturnsBadRequest_WhenPayloadIsEmpty()
        {
            // Arrange: Create an empty payload
            var emptyPayload = "{}";

            var content = new StringContent(emptyPayload, System.Text.Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync(PatientApiUrl, content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PutPatient_ReturnsNoContent_WhenPayloadIsEmpty()
        {
            // Arrange: Fetch an existing patient
            var getResponseBeforeUpdate = await _client.GetAsync($"{PatientApiUrl}/1");
            getResponseBeforeUpdate.EnsureSuccessStatusCode();

            var patientBeforeUpdate = await DeserializeResponse<PatientDTO>(getResponseBeforeUpdate);

            // Act: Send an empty payload to update the patient
            var emptyPayload = "{}"; // Empty JSON object
            var updateContent = new StringContent(emptyPayload, System.Text.Encoding.UTF8, "application/json");

            var putResponse = await _client.PutAsync($"{PatientApiUrl}/1", updateContent);

            // Assert: The response should be 204 No Content
            Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);

            // Act: Fetch the patient again after the update
            var getResponseAfterUpdate = await _client.GetAsync($"{PatientApiUrl}/1");
            var patientAfterUpdate = await DeserializeResponse<PatientDTO>(getResponseAfterUpdate);

            // Assert: The patient should remain unchanged
            Assert.Equal(patientBeforeUpdate.NIN, patientAfterUpdate.NIN);
            Assert.Equal(patientBeforeUpdate.FirstName, patientAfterUpdate.FirstName);
            Assert.Equal(patientBeforeUpdate.LastName, patientAfterUpdate.LastName);
            Assert.Equal(patientBeforeUpdate.DateOfBirth, patientAfterUpdate.DateOfBirth);
            Assert.Equal(patientBeforeUpdate.Gender, patientAfterUpdate.Gender);
            Assert.Equal(patientBeforeUpdate.ContactInfo, patientAfterUpdate.ContactInfo);
        }

        [Fact]
        public async Task PostPatient_ReturnsConflict_WhenPatientNameIsNotUnique()
        {
            // Arrange: Create a patient with an existing NIN
            var duplicatePatient = new PatientCreateDTO
            {
                NIN = "000000001",
                FirstName = "George",
                LastName = "Washington",
                DateOfBirth = new DateTime(1732, 2, 22),
                Gender = "Male",
                ContactInfo = "potus@usa.gov"
            };

            // Act: Try to create a duplicate patient
            var response = await _client.PostAsync(PatientApiUrl, CreateContent(duplicatePatient));

            // Assert: Conflict due to uniqueness violation
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task PutPatient_ReturnsConflict_WhenPatientNameIsNotUnique()
        {
            // Arrange: Prepare a payload with an existing NIN
            var duplicatePatient = new PatientUpdateDTO { NIN = "000000001" };

            // Act: Try to update a patient with the duplicate data
            var response = await _client.PutAsync($"{PatientApiUrl}/2", CreateContent(duplicatePatient));

            // Assert: Conflict due to uniqueness violation
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }
    }
}
