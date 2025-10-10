@echo off
echo 🌸 Flawless Makeup Sumaia Setup Script 🌸
echo ==========================================

REM Check if .NET is installed
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ .NET 8.0 SDK is not installed.
    echo 📥 Please install .NET 8.0 SDK from: https://dotnet.microsoft.com/download/dotnet/8.0
    echo.
)

REM Check if Node.js is installed
node --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ Node.js is not installed.
    echo 📥 Please install Node.js from: https://nodejs.org/
    echo.
)

REM Check if Angular CLI is installed
ng version >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ Angular CLI is not installed.
    echo 📥 Installing Angular CLI...
    npm install -g @angular/cli
)

echo 🔧 Setting up Backend (ASP.NET Core API)...
cd FlawlessMakeupSumaia.API

dotnet --version >nul 2>&1
if %errorlevel% equ 0 (
    echo 📦 Restoring NuGet packages...
    dotnet restore

    echo 🗄️ Creating database...
    dotnet ef database update 2>nul || echo ⚠️ Entity Framework tools not installed. Run: dotnet tool install --global dotnet-ef

    echo ✅ Backend setup complete!
) else (
    echo ⚠️ Skipping backend setup - .NET not installed
)

cd ..

echo.
echo 🎨 Setting up Frontend (Angular)...
cd FlawlessMakeupSumaia.Client

node --version >nul 2>&1
if %errorlevel% equ 0 (
    echo 📦 Installing npm packages...
    npm install

    echo 🏗️ Building application...
    npm run build

    echo ✅ Frontend setup complete!
) else (
    echo ⚠️ Skipping frontend setup - Node.js not installed
)

cd ..

echo.
echo 🎉 Setup Complete!
echo ===================
echo.
echo 🚀 To start the application:
echo.
echo Backend (API):
echo   cd FlawlessMakeupSumaia.API
echo   dotnet run
echo   → API will be available at https://localhost:7001
echo.
echo Frontend (Angular):
echo   cd FlawlessMakeupSumaia.Client
echo   ng serve
echo   → App will be available at http://localhost:4200
echo.
echo 📚 Documentation: See README.md for detailed instructions
echo.
echo Happy coding! 💄✨
pause
