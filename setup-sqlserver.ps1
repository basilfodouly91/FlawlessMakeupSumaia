# Setup SQL Server for Flawless Makeup Sumaia

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  SQL Server Setup Script" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if SQL Server is reachable
Write-Host "Checking SQL Server connection..." -ForegroundColor Yellow

$serverTest = Test-NetConnection -ComputerName localhost -Port 1433 -WarningAction SilentlyContinue

if ($serverTest.TcpTestSucceeded) {
    Write-Host "✅ SQL Server is reachable on port 1433" -ForegroundColor Green
    Write-Host ""
    
    # Navigate to API project
    Set-Location FlawlessMakeupSumaia.API
    
    # Create migrations
    Write-Host "Creating database migrations..." -ForegroundColor Yellow
    dotnet ef migrations add InitialCreate
    
    # Apply migrations
    Write-Host "Applying migrations to database..." -ForegroundColor Yellow
    dotnet ef database update
    
    Write-Host ""
    Write-Host "✅ Database setup complete!" -ForegroundColor Green
    Write-Host ""
    Write-Host "You can now run: dotnet run" -ForegroundColor Cyan
    
    Set-Location ..
} else {
    Write-Host "❌ SQL Server is not running on port 1433" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please install and start SQL Server:" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Option 1: Docker (Recommended)" -ForegroundColor Cyan
    Write-Host "  docker-compose up -d" -ForegroundColor White
    Write-Host ""
    Write-Host "Option 2: Install SQL Server Express" -ForegroundColor Cyan
    Write-Host "  https://www.microsoft.com/en-us/sql-server/sql-server-downloads" -ForegroundColor White
    Write-Host ""
}

