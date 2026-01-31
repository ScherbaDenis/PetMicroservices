@echo off
REM Database generation script for Comment microservice
REM This script applies EF Core migrations to create/update the database

echo === Comment Microservice - Database Generation ===
echo.

REM Navigate to the DataAccess project directory
cd /d "%~dp0Comment.DataAccess.MsSql"

REM Check if EF Core tools are installed
dotnet ef --version >nul 2>&1
if %errorlevel% neq 0 (
    echo Entity Framework Core tools not found. Installing...
    dotnet tool install --global dotnet-ef
    echo.
)

REM Apply migrations to create/update the database
echo Applying migrations to the database...
dotnet ef database update --startup-project ../WebApiComment

if %errorlevel% equ 0 (
    echo.
    echo Database successfully created/updated!
    echo.
    echo Connection string: Server=localhost;Database=CommentDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True
) else (
    echo.
    echo Failed to update database. Please check the error messages above.
    exit /b 1
)
