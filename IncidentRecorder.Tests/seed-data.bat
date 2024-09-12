@echo off

set LOCALHOST=http://localhost:5168

echo Seeding Diseases...
curl -X POST "%LOCALHOST%/api/Disease" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"name\":\"COVID-19\",\"description\":\"Coronavirus disease\"}"

curl -X POST "%LOCALHOST%/api/Disease" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"name\":\"Gastroenteritis\",\"description\":\"Inflammation of the stomach and intestines\"}"

curl -X POST "%LOCALHOST%/api/Disease" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"name\":\"Malaria\",\"description\":\"Mosquito-borne infectious disease\"}"

curl -X POST "%LOCALHOST%/api/Disease" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"name\":\"Tuberculosis\",\"description\":\"Bacterial infection that mainly affects the lungs\"}"

curl -X POST "%LOCALHOST%/api/Disease" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"name\":\"Dengue Fever\",\"description\":\"Mosquito-borne viral infection\"}"

curl -X POST "%LOCALHOST%/api/Disease" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"name\":\"Chickenpox\",\"description\":\"Highly contagious viral infection causing an itchy rash\"}"

echo.
echo.

echo Seeding Patients...
curl -X POST "%LOCALHOST%/api/Patient" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"NIN\":\"000000001\",\"firstName\":\"Kostas\",\"lastName\":\"Ganitis\",\"dateOfBirth\":\"1992-01-01T00:00:00Z\",\"gender\":\"Male\",\"contactInfo\":\"k.ganitis@gmail.com\"}"

curl -X POST "%LOCALHOST%/api/Patient" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"NIN\":\"000000002\",\"firstName\":\"Efthymis\",\"lastName\":\"Alepis\",\"dateOfBirth\":\"1980-02-02T00:00:00Z\",\"gender\":\"Male\",\"contactInfo\":\"e.alepis@unipi.gr\"}"

curl -X POST "%LOCALHOST%/api/Patient" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"NIN\":\"000000003\",\"firstName\":\"Maria\",\"lastName\":\"Konstantinou\",\"dateOfBirth\":\"1980-03-03T00:00:00Z\",\"gender\":\"Female\",\"contactInfo\":\"maria.kon@hotmail.com\"}"

curl -X POST "%LOCALHOST%/api/Patient" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"NIN\":\"000000004\",\"firstName\":\"Giannis\",\"lastName\":\"Nikolaou\",\"dateOfBirth\":\"1990-04-04T00:00:00Z\",\"gender\":\"Male\",\"contactInfo\":\"john.nikolaou@yahoo.gr\"}"

curl -X POST "%LOCALHOST%/api/Patient" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"NIN\":\"000000005\",\"firstName\":\"Emmanouela\",\"lastName\":\"Giannakidi\",\"dateOfBirth\":\"2000-05-05T00:00:00Z\",\"gender\":\"Female\",\"contactInfo\":\"emma.giannak@proton.com\"}"

curl -X POST "%LOCALHOST%/api/Patient" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"NIN\":\"000000006\",\"firstName\":\"Sophia\",\"lastName\":\"Rizou\",\"dateOfBirth\":\"2010-06-06T00:00:00Z\",\"gender\":\"Female\",\"contactInfo\":\"s.rizou@gmail.com\"}"

echo.
echo.

echo Seeding Locations...
curl -X POST "%LOCALHOST%/api/Location" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"city\":\"Athens\",\"country\":\"Greece\"}"

curl -X POST "%LOCALHOST%/api/Location" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"city\":\"Thessaloniki\",\"country\":\"Greece\"}"

curl -X POST "%LOCALHOST%/api/Location" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"city\":\"Rome\",\"country\":\"Italy\"}"

curl -X POST "%LOCALHOST%/api/Location" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"city\":\"Berlin\",\"country\":\"Germany\"}"

curl -X POST "%LOCALHOST%/api/Location" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"city\":\"Paphos\",\"country\":\"Cyprus\"}"

curl -X POST "%LOCALHOST%/api/Location" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"city\":\"Patra\",\"country\":\"Greece\"}"

echo.
echo.

echo Seeding Symptoms...
curl -X POST "%LOCALHOST%/api/Symptom" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"name\":\"Cough\",\"description\":\"Persistent cough\"}"

curl -X POST "%LOCALHOST%/api/Symptom" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"name\":\"Nausea\",\"description\":\"Feeling of sickness with an inclination to vomit\"}"

curl -X POST "%LOCALHOST%/api/Symptom" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"name\":\"Chills\",\"description\":\"Feeling of coldness despite a fever\"}"

curl -X POST "%LOCALHOST%/api/Symptom" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"name\":\"Coughing up blood\",\"description\":\"Coughing up blood or bloody mucus\"}"

curl -X POST "%LOCALHOST%/api/Symptom" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"name\":\"Joint Pain\",\"description\":\"Severe pain in muscles and joints\"}"

curl -X POST "%LOCALHOST%/api/Symptom" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"name\":\"Rash\",\"description\":\"Red, itchy skin rash with blisters\"}"

echo.
echo.

echo Seeding Incidents...
curl -X POST "%LOCALHOST%/api/Incident/create" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"diseaseId\":1,\"patientId\":1,\"locationId\":1,\"dateReported\":\"2024-09-07T10:00:00Z\",\"symptomIds\":[1, 3]}"

curl -X POST "%LOCALHOST%/api/Incident/create" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"diseaseId\":2,\"patientId\":2,\"locationId\":2,\"dateReported\":\"2024-09-08T10:00:00Z\",\"symptomIds\":[2, 3]}"

curl -X POST "%LOCALHOST%/api/Incident/create" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"diseaseId\":3,\"patientId\":3,\"locationId\":3,\"dateReported\":\"2024-09-09T10:00:00Z\",\"symptomIds\":[3, 5]}"

curl -X POST "%LOCALHOST%/api/Incident/create" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"diseaseId\":4,\"patientId\":4,\"locationId\":4,\"dateReported\":\"2024-09-10T10:00:00Z\",\"symptomIds\":[4]}"

curl -X POST "%LOCALHOST%/api/Incident/create" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"diseaseId\":5,\"patientId\":5,\"locationId\":5,\"dateReported\":\"2024-09-11T10:00:00Z\",\"symptomIds\":[5]}"

curl -X POST "%LOCALHOST%/api/Incident/create" ^
-H "Accept: application/json" ^
-H "Content-Type: application/json" ^
-d "{\"diseaseId\":6,\"patientId\":6,\"locationId\":6,\"dateReported\":\"2024-09-12T10:00:00Z\",\"symptomIds\":[6]}"

echo.
echo.
echo Seeding completed.
pause
