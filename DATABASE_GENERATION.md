# Database Generation

This document describes how to generate and update the databases for the PetMicroservices project.

## Overview

The project includes two microservices, each with its own database:
- **Comment Microservice**: Uses `CommentDb` database
- **Template Microservice**: Uses `TemplateDb` database

Each microservice has database generation scripts that automatically apply Entity Framework Core migrations to create or update the database schema.

## Prerequisites

1. **.NET 8 SDK** installed
2. **SQL Server** (or SQL Server Express/LocalDB) running on `localhost`
3. **dotnet-ef tools** (the scripts will automatically install if missing)

## Database Generation Scripts

### Comment Microservice

Located in the `Comment/` directory:

**Linux/macOS:**
```bash
./generate-db.sh
```

**Windows:**
```cmd
generate-db.bat
```

This will:
- Check if EF Core tools are installed (install if needed)
- Apply all migrations to create/update the `CommentDb` database
- Display success message with connection details

### Template Microservice

Located in the `Template/` directory:

**Linux/macOS:**
```bash
./generate-db.sh
```

**Windows:**
```cmd
generate-db.bat
```

This will:
- Check if EF Core tools are installed (install if needed)
- Apply all migrations to create/update the `TemplateDb` database
- Display success message with connection details

## Connection Strings

The default connection strings are defined in the `appsettings.json` files:

**Comment Microservice:**
```
Server=localhost;Database=CommentDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True
```

**Template Microservice:**
```
Server=localhost;Database=TemplateDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True
```

> **⚠️ Security Note**: The connection strings use `TrustServerCertificate=True` which bypasses SSL certificate validation. This is appropriate for local development but **should never be used in production environments**. In production, use properly signed certificates and remove this setting.

## Manual Database Operations

If you need more control, you can use the dotnet-ef CLI directly:

### Install EF Core Tools (if not already installed)
```bash
dotnet tool install --global dotnet-ef
```

### Apply Migrations
```bash
# For Comment microservice
cd Comment/Comment.DataAccess.MsSql
dotnet ef database update --startup-project ../WebApiComment

# For Template microservice
cd Template/Template.DataAccess
dotnet ef database update --startup-project ../WebApiTemplate
```

### Create New Migration
```bash
# For Comment microservice
cd Comment/Comment.DataAccess.MsSql
dotnet ef migrations add MigrationName --startup-project ../WebApiComment

# For Template microservice
cd Template/Template.DataAccess
dotnet ef migrations add MigrationName --startup-project ../WebApiTemplate
```

### Remove Last Migration (if not applied)
```bash
# For Comment microservice
cd Comment/Comment.DataAccess.MsSql
dotnet ef migrations remove --startup-project ../WebApiComment

# For Template microservice
cd Template/Template.DataAccess
dotnet ef migrations remove --startup-project ../WebApiTemplate
```

### Drop Database
```bash
# For Comment microservice
cd Comment/Comment.DataAccess.MsSql
dotnet ef database drop --startup-project ../WebApiComment

# For Template microservice
cd Template/Template.DataAccess
dotnet ef database drop --startup-project ../WebApiTemplate
```

## Automatic Database Creation in Development

Both microservices are configured to automatically apply migrations when running in Development mode:

```csharp
.UseAsyncSeeding(async (context, _, cancellationToken) =>
{
    if (env.IsDevelopment())
    {
        await context.Database.MigrateAsync(cancellationToken);
    }
})
```

This means that when you run the application in Development mode, the database will be automatically created/updated if needed.

## Seed Data

When running in Development mode, the applications will also seed initial data:

**Comment Microservice Seeds:**
- 3 Templates (Customer Feedback, Product Review, Support Ticket)
- 5 Sample Comments

**Template Microservice Seeds:**
- 2 Users (John Doe, Jane Smith)
- 3 Topics (Technology, Science, Education)
- 4 Tags (Programming, Database, Web Development, Machine Learning)
- 2 Templates (Customer Feedback Survey, Employee Onboarding Checklist)
- 3 Questions (Name, Email, Satisfaction rating)

## Troubleshooting

### Connection Issues
If you encounter connection errors, verify that:
- SQL Server is running
- The connection strings in `appsettings.json` match your SQL Server configuration
- You have permissions to create databases on the SQL Server instance

### Migration Issues
If migrations fail to apply:
- Check that all dependent projects build successfully
- Ensure no breaking changes exist in your entity models
- Try dropping the database and recreating it from scratch

### EF Core Tools Not Found
If you get "command not found" errors for `dotnet-ef`:
```bash
dotnet tool install --global dotnet-ef
```

Then restart your terminal or add the tools path to your PATH environment variable.
