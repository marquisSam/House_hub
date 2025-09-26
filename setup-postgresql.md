# PostgreSQL Setup Guide

## What's Changed

Your project has been successfully converted from SQL Server to PostgreSQL! Here's what was updated:

### 1. Package References

- ✅ Replaced `Microsoft.EntityFrameworkCore.SqlServer` with `Npgsql.EntityFrameworkCore.PostgreSQL`

### 2. Database Configuration

- ✅ Updated `Program.cs` to use `UseNpgsql()` instead of `UseSqlServer()`
- ✅ Updated `ItemDbContext.cs` to use PostgreSQL
- ✅ Updated connection strings in all config files

### 3. Docker Configuration

- ✅ Replaced SQL Server container with PostgreSQL 16 Alpine
- ✅ Updated environment variables and health checks
- ✅ Changed port from 1433 to 5432

### 4. Database Credentials

- **Database**: HouseHubApiDb
- **Username**: househub_user
- **Password**: househub_password
- **Port**: 5432

## How to Start

### 1. Start Docker Desktop

Make sure Docker Desktop is running on your machine.

### 2. Start PostgreSQL Container

```bash
docker-compose up database -d
```

### 3. Update Database with New Migrations

```bash
cd backend
dotnet ef database update
```

### 4. Start Backend

```bash
dotnet run
```

### 5. Start Complete Stack (Optional)

```bash
docker-compose up -d
```

## Connection Strings

### Local Development

```json
"ConnectionString": "Host=localhost;Port=5432;Database=HouseHubApiDb;Username=househub_user;Password=househub_password;"
```

### Docker Environment

```json
"ConnectionString": "Host=database;Port=5432;Database=HouseHubApiDb;Username=househub_user;Password=househub_password;"
```

## Database Management

### Connect to PostgreSQL Container

```bash
docker exec -it househub-database psql -U househub_user -d HouseHubApiDb
```

### Common PostgreSQL Commands

- `\l` - List all databases
- `\dt` - List all tables
- `\d table_name` - Describe table structure
- `\q` - Quit psql

## Benefits of PostgreSQL

1. **Open Source**: No licensing costs
2. **Better Performance**: Often faster than SQL Server for many workloads
3. **Advanced Features**: JSON support, full-text search, etc.
4. **Cross-Platform**: Runs on Linux, Windows, macOS
5. **Standards Compliant**: Better SQL standard compliance

## Notes

- All your existing EF Core models and configurations remain the same
- No changes needed to your Angular frontend
- The migration from SQL Server to PostgreSQL is complete
- Your data structure and relationships are preserved
