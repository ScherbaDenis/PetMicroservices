# MSSQL Setup Guide

## Quick Start with SQL Server

### Option 1: Using Docker (Recommended for Development)

1. Start SQL Server in Docker:
```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong!Passw0rd" \
  -p 1433:1433 --name sqlserver \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

2. Update `appsettings.Development.json`:
```json
{
  "UseInMemoryDatabase": false,
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=AnswerDb;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True"
  }
}
```

3. Run migrations to create the database:
```bash
cd src/Answer.Infrastructure
dotnet ef database update --startup-project ../Answer.Api/Answer.Api.csproj
```

4. Start the application:
```bash
cd src/Answer.Api
dotnet run
```

### Option 2: Using Local SQL Server

1. Install SQL Server Express (if not already installed)
   - Download from: https://www.microsoft.com/sql-server/sql-server-downloads

2. Use Windows Authentication (already configured in `appsettings.json`):
```json
{
  "UseInMemoryDatabase": false,
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AnswerDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

3. Run migrations and start the app (same as Docker option above)

### Option 3: Using Azure SQL Database

1. Create an Azure SQL Database

2. Update connection string:
```json
{
  "UseInMemoryDatabase": false,
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:yourserver.database.windows.net,1433;Database=AnswerDb;User ID=yourusername;Password=yourpassword;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  }
}
```

3. Run migrations and start the app

## Environment Variables

You can also set the configuration via environment variables:

### Linux/macOS:
```bash
export UseInMemoryDatabase=false
export ConnectionStrings__DefaultConnection="Server=localhost;Database=AnswerDb;User Id=sa;Password=YourPassword;TrustServerCertificate=True"
```

### Windows (PowerShell):
```powershell
$env:UseInMemoryDatabase="false"
$env:ConnectionStrings__DefaultConnection="Server=localhost;Database=AnswerDb;User Id=sa;Password=YourPassword;TrustServerCertificate=True"
```

## Verifying the Setup

1. Check if the database was created:
```bash
# Using sqlcmd (if installed)
sqlcmd -S localhost -U sa -P "YourStrong!Passw0rd" -Q "SELECT name FROM sys.databases WHERE name = 'AnswerDb'"
```

2. Check tables:
```bash
sqlcmd -S localhost -U sa -P "YourStrong!Passw0rd" -d AnswerDb -Q "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES"
```

Expected output:
- Users
- Questions
- Templates
- Answers
- __EFMigrationsHistory

## Troubleshooting

### Connection Failed
- Ensure SQL Server is running
- Check firewall settings
- Verify connection string credentials
- For Docker: Ensure port 1433 is not already in use

### Migration Errors
- Ensure the database user has CREATE DATABASE permissions
- Check if migrations folder exists in `src/Answer.Infrastructure/Migrations`
- Try removing the last migration and recreating it

### Performance Issues
- Add indexes for frequently queried columns
- Consider connection pooling settings in connection string
- Monitor SQL Server performance metrics

## Migration Management

### Create New Migration
```bash
cd src/Answer.Infrastructure
dotnet ef migrations add YourMigrationName --startup-project ../Answer.Api/Answer.Api.csproj
```

### Update Database to Latest Migration
```bash
cd src/Answer.Infrastructure
dotnet ef database update --startup-project ../Answer.Api/Answer.Api.csproj
```

### Rollback to Specific Migration
```bash
cd src/Answer.Infrastructure
dotnet ef database update MigrationName --startup-project ../Answer.Api/Answer.Api.csproj
```

### Remove Last Migration (if not applied)
```bash
cd src/Answer.Infrastructure
dotnet ef migrations remove --startup-project ../Answer.Api/Answer.Api.csproj
```

### Generate SQL Script
```bash
cd src/Answer.Infrastructure
dotnet ef migrations script --startup-project ../Answer.Api/Answer.Api.csproj -o migration.sql
```
