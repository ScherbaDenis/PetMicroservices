# Quick Reference: Switching Between Databases

## Use In-Memory Database (Default)

**appsettings.json:**
```json
{
  "UseInMemoryDatabase": true
}
```

No additional setup required. Just run:
```bash
cd src/Answer.Api
dotnet run
```

---

## Use SQL Server Database

### Step 1: Start SQL Server

**Option A - Docker (Recommended):**
```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong!Passw0rd" \
  -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2022-latest
```

**Option B - Local SQL Server:**
```bash
# Windows: SQL Server Express already installed
# Server: (localdb)\mssqllocaldb
```

### Step 2: Update Configuration

**appsettings.Development.json:**
```json
{
  "UseInMemoryDatabase": false,
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=AnswerDb;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True"
  }
}
```

### Step 3: Create Database Schema

```bash
cd src/Answer.Infrastructure
dotnet ef database update --startup-project ../Answer.Api/Answer.Api.csproj
```

### Step 4: Run the Application

```bash
cd src/Answer.Api
dotnet run
```

---

## Quick Commands

### Check which database is being used
```bash
# Look for this log entry when app starts:
# In-Memory: "Using in-memory database"
# SQL Server: Check EF Core logs for SQL connections
```

### Reset In-Memory Database
```bash
# Just restart the application - data is lost
dotnet run
```

### Reset SQL Server Database
```bash
# Drop and recreate
cd src/Answer.Infrastructure
dotnet ef database drop --startup-project ../Answer.Api/Answer.Api.csproj --force
dotnet ef database update --startup-project ../Answer.Api/Answer.Api.csproj
```

### View SQL Server Data
```bash
# Using sqlcmd
sqlcmd -S localhost -U sa -P "YourStrong!Passw0rd" -d AnswerDb -Q "SELECT * FROM Users"
```

---

## Environment Variables (Alternative to appsettings)

### Linux/macOS:
```bash
export UseInMemoryDatabase=false
export ConnectionStrings__DefaultConnection="Server=localhost;Database=AnswerDb;User Id=sa;Password=YourPassword;TrustServerCertificate=True"
```

### Windows CMD:
```cmd
set UseInMemoryDatabase=false
set ConnectionStrings__DefaultConnection=Server=localhost;Database=AnswerDb;User Id=sa;Password=YourPassword;TrustServerCertificate=True
```

### Windows PowerShell:
```powershell
$env:UseInMemoryDatabase="false"
$env:ConnectionStrings__DefaultConnection="Server=localhost;Database=AnswerDb;User Id=sa;Password=YourPassword;TrustServerCertificate=True"
```

---

## Troubleshooting

### "Cannot connect to SQL Server"
```bash
# Check if SQL Server is running
docker ps | grep sqlserver  # For Docker
# OR
# Check Windows Services for SQL Server
```

### "Login failed for user 'sa'"
- Verify password in connection string matches what you set
- For Docker: Check the SA_PASSWORD environment variable

### "Database 'AnswerDb' does not exist"
```bash
# Run migrations
cd src/Answer.Infrastructure
dotnet ef database update --startup-project ../Answer.Api/Answer.Api.csproj
```

### Switch back to In-Memory quickly
```bash
# Set UseInMemoryDatabase=true in appsettings.json
# OR use environment variable
export UseInMemoryDatabase=true  # Linux/macOS
$env:UseInMemoryDatabase="true"   # Windows PowerShell
```
