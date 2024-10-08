# Incident Recorder API

Το Incident Recorder είναι ένα **RESTful API**, κατασκευασμένο σε **ASP.NET Core**, για την **καταγραφή και διαχείριση κρουσμάτων** ασθενειών μαζί με όλα τα σχετικά στοιχεία (ασθενείς, συμπτώματα, τοποθεσίες).

## Πίνακας Περιεχομένων

- [Δυνατότητες](#δυνατότητες)
- [Μεθοδολογία Ανάπτυξης](#μεθοδολογία-ανάπτυξης)
- [Εγκατάσταση & Εκτέλεση του API](#εγκατάσταση--εκτέλεση-του-api)
- [Αρχικοποίηση Βάσης Δεδομένων](#αρχικοποίηση-βάσης-δεδομένων)
- [API Endpoints](#api-endpoints)
  - [Ασθένειες](#ασθένειες-diseases)
  - [Ασθενείς](#ασθενείς-patients)
  - [Τοποθεσίες](#τοποθεσίες-locations)
  - [Συμπτώματα](#συμπτώματα-symptoms)
  - [Κρούσματα](#κρούσματα-incidents)
- [Παραδείγματα Χρήσης](#παραδείγματα-χρήσης)
- [Διαχείριση Σφαλμάτων](#διαχείριση-σφαλμάτων)
- [Διεπαφή Χρήστη με Swagger](#διεπαφή-χρήστη-με-swagger)
- [Εκτέλεση Test](#εκτέλεση-test)
- [Μελλοντικό Έργο](#μελλοντικό-έργο)

## Δυνατότητες

- **Καταγραφή και διαχείριση κρουσμάτων** μαζί με όλα τα σχετικά στοιχεία (ασθένειες, ασθενείς, συμπτώματα, τοποθεσίες).
- Υποστήριξη όλων των βασικών **CRUD Operations** για όλες τις οντότητες.
- Επικύρωση των δεδομένων εισόδου και εμφάνιση φιλικών προς τον χρήστη μηνυμάτων σφαλμάτων.
- Τεκμηρίωση και διεπαφή χρήστη μέσω **Swagger**.

## Μεθοδολογία Ανάπτυξης

- Γλώσσα προγραμματισμού **C#**.
- Χρήση του Microsoft **Entity Framework** και του **ASP.NET Core MVC**.
- Υλοποίηση της βάσης δεδομένων σε **SQLite**.
- Χρήση **Data Access Objects** (DTOs) για τη διαχείριση δεδομένων.
- Προσαρμοσμένα μηνύματα σφαλμάτων για την απόκρυψη ευαίσθητων πληροφοριών.
- **Unit & Integration Tests** για κάθε Controller με τη χρήση in-memory databases.
- **End-to-end tests** μέσω προσαρμοσμένων scripts.
- Χρήση **Swagger** για την τεκμηρίωση και τη διεπαφή χρήστη.
- **Git Version Control** καθ' όλη τη διάρκεια της ανάπτυξης.

## Εγκατάσταση & Εκτέλεση του API

Για την εγκατάσταση και εκτέλεση του API, βεβαιωθείτε ότι έχετε τα ακόλουθα εγκατεστημένα στον υπολογιστή σας:

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- [SQLite](https://www.sqlite.org/download.html)
- Git

Για να εκτελέσετε τοπικά το API, ακολουθήστε τα παρακάτω βήματα:

Κάνετε clone το git repository:

``` bash
git clone https://github.com/kganitis/IncidentRecorder.git
```

Περιηγηθείτε στο directory του project:

``` bash
cd IncidentRecorder/IncidentRecorder
```

Ενδεχομένως να χρειαστεί να πραγματοποιήσετε επαναφορά των απαιτούμενων πακέτων:

``` bash
dotnet restore
```

Ενδεχομένως να χρειαστεί να αρχικοποιήσετε τη βάση δεδομένων και να εφαρμόσετε τα migrations:

``` bash
dotnet ef database update
```

Η εντολή αυτή θα δημιουργήσει τους απαραίτητους πίνακες στη βάση δεδομένων βάσει του τελευταίου migration.

Εκτελέστε το API:

```bash
dotnet run
```

Από προεπιλογή, το API φιλοξενείται στην τοποθεσία **<http://localhost:5168>**
Μπορείτε να προσαρμόσετε τον αριθμό της θύρας από το αρχείο `Properties/launchSettings.json`.

## Αρχικοποίηση Βάσης Δεδομένων

Η βάση δεδομένων είναι ήδη αρχικοποιημένη με έξι (6) ενδεικτικές οντότητες από κάθε κλάση (κρούσμα, ασθένεια, ασθενής, σύμπτωμα, τοποθεσία).

- Για να **επαναφέρετε** τη βάση δεδομένων, διαγράφοντας όλες τις καταχωρήσεις, εκτελέστε το script
`IncidentRecorder/IncidentRecorder.Tests/reset-db.bat`.

- Για να **επανατροφοδοτήσετε** τη βάση δεδομένων με τις αρχικές καταχωρήσεις, εκτελέστε το script
`IncidentRecorder/IncidentRecorder.Tests/seed-data.bat`.

- Μπορείτε να υποβάλλετε αυτοματοποιημένα μία πληθώρα δοκιμαστικών requests, που καλύπτουν όλες τις περιπτώσεις, εκτελώντας το script
`IncidentRecorder/IncidentRecorder.Tests/test-api.bat`.

    **ΠΡΟΣΟΧΗ**: Φροντίστε πρώτα να πραγματοποιήσετε επαναφορά της βάσης δεδομένων εκτελώντας το script `reset-db.bat`.

## API Endpoints

### Ασθένειες (Diseases)

`GET /api/disease`: Ανάκτηση όλων των ασθενειών.

`GET /api/disease/{id}`: Ανάκτηση ασθένειας με βάση το ID.

`POST /api/disease`: Δημιουργία νέας ασθένειας.

`PUT /api/disease/{id}`: Ενημέρωση υπάρχουσας ασθένειας.

`DELETE /api/disease/{id}`: Διαγραφή ασθένειας.

### Ασθενείς (Patients)

`GET /api/patient`: Ανάκτηση όλων των ασθενών.

`GET /api/patient/{id}`: Ανάκτηση ασθενούς με βάση το ID.

`POST /api/patient`: Δημιουργία νέου ασθενούς.

`PUT /api/patient/{id}`: Ενημέρωση υπάρχοντος ασθενούς.

`DELETE /api/patient/{id}`: Διαγραφή ασθενούς.

### Τοποθεσίες (Locations)

`GET /api/location`: Ανάκτηση όλων των τοποθεσιών.

`GET /api/location/{id}`: Ανάκτηση τοποθεσίας με βάση το ID.

`POST /api/location`: Δημιουργία νέας τοποθεσίας.

`PUT /api/location/{id}`: Ενημέρωση υπάρχουσας τοποθεσίας.

`DELETE /api/location/{id}`: Διαγραφή τοποθεσίας.

### Συμπτώματα (Symptoms)

`GET /api/symptom`: Ανάκτηση όλων των συμπτωμάτων.

`GET /api/symptom/{id}`: Ανάκτηση συμπτώματος με βάση το ID.

`POST /api/symptom`: Δημιουργία νέου συμπτώματος.

`PUT /api/symptom/{id}`: Ενημέρωση υπάρχοντος συμπτώματος.

`DELETE /api/symptom/{id}`: Διαγραφή συμπτώματος.

### Κρούσματα (Incidents)

`GET /api/incident/all`: Ανάκτηση όλων των κρουσμάτων.

`GET /api/incident/{id}`: Ανάκτηση κρούσματος με βάση το ID.

`POST /api/incident/create`: Δημιουργία νέου κρούσματος.

`PUT /api/incident/{id}`: Ενημέρωση υπάρχοντος κρούσματος.

`DELETE /api/incident/{id}`: Διαγραφή κρούσματος.

`GET /api/incident/list`: Ανάκτηση συνοπτικής λίστας κρουσμάτων.

`GET /api/incident/details/{id}`: Ανάκτηση λεπτομερών πληροφοριών για ένα κρούσμα.

## Παραδείγματα χρήσης
  
### Ανάκτηση κρούσματος με βάση το ID
  
#### `Request`

``` bash
GET /api/incident/3
```

#### `Response`

``` json
{
  "id": 3,
  "diseaseName": "Malaria",
  "patientName": "Maria Konstantinou",
  "location": "Rome, Italy",
  "dateReported": "2024-09-09T10:00:00Z",
  "symptoms": ["Chills", "Joint Pain"]
}
```

### Ενημέρωση υπάρχοντος ασθενή με βάση το ID

#### `Request`

``` bash
PUT /api/patient/2
Content-Type: application/json
{
  "contactInfo": "e.alepis@newemail.com"
}
```

#### `Response`

``` bash
204 No Content
```

## Διαχείριση Σφαλμάτων

Το API υποστηρίζει τους καθιερωμένους HTTP status κωδικούς για τη διαχείριση σφαλμάτων:

- `200 OK`: Το αίτημα ήταν επιτυχές.
- `201 Created`: Νέο περιεχόμενο δημιουργήθηκε με επιτυχία.
- `204 No Content`: Το αίτημα ήταν επιτυχές, αλλά δεν υπάρχει περιεχόμενο για επιστροφή.
- `400 Bad Request`: Το αίτημα δεν μπορούσε να επεξεργαστεί λόγω μη έγκυρης εισόδου.
- `404 Not Found`: Το περιεχόμενο δεν βρέθηκε.
- `409 Conflict`: Υπάρχει σύγκρουση, όπως για παράδειγμα μια διπλή καταχώρηση.

Τα validation errors και τα απουσιάζοντα ή μη έγκυρα πεδία επιστρέφουν μηνύματα σφάλματος σε μορφή JSON, για παράδειγμα:

``` json
{
  "title":"One or more validation errors occurred.",
  "status":400,
  "detail":"The request contains missing or invalid data.",
  "instance":"/api/Incident/create",
  "errors":
    {
      "$":["JSON deserialization was missing required properties, including the following: diseaseId"]
    }
}
```

## Διεπαφή Χρήστη με Swagger

Για την τεκμηρίωση του API χρησιμοποιήθηκε το Swagger.

Για να αλληλεπιδράσετε με τη διεπαφή χρήστη του Swagger και να προβάλλετε την τεκμηρίωση, περιηγηθείτε στην τοποθεσία:
**<https://localhost:5168/swagger>**

Από εδώ μπορείτε να εξερευνήσετε όλα τα endpoints αναλυτικά, να προβάλλετε τα format των requests και των responses, και να δοκιμάσετε το API απευθείας στον browser σας.

## Εκτέλεση Test

Το solution περιέχει πληθώρα από **unit** και **integration tests**. Μπορείτε να εκτελέσετε τα test με την ακόλουθη εντολή:

``` bash
dotnet test
```

Η εντολή θα εντοπίσει και εκτελέσει όλα τα test, παρέχοντας αναλυτική ενημέρωση για τα αποτελέσματά τους.

## Μελλοντικό Έργο

- Χρήση **Services** για τον διαχωρισμό της λογικής από τους Controllers.
- Δημιουργία **Mappers** (π.χ. AutoMapper) για την αυτόματη αντιστοίχιση του μοντέλου στα DTOs.
- Επέκταση του Data Layer με χρήση **Entities** και **Repositories**.
- Εισαγωγή ρόλων χρηστών και μηχανισμών **authentication**.
- Ανάπτυξη ενδεικτικής **front-end εφαρμογής** που θα χρησιμοποιεί το API.
