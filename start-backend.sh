#!/bin/bash

echo "🚀 Starting Flawless Makeup Sumaia Backend..."
echo "============================================"

cd FlawlessMakeupSumaia.API

# Check if .NET is installed
if ! command -v dotnet &> /dev/null; then
    echo "❌ .NET 8.0 SDK is not installed."
    echo "📥 Please install it from: https://dotnet.microsoft.com/download/dotnet/8.0"
    echo ""
    echo "For macOS with Homebrew:"
    echo "  brew install --cask dotnet"
    echo ""
    exit 1
fi

echo "📦 Restoring packages..."
dotnet restore

echo "🗄️ Setting up database..."
dotnet ef database update 2>/dev/null || {
    echo "📥 Installing Entity Framework tools..."
    dotnet tool install --global dotnet-ef
    dotnet ef database update
}

echo "🌟 Starting API server..."
echo "📍 API will be available at: https://localhost:7001"
echo "📖 Swagger documentation: https://localhost:7001/swagger"
echo ""

dotnet run
