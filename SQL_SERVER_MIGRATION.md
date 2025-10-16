# SQL Server Migration Guide

## ✅ Migration Complete!

Your application has been successfully migrated from SQLite to SQL Server.

## 📋 Changes Made

### 1. **Project File** (`FlawlessMakeupSumaia.API.csproj`)
- ❌ Removed: `Microsoft.EntityFrameworkCore.Sqlite`
- ✅ Added: `Microsoft.EntityFrameworkCore.SqlServer`

### 2. **Connection Strings**
- **appsettings.json**: Updated to SQL Server connection
- **appsettings.Development.json**: Updated to SQL Server connection
- **NEW: appsettings.LocalDB.json**: Alternative LocalDB configuration

### 3. **Program.cs**
- Changed from `options.UseSqlite()` to `options.UseSqlServer()`

### 4. **Database Files**
- Deleted old SQLite database files (`.db`, `.db-shm`, `.db-wal`)
- Deleted old migrations (will be recreated for SQL Server)

## 🚀 Quick Start

### Option 1: Using Docker (Recommended)

1. **Install Docker Desktop**
   - Download: https://www.docker.com/products/docker-desktop
   - Install and start Docker Desktop

2. **Start SQL Server**
   ```powershell
   docker-compose up -d
   ```

3. **Setup Database**
   ```powershell
   .\setup-sqlserver.ps1
   ```

4. **Run Application**
   ```powershell
   cd FlawlessMakeupSumaia.API
   dotnet run --urls http://localhost:5001
   ```

### Option 2: Using SQL Server Express

1. **Download and Install**
   - Visit: https://www.microsoft.com/en-us/sql-server/sql-server-downloads
   - Choose: SQL Server 2022 Express
   - Install with default settings

2. **Enable TCP/IP** (if needed)
   - Open SQL Server Configuration Manager
   - Enable TCP/IP protocol
   - Restart SQL Server service

3. **Setup Database**
   ```powershell
   .\setup-sqlserver.ps1
   ```

4. **Run Application**
   ```powershell
   cd FlawlessMakeupSumaia.API
   dotnet run --urls http://localhost:5001
   ```

### Option 3: Using LocalDB (Lightweight)

1. **Check if LocalDB is installed** (comes with Visual Studio)
   ```powershell
   sqllocaldb info
   ```

2. **Use LocalDB Configuration**
   ```powershell
   Copy-Item appsettings.LocalDB.json FlawlessMakeupSumaia.API\appsettings.Development.json
   ```

3. **Setup Database**
   ```powershell
   cd FlawlessMakeupSumaia.API
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   cd ..
   ```

4. **Run Application**
   ```powershell
   cd FlawlessMakeupSumaia.API
   dotnet run --urls http://localhost:5001
   ```

## 📊 Connection Strings

### Standard SQL Server (Docker/Express)
```
Server=localhost,1433;
Database=FlawlessMakeupDB_Dev;
User Id=sa;
Password=YourStrong@Passw0rd123;
TrustServerCertificate=True;
```

### LocalDB
```
Server=(localdb)\mssqllocaldb;
Database=FlawlessMakeupDB;
Trusted_Connection=True;
MultipleActiveResultSets=true
```

## 🔧 Manual Database Setup

If the automated script doesn't work, you can manually set up the database:

```powershell
cd FlawlessMakeupSumaia.API

# Create migration
dotnet ef migrations add InitialCreate

# Apply migration
dotnet ef database update

# Run application
dotnet run --urls http://localhost:5001
```

## 🐛 Troubleshooting

### Error: "SQL Server does not exist or access denied"
- **Solution 1**: Install SQL Server (see options above)
- **Solution 2**: Check if SQL Server service is running
- **Solution 3**: Verify connection string in `appsettings.Development.json`

### Error: "Login failed for user 'sa'"
- **Solution**: Check password in connection string matches SQL Server configuration
- **For Docker**: Password is `YourStrong@Passw0rd123`

### Error: "Cannot open database"
- **Solution**: Run `dotnet ef database update` to create the database

### Port 1433 already in use
- **Solution**: Another SQL Server instance might be running
- **Check**: `netstat -ano | findstr :1433`
- **Fix**: Stop other SQL Server instance or change port in connection string

## 📝 Notes

- **Development Database**: `FlawlessMakeupDB_Dev`
- **Production Database**: `FlawlessMakeupDB`
- **Default Admin**: admin@flawlessmakeup.com / Admin@123
- All previous data from SQLite needs to be re-entered or migrated manually

## 🔄 Reverting to SQLite (if needed)

If you need to revert back to SQLite:

1. Change `.csproj`:
   ```xml
   <PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="8.0.8" />
   ```

2. Update `Program.cs`:
   ```csharp
   options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
   ```

3. Update connection string:
   ```json
   "DefaultConnection": "Data Source=flawlessmakeup-dev.db"
   ```

4. Restore packages and recreate database

