@echo off

echo Testing API: Create Disease 1
curl -X POST "http://localhost:5168/api/Disease" ^
-H "accept: text/plain" ^
-H "Content-Type: application/json" ^
-d "{\"name\":\"COVID-19\",\"description\":\"Coronavirus disease\"}"

echo.

echo Testing API: Create Disease 2
curl -X POST "http://localhost:5168/api/Disease" ^
-H "accept: text/plain" ^
-H "Content-Type: application/json" ^
-d "{\"name\":\"Flu\",\"description\":\"Seasonal influenza\"}"

echo.

echo Testing API: Get All Diseases
curl -X GET "http://localhost:5168/api/Disease" ^
-H "accept: text/plain"

echo.

echo Testing API: Update Disease
curl -X PUT "http://localhost:5168/api/Disease/1" ^
-H "accept: text/plain" ^
-H "Content-Type: application/json" ^
-d "{\"description\":\"Caused by SARS-CoV-2\"}"

echo.

echo Testing API: Get Disease by ID
curl -X GET "http://localhost:5168/api/Disease/1" ^
-H "accept: text/plain"

echo.

echo Testing API: Create Location 1
curl -X POST "http://localhost:5168/api/Location" ^
-H "accept: text/plain" ^
-H "Content-Type: application/json" ^
-d "{\"city\":\"Athens\",\"country\":\"Greece\"}"

echo.

echo Testing API: Create Location 2
curl -X POST "http://localhost:5168/api/Location" ^
-H "accept: text/plain" ^
-H "Content-Type: application/json" ^
-d "{\"city\":\"Thessaloniki\",\"country\":\"Greece\"}"

echo.

echo Testing API: Get All Locations
curl -X GET "http://localhost:5168/api/Location" ^
-H "accept: text/plain"

echo.

echo Testing API: Update Location
curl -X PUT "http://localhost:5168/api/Location/2" ^
-H "accept: text/plain" ^
-H "Content-Type: application/json" ^
-d "{\"city\":\"Salonica\",\"country\":\"Hellas\"}"

echo.

echo Testing API: Get Location By ID
curl -X GET "http://localhost:5168/api/Location/2" ^
-H "accept: text/plain"

echo.

echo Testing API: Create Patient 1
curl -X POST "http://localhost:5168/api/Patient" ^
-H "Content-Type: application/json" ^
-d "{\"firstName\":\"Kostas\",\"lastName\":\"Ganitis\",\"dateOfBirth\":\"1992-02-16T18:00:00Z\",\"gender\":\"Male\",\"contactInfo\":\"k.ganitis@unipi.gr\"}"

echo.

echo Testing API: Create Patient 2
curl -X POST "http://localhost:5168/api/Patient" ^
-H "Content-Type: application/json" ^
-d "{\"firstName\":\"Efthymios\",\"lastName\":\"Alepis\",\"dateOfBirth\":\"1980-07-01T18:00:00Z\",\"gender\":\"Male\",\"contactInfo\":\"e.alepis@unipi.gr\"}"

echo.

echo Testing API: Get All Patients
curl -X GET "http://localhost:5168/api/Patient" ^
-H "accept: text/plain"

echo.

echo Testing API: Update Patient
curl -X PUT "http://localhost:5168/api/Patient/1" ^
-H "accept: text/plain" ^
-H "Content-Type: application/json" ^
-d "{\"firstName\":\"Konstantinos\",\"contactInfo\":\"kon.ganitis@unipi.gr\"}"

echo.

echo Testing API: Get Patient by ID
curl -X GET "http://localhost:5168/api/Patient/1" ^
-H "accept: text/plain"

echo.

echo Testing API: Create Symptom 1
curl -X POST "http://localhost:5168/api/Symptom" ^
-H "accept: text/plain" ^
-H "Content-Type: application/json" ^
-d "{\"name\":\"Fever\",\"description\":\"High body temperature\"}"

echo.

echo Testing API: Create Symptom 2
curl -X POST "http://localhost:5168/api/Symptom" ^
-H "accept: text/plain" ^
-H "Content-Type: application/json" ^
-d "{\"name\":\"Cough\",\"description\":\"Persistent cough\"}"

echo.

echo Testing API: Get All Symptoms
curl -X GET "http://localhost:5168/api/Symptom" ^
-H "accept: text/plain"

echo.

echo Testing API: Update Symptom
curl -X PUT "http://localhost:5168/api/Symptom/2" ^
-H "accept: text/plain" ^
-H "Content-Type: application/json" ^
-d "{\"name\":\"Persistent cough\",\"description\":\"Persistent cough for more than 3 weeks\"}"

echo.

echo Testing API: Get Symptom by ID
curl -X GET "http://localhost:5168/api/Symptom/2" ^
-H "accept: text/plain"

echo.

echo Testing API: Create Incident 1
curl -X POST "http://localhost:5168/api/Incident/create" ^
-H "accept: text/plain" ^
-H "Content-Type: application/json" ^
-d "{\"diseaseId\":1,\"patientId\":1,\"locationId\":1,\"dateReported\":\"2024-09-07T10:00:00Z\",\"symptomIds\":[1]}"

echo.

echo Testing API: Create Incident 2
curl -X POST "http://localhost:5168/api/Incident/create" ^
-H "accept: text/plain" ^
-H "Content-Type: application/json" ^
-d "{\"diseaseId\":2,\"patientId\":2,\"locationId\":2,\"dateReported\":\"2024-09-08T10:00:00Z\",\"symptomIds\":[2]}"

echo.

echo Testing API: Create Incident only with DiseaseId
curl -X POST "http://localhost:5168/api/Incident/create" ^
-H "accept: text/plain" ^
-H "Content-Type: application/json" ^
-d "{\"diseaseId\":2}"

echo.

echo Testing API: Get All Incidents
curl -X GET "http://localhost:5168/api/Incident/all" ^
-H "Content-Type: application/json"

echo.

echo Testing API: Get Incidents List
curl -X GET "http://localhost:5168/api/Incident/list" ^
-H "accept: text/plain"

echo.

echo Testing API: Update Incident 3
curl -X PUT "http://localhost:5168/api/Incident/3" ^
-H "accept: text/plain" ^
-H "Content-Type: application/json" ^
-d "{\"dateReported\":\"2024-09-09T10:00:00Z\",\"symptomIds\":[1, 2]}"

echo.

echo Testing API: Get Incident by ID
curl -X GET "http://localhost:5168/api/Incident/2" ^
-H "accept: text/plain"

echo.

echo Testing API: Get Incident Details by ID
curl -X GET "http://localhost:5168/api/Incident/details/1" ^
-H "accept: text/plain"

echo.

echo Testing API: Invalid Request - Create Incident without DiseaseId
curl -X POST "http://localhost:5168/api/Incident/create" ^
-H "Content-Type: application/json" ^
-d "{\"patientId\":1,\"locationId\":1,\"dateReported\":\"2024-09-07T10:00:00Z\",\"symptomIds\":[1]}"

echo.

echo Testing API: Invalid Request - Create Incident with Non-Existing DiseaseId
curl -X POST "http://localhost:5168/api/Incident/create" ^
-H "Content-Type: application/json" ^
-d "{\"diseaseId\":999,\"patientId\":1,\"locationId\":1,\"dateReported\":\"2024-09-07T10:00:00Z\",\"symptomIds\":[1]}"

echo.

echo Testing API: Invalid Request - Create Incident with Non-Existing Optional Ids
curl -X POST "http://localhost:5168/api/Incident/create" ^
-H "Content-Type: application/json" ^
-d "{\"diseaseId\":1,\"patientId\":999,\"locationId\":999,\"dateReported\":\"2024-09-07T10:00:00Z\",\"symptomIds\":[999]}"

echo.