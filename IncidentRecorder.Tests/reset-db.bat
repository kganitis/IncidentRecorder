@echo off
REM Path to the SQLite database
set DB_PATH=../IncidentRecorder/incidentrecorder.db

REM Table name where you want to reset the auto-increment
set TABLE_NAME=Diseases

REM Path to sqlite3.exe (Make sure sqlite3.exe is in your PATH or provide the full path)
set SQLITE_PATH=sqlite3

REM Print message
echo Deleting all records and resetting auto-increment values for all tables in the database...

REM Get the list of all table names in the database, excluding sqlite_sequence and system tables
for /f "tokens=*" %%A in ('%SQLITE_PATH% %DB_PATH% ".tables"') do (
    REM Split the table names into individual tokens and process them one by one
    for %%B in (%%A) do (
        REM Skip the __EFMigrationsHistory table
        if "%%B" neq "__EFMigrationsHistory" (
            REM Delete all records from each table
            echo Deleting all records from table %%B...
            %SQLITE_PATH% %DB_PATH% "DELETE FROM %%B;"

            REM Reset auto-increment for each table
            echo Resetting auto-increment for table %%B...
            %SQLITE_PATH% %DB_PATH% "DELETE FROM sqlite_sequence WHERE name = '%%B';"
        )
    )
)

REM Verify the result by outputting the sqlite_sequence table (should be empty now)
echo Auto-increment reset for all tables except __EFMigrationsHistory.
echo Current sqlite_sequence table (should be empty):
%SQLITE_PATH% %DB_PATH% "SELECT * FROM sqlite_sequence;"

REM Exit
echo Operation completed.
