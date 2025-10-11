#!/bin/bash

echo "🚀 Starting SQL Server with Docker..."

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    echo "❌ Docker is not running. Please start Docker Desktop first."
    exit 1
fi

# Start SQL Server
docker-compose up -d sqlserver

echo "⏳ Waiting for SQL Server to start..."
sleep 10

echo "✅ SQL Server is running!"
echo "📊 Connection details:"
echo "   Server: localhost,1433"
echo "   Database: FlawlessMakeupSumaiaDB"
echo "   Username: sa"
echo "   Password: YourStrong@Passw0rd123"
echo ""
echo "🔧 To connect with SQL Server Management Studio:"
echo "   Server name: localhost,1433"
echo "   Authentication: SQL Server Authentication"
echo "   Login: sa"
echo "   Password: YourStrong@Passw0rd123"
echo ""
echo "🛑 To stop SQL Server: docker-compose down"
