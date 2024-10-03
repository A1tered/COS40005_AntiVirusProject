@echo off
rem Create the signatures table if it does not exist
sqlite3 SigHashDB.db "CREATE TABLE IF NOT EXISTS hashSignatures (sigHash TEXT);"

rem For loop thhat loops through all lines in the signatures.txt document and runs an sqlite3 INSERT INTO statement
for /f "tokens=*" %%A in (signatures.txt) do (
    sqlite3 SigHashDB.db "INSERT INTO hashSignatures (sigHash) VALUES ('%%A');"
)

rem Confirm that the process has completed
echo Records inserted successfully from signatures.txt.
pause