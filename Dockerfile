# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["FlawlessMakeupSumaia.API/FlawlessMakeupSumaia.API.csproj", "FlawlessMakeupSumaia.API/"]
RUN dotnet restore "FlawlessMakeupSumaia.API/FlawlessMakeupSumaia.API.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/FlawlessMakeupSumaia.API"
RUN dotnet publish "FlawlessMakeupSumaia.API.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Expose port (Render will provide PORT env variable)
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "FlawlessMakeupSumaia.API.dll"]

