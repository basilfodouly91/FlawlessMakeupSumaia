#!/bin/bash

echo "🌸 Flawless Makeup Sumaia Setup Script 🌸"
echo "=========================================="

# Check if .NET is installed
if ! command -v dotnet &> /dev/null; then
    echo "❌ .NET 8.0 SDK is not installed."
    echo "📥 Please install .NET 8.0 SDK from: https://dotnet.microsoft.com/download/dotnet/8.0"
    echo ""
fi

# Check if Node.js is installed
if ! command -v node &> /dev/null; then
    echo "❌ Node.js is not installed."
    echo "📥 Please install Node.js from: https://nodejs.org/"
    echo ""
fi

# Check if Angular CLI is installed
if ! command -v ng &> /dev/null; then
    echo "❌ Angular CLI is not installed."
    echo "📥 Installing Angular CLI..."
    npm install -g @angular/cli
fi

echo "🔧 Setting up Backend (ASP.NET Core API)..."
cd FlawlessMakeupSumaia.API

if command -v dotnet &> /dev/null; then
    echo "📦 Restoring NuGet packages..."
    dotnet restore

    echo "🗄️ Creating database..."
    dotnet ef database update 2>/dev/null || echo "⚠️ Entity Framework tools not installed. Run: dotnet tool install --global dotnet-ef"

    echo "✅ Backend setup complete!"
else
    echo "⚠️ Skipping backend setup - .NET not installed"
fi

cd ..

echo ""
echo "🎨 Setting up Frontend (Angular)..."
cd FlawlessMakeupSumaia.Client

if command -v npm &> /dev/null; then
    echo "📦 Installing npm packages..."
    npm install

    echo "🏗️ Building application..."
    npm run build

    echo "✅ Frontend setup complete!"
else
    echo "⚠️ Skipping frontend setup - Node.js not installed"
fi

cd ..

echo ""
echo "🎉 Setup Complete!"
echo "==================="
echo ""
echo "🚀 To start the application:"
echo ""
echo "Backend (API):"
echo "  cd FlawlessMakeupSumaia.API"
echo "  dotnet run"
echo "  → API will be available at https://localhost:7001"
echo ""
echo "Frontend (Angular):"
echo "  cd FlawlessMakeupSumaia.Client"
echo "  ng serve"
echo "  → App will be available at http://localhost:4200"
echo ""
echo "📚 Documentation: See README.md for detailed instructions"
echo ""
echo "Happy coding! 💄✨"
