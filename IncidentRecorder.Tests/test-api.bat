@echo off

set LOCALHOST=http://localhost:5168

echo ====== Basic CRUD Operations Testing ======
echo.

echo Testing API: Create Disease 1
curl -X POST "%LOCALHOST%/api/Disease" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"name\":\"COVID-19\",\"description\":\"Coronavirus disease\"}"

echo.
echo.

echo Testing API: Create Disease 2
curl -X POST "%LOCALHOST%/api/Disease" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"name\":\"Flu\",\"description\":\"Seasonal influenza\"}"

echo.
echo.

echo Testing API: Get All Diseases
curl -X GET "%LOCALHOST%/api/Disease" ^
-H "Accept: application/json"

echo.
echo.

echo Testing API: Update Disease
curl -X PUT "%LOCALHOST%/api/Disease/1" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"description\":\"Caused by SARS-CoV-2\"}"

echo.
echo.

echo Testing API: Get Disease by ID
curl -X GET "%LOCALHOST%/api/Disease/1" ^
-H "Accept: application/json"

echo.
echo.

echo Testing API: Create Location 1
curl -X POST "%LOCALHOST%/api/Location" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"city\":\"Athens\",\"country\":\"Greece\"}"

echo.
echo.

echo Testing API: Create Location 2
curl -X POST "%LOCALHOST%/api/Location" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"city\":\"Thessaloniki\",\"country\":\"Greece\"}"

echo.
echo.

echo Testing API: Get All Locations
curl -X GET "%LOCALHOST%/api/Location" ^
-H "Accept: application/json"

echo.
echo.

echo Testing API: Update Location
curl -X PUT "%LOCALHOST%/api/Location/2" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"city\":\"Salonica\",\"country\":\"Hellas\"}"

echo.
echo.

echo Testing API: Get Location By ID
curl -X GET "%LOCALHOST%/api/Location/2" ^
-H "Accept: application/json"

echo.
echo.

echo Testing API: Create Patient 1
curl -X POST "%LOCALHOST%/api/Patient" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"NIN\":\"000000001\",\"firstName\":\"Kostas\",\"lastName\":\"Ganitis\",\"dateOfBirth\":\"1992-02-16T18:00:00Z\",\"gender\":\"Male\",\"contactInfo\":\"k.ganitis@unipi.gr\"}"

echo.
echo.

echo Testing API: Create Patient 2
curl -X POST "%LOCALHOST%/api/Patient" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"NIN\":\"000000002\",\"firstName\":\"Efthymios\",\"lastName\":\"Alepis\",\"dateOfBirth\":\"1980-07-01T18:00:00Z\",\"gender\":\"Male\",\"contactInfo\":\"e.alepis@unipi.gr\"}"

echo.
echo.

echo Testing API: Get All Patients
curl -X GET "%LOCALHOST%/api/Patient" ^
-H "Accept: application/json"

echo.
echo.

echo Testing API: Update Patient
curl -X PUT "%LOCALHOST%/api/Patient/1" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"firstName\":\"Konstantinos\",\"contactInfo\":\"kon.ganitis@unipi.gr\"}"

echo.

echo Testing API: Get Patient by ID
curl -X GET "%LOCALHOST%/api/Patient/1" ^
-H "Accept: application/json"

echo.
echo.

echo Testing API: Create Symptom 1
curl -X POST "%LOCALHOST%/api/Symptom" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"name\":\"Fever\",\"description\":\"High body temperature\"}"

echo.
echo.

echo Testing API: Create Symptom 2
curl -X POST "%LOCALHOST%/api/Symptom" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"name\":\"Cough\",\"description\":\"Persistent cough\"}"

echo.
echo.

echo Testing API: Get All Symptoms
curl -X GET "%LOCALHOST%/api/Symptom" ^
-H "Accept: application/json"

echo.
echo.

echo Testing API: Update Symptom
curl -X PUT "%LOCALHOST%/api/Symptom/2" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"name\":\"Persistent cough\",\"description\":\"Persistent cough for more than 3 weeks\"}"

echo.
echo.

echo Testing API: Get Symptom by ID
curl -X GET "%LOCALHOST%/api/Symptom/2" ^
-H "Accept: application/json"

echo.
echo.

echo Testing API: Create Incident 1
curl -X POST "%LOCALHOST%/api/Incident/create" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"diseaseId\":1,\"patientId\":1,\"locationId\":1,\"dateReported\":\"2024-09-07T10:00:00Z\",\"symptomIds\":[1]}"

echo.
echo.

echo Testing API: Create Incident 2
curl -X POST "%LOCALHOST%/api/Incident/create" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"diseaseId\":2,\"patientId\":2,\"locationId\":2,\"dateReported\":\"2024-09-08T10:00:00Z\",\"symptomIds\":[2]}"

echo.
echo.

echo Testing API: Create Incident only with DiseaseId
curl -X POST "%LOCALHOST%/api/Incident/create" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"diseaseId\":2}"

echo.
echo.

echo Testing API: Get All Incidents
curl -X GET "%LOCALHOST%/api/Incident/all" ^
-H "Content-Type: application/json"

echo.
echo.

echo Testing API: Get Incidents List
curl -X GET "%LOCALHOST%/api/Incident/list" ^
-H "Accept: application/json"

echo.
echo.

echo Testing API: Update Incident 3
curl -X PUT "%LOCALHOST%/api/Incident/3" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"dateReported\":\"2024-09-09T10:00:00Z\",\"symptomIds\":[1, 2]}"

echo.

echo Testing API: Get Incident by ID
curl -X GET "%LOCALHOST%/api/Incident/2" ^
-H "Accept: application/json"

echo.
echo.

echo Testing API: Get Incident Details by ID
curl -X GET "%LOCALHOST%/api/Incident/details/1" ^
-H "Accept: application/json"

echo.
echo.
echo.
echo ====== Create with Invalid Data Testing (400 Bad Request) ======
echo.
echo.

echo Testing API: Create Disease with Missing Field (Name)
curl -X POST "%LOCALHOST%/api/Disease" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"description\":\"Coronavirus disease\"}"

echo.
echo.

echo Testing API: Create Disease with Invalid Name Type (Integer instead of String)
curl -X POST "%LOCALHOST%/api/Disease" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"name\":123,\"description\":\"Coronavirus disease\"}"

echo.
echo.

echo Testing API: Create Location with Missing Field (City)
curl -X POST "%LOCALHOST%/api/Location" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"country\":\"Greece\"}"

echo.
echo.

echo Testing API: Create Location with Invalid Country Type (Integer instead of String)
curl -X POST "%LOCALHOST%/api/Location" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"city\":\"Athens\",\"country\":123}"

echo.
echo.

echo Testing API: Create Patient with Missing Field (FirstName)
curl -X POST "%LOCALHOST%/api/Patient" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"nin\":\"000000010\",\"lastName\":\"Doe\",\"dateOfBirth\":\"1990-05-20T00:00:00Z\",\"gender\":\"Male\",\"contactInfo\":\"john.doe@example.com\"}"

echo.
echo.

echo Testing API: Create Patient with Invalid NIN format 
curl -X POST "%LOCALHOST%/api/Patient" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"nin\":100000010,\"firstName\":\"John\",\"lastName\":\"Doe\",\"dateOfBirth\":\"1990-05-20T00:00:00Z\",\"gender\":\"Male\",\"contactInfo\":\"john.doe@example.com\"}"

echo.
echo.

echo Testing API: Create Patient with Invalid Date Format
curl -X POST "%LOCALHOST%/api/Patient" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"nin\":\"000000010\",\"firstName\":\"John\",\"lastName\":\"Doe\",\"dateOfBirth\":\"1990/05/20\",\"gender\":\"Male\",\"contactInfo\":\"john.doe@example.com\"}"

echo.
echo.

echo Testing API: Create Symptom with Missing Field (Name)
curl -X POST "%LOCALHOST%/api/Symptom" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"description\":\"High fever\"}"

echo.
echo.

echo Testing API: Create Symptom with Invalid Name Type (Integer instead of String)
curl -X POST "%LOCALHOST%/api/Symptom" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"name\":123,\"description\":\"High fever\"}"

echo.
echo.

echo Testing API: Create Incident with Missing Required Field (DiseaseID)
curl -X POST "%LOCALHOST%/api/Incident/create" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"patientId\":1,\"locationId\":1,\"dateReported\":\"2024-09-08T10:00:00Z\",\"symptomIds\":[1]}"

echo.
echo.

echo Testing API: Create Incident with Invalid DiseaseId Type (String instead of Integer)
curl -X POST "%LOCALHOST%/api/Incident/create" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"diseaseId\":\"one\",\"patientId\":1,\"locationId\":1,\"dateReported\":\"2024-09-08T10:00:00Z\",\"symptomIds\":[1]}"

echo.
echo.
echo.
echo ====== Update with Invalid Data Testing (400 Bad Request) ======
echo.
echo.

echo Testing API: Update Disease with Invalid Name Type (Integer instead of String)
curl -X PUT "%LOCALHOST%/api/Disease/1" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"name\":123}"

echo.
echo.

echo Testing API: Update Location with Invalid Country Type (Integer instead of String)
curl -X PUT "%LOCALHOST%/api/Location/1" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"country\":123}"

echo.
echo.

echo Testing API: Update Patient with Invalid NIN format
curl -X PUT "%LOCALHOST%/api/Patient/1" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"nin\":100000010}"

echo.
echo.

echo Testing API: Update Patient with Invalid Date Format
curl -X PUT "%LOCALHOST%/api/Patient/1" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"dateOfBirth\":\"1990/05/20\"}"

echo.
echo.

echo Testing API: Update Symptom with Invalid Name Type (Integer instead of String)
curl -X PUT "%LOCALHOST%/api/Symptom/1" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"name\":123}"

echo.
echo.

echo Testing API: Update Incident with Invalid DiseaseId Type (String instead of Integer)
curl -X PUT "%LOCALHOST%/api/Incident/1" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"diseaseId\":\"one\"}"

echo.
echo.
echo.
echo ====== Non-Existent ID Testing (404 Not Found) ======
echo.
echo.

echo Testing API: Get Non-existent Disease
curl -X GET "%LOCALHOST%/api/Disease/999" ^
-H "Accept: application/json"

echo.
echo.

echo Testing API: Update Non-existent Disease
curl -X PUT "%LOCALHOST%/api/Disease/999" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"name\":\"Test Disease\",\"description\":\"Testing 404\"}"

echo.
echo.

echo Testing API: Delete Non-existent Disease
curl -X DELETE "%LOCALHOST%/api/Disease/999" ^
-H "Accept: application/json"

echo.
echo.

echo Testing API: Get Non-existent Location
curl -X GET "%LOCALHOST%/api/Location/999" ^
-H "Accept: application/json"

echo.
echo.

echo Testing API: Update Non-existent Location
curl -X PUT "%LOCALHOST%/api/Location/999" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"city\":\"Test City\",\"country\":\"Test Country\"}"

echo.
echo.

echo Testing API: Delete Non-existent Location
curl -X DELETE "%LOCALHOST%/api/Location/999" ^
-H "Accept: application/json"

echo.
echo.

echo Testing API: Get Non-existent Patient
curl -X GET "%LOCALHOST%/api/Patient/999" ^
-H "Accept: application/json"

echo.
echo.

echo Testing API: Update Non-existent Patient
curl -X PUT "%LOCALHOST%/api/Patient/999" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"firstName\":\"Test\",\"lastName\":\"User\"}"

echo.
echo.

echo Testing API: Delete Non-existent Patient
curl -X DELETE "%LOCALHOST%/api/Patient/999" ^
-H "Accept: application/json"

echo.
echo.

echo Testing API: Get Non-existent Symptom
curl -X GET "%LOCALHOST%/api/Symptom/999" ^
-H "Accept: application/json"

echo.
echo.

echo Testing API: Update Non-existent Symptom
curl -X PUT "%LOCALHOST%/api/Symptom/999" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"name\":\"Test Symptom\",\"description\":\"Testing 404\"}"

echo.
echo.

echo Testing API: Delete Non-existent Symptom
curl -X DELETE "%LOCALHOST%/api/Symptom/999" ^
-H "Accept: application/json"

echo.
echo.

echo Testing API: Get Non-existent Incident
curl -X GET "%LOCALHOST%/api/Incident/999" ^
-H "Accept: application/json"

echo.
echo.

echo Testing API: Update Non-existent Incident
curl -X PUT "%LOCALHOST%/api/Incident/999" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"dateReported\":\"2024-09-10T10:00:00Z\",\"symptomIds\":[1]}"

echo.
echo.

echo Testing API: Delete Non-existent Incident
curl -X DELETE "%LOCALHOST%/api/Incident/999" ^
-H "Accept: application/json"

echo.
echo.
echo.
echo ====== Invalid ID Testing (400 Bad Request) ======
echo.
echo.

echo Testing API: Fetch Disease with invalid ID
curl -X GET "%LOCALHOST%/api/Disease/invalid-id" ^
-H "Accept: application/json"

echo.
echo.

echo Testing API: Fetch Patient with invalid ID
curl -X GET "%LOCALHOST%/api/Patient/invalid-id" ^
-H "Accept: application/json"

echo.
echo.

echo Testing API: Fetch Location with invalid ID
curl -X GET "%LOCALHOST%/api/Location/invalid-id" ^
-H "Accept: application/json"

echo.
echo.

echo Testing API: Fetch Symptom with invalid ID
curl -X GET "%LOCALHOST%/api/Symptom/invalid-id" ^
-H "Accept: application/json"

echo.
echo.

echo Testing API: Fetch Incident with invalid ID
curl -X GET "%LOCALHOST%/api/Incident/invalid-id" ^
-H "Accept: application/json"

echo.
echo.

echo Testing API: Update Disease with invalid ID
curl -X PUT "%LOCALHOST%/api/Disease/invalid-id" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{}"

echo.
echo.

echo Testing API: Update Patient with invalid ID
curl -X PUT "%LOCALHOST%/api/Patient/invalid-id" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{}"

echo.
echo.

echo Testing API: Update Location with invalid ID
curl -X PUT "%LOCALHOST%/api/Location/invalid-id" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"city\":\"Invalid City\",\"country\":\"Invalid Country\"}"

echo.
echo.

echo Testing API: Update Symptom with invalid ID
curl -X PUT "%LOCALHOST%/api/Symptom/invalid-id" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{}"

echo.
echo.

echo Testing API: Update Incident with invalid ID
curl -X PUT "%LOCALHOST%/api/Incident/invalid-id" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{}"

echo.
echo.
echo.
echo ====== Empty Payload Testing ======
echo.
echo.

echo Testing API: Create Disease with empty payload
curl -X POST "%LOCALHOST%/api/Disease" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{}"

echo.
echo.

echo Testing API: Create Patient with empty payload
curl -X POST "%LOCALHOST%/api/Patient" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{}"

echo.
echo.

echo Testing API: Create Location with empty payload
curl -X POST "%LOCALHOST%/api/Location" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{}"

echo.
echo.

echo Testing API: Create Symptom with empty payload
curl -X POST "%LOCALHOST%/api/Symptom" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{}"

echo.
echo.

echo Testing API: Create Incident with empty payload
curl -X POST "%LOCALHOST%/api/Incident/create" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{}"

echo.
echo.

echo Testing API: Update Incident with empty payload
curl -X PUT "%LOCALHOST%/api/Incident/1" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{}"
curl -X GET "%LOCALHOST%/api/Incident/1" ^
-H "Accept: application/json"

echo.
echo.
echo.
echo ====== Non-Existing Foreign Key Testing ======
echo.
echo.

echo Testing API: Create Incident with Non-Existing PatientId
curl -X POST "%LOCALHOST%/api/Incident/create" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"diseaseId\":1,\"patientId\":999,\"locationId\":1,\"dateReported\":\"2024-09-07T10:00:00Z\",\"symptomIds\":[1]}"

echo.
echo.

echo Testing API: Create Incident with Non-Existing LocationId
curl -X POST "%LOCALHOST%/api/Incident/create" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"diseaseId\":1,\"patientId\":1,\"locationId\":999,\"dateReported\":\"2024-09-07T10:00:00Z\",\"symptomIds\":[1]}"

echo.
echo.

echo Testing API: Create Incident with Non-Existing SymptomIds
curl -X POST "%LOCALHOST%/api/Incident/create" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"diseaseId\":1,\"patientId\":1,\"locationId\":1,\"dateReported\":\"2024-09-07T10:00:00Z\",\"symptomIds\":[999]}"

echo.
echo.

echo Testing API: Update Incident with Non-Existing PatientId
curl -X PUT "%LOCALHOST%/api/Incident/1" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"patientId\":999}"

echo.
echo.

echo Testing API: Update Incident with Non-Existing LocationId
curl -X PUT "%LOCALHOST%/api/Incident/1" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"locationId\":999}"

echo.
echo.

echo Testing API: Update Incident with Non-Existing SymptomIds
curl -X PUT "%LOCALHOST%/api/Incident/1" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"symptomIds\":[999]}"

echo.
echo.
echo.
echo ====== Unique Constraint Testing ======
echo.
echo.

echo Testing API: Create Disease with Duplicate Name
curl -X POST "%LOCALHOST%/api/Disease" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"name\":\"COVID-19\",\"description\":\"Disease with duplicate name\"}"

echo.
echo.

echo Testing API: Update Disease with Duplicate Name
curl -X PUT "%LOCALHOST%/api/Disease/2" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"name\":\"COVID-19\"}"

echo.
echo.

echo Testing API: Create Duplicate Location
curl -X POST "%LOCALHOST%/api/Location" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"city\":\"Athens\",\"country\":\"Greece\"}"

echo.
echo.

echo Testing API: Update Location with Duplicate Data
curl -X PUT "%LOCALHOST%/api/Location/2" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"city\":\"Athens\",\"country\":\"Greece\"}"

echo.
echo.

echo Testing API: Create Patient with Duplicate NIN
curl -X POST "%LOCALHOST%/api/Patient" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"NIN\":\"000000001\",\"firstName\":\"George\",\"lastName\":\"Pap\",\"dateOfBirth\":\"1998-02-16T18:00:00Z\",\"gender\":\"Male\",\"contactInfo\":\"geo.pap@unipi.gr\"}"

echo.
echo.

echo Testing API: Update Patient with Duplicate NIN
curl -X PUT "%LOCALHOST%/api/Patient/2" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"NIN\":\"000000001\"}"

echo.
echo.

echo Testing API: Create Symptom with Duplicate Name
curl -X POST "%LOCALHOST%/api/Symptom" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"name\":\"Fever\",\"description\":\"Symptom with duplicate name\"}"

echo.
echo.

echo Testing API: Update Symptom with Duplicate Name
curl -X PUT "%LOCALHOST%/api/Symptom/2" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"name\":\"Fever\"}"

echo.
echo.

pause