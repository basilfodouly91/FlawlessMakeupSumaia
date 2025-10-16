# Manual SQL Server Startup Script
# This script helps you start SQL Server if Docker is not available

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  SQL Server Manual Start Helper" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if Docker Desktop is installed
$dockerDesktopPath = "C:\Program Files\Docker\Docker\Docker Desktop.exe"
if (Test-Path $dockerDesktopPath) {
    Write-Host "Docker Desktop found! Starting Docker Desktop..." -ForegroundColor Green
    Start-Process $dockerDesktopPath
    Write-Host "Waiting for Docker Desktop to start (30 seconds)..." -ForegroundColor Yellow
    Start-Sleep -Seconds 30
    
    # Try to start SQL Server container
    Write-Host "Attempting to start SQL Server container..." -ForegroundColor Yellow
    & docker-compose up -d
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ SQL Server container started successfully!" -ForegroundColor Green
        Write-Host "Waiting for SQL Server to initialize (10 seconds)..." -ForegroundColor Yellow
        Start-Sleep -Seconds 10
        Write-Host ""
        Write-Host "You can now run the migration with:" -ForegroundColor Cyan
        Write-Host "  cd FlawlessMakeupSumaia.API" -ForegroundColor White
        Write-Host "  dotnet ef database update" -ForegroundColor White
    }
} else {
    Write-Host "Docker Desktop not found." -ForegroundColor Yellow
    Write-Host ""
    Write-Host "OPTION 1: Install and Start Docker Desktop" -ForegroundColor Cyan
    Write-Host "1. Download Docker Desktop: https://www.docker.com/products/docker-desktop" -ForegroundColor White
    Write-Host "2. Install and start Docker Desktop" -ForegroundColor White
    Write-Host "3. Run: docker-compose up -d" -ForegroundColor White
    Write-Host ""
    Write-Host "OPTION 2: Use SQL Server LocalDB (Simpler)" -ForegroundColor Cyan
    Write-Host "Update your connection string in appsettings.json to:" -ForegroundColor White
    Write-Host '  "Server=(localdb)\\MSSQLLocalDB;Database=FlawlessMakeupDB;Trusted_Connection=True;"' -ForegroundColor Yellow
    Write-Host ""
}










