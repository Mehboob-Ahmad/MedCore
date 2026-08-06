# Use the official .NET 9 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and restore dependencies
COPY ["services/api/MedCore.API/MedCore.API.csproj", "services/api/MedCore.API/"]
COPY ["services/api/MedCore.Application/MedCore.Application.csproj", "services/api/MedCore.Application/"]
COPY ["services/api/MedCore.Domain/MedCore.Domain.csproj", "services/api/MedCore.Domain/"]
COPY ["services/api/MedCore.Infrastructure/MedCore.Infrastructure.csproj", "services/api/MedCore.Infrastructure/"]
COPY ["services/api/MedCore.Persistence/MedCore.Persistence.csproj", "services/api/MedCore.Persistence/"]
COPY ["services/api/MedCore.Shared/MedCore.Shared.csproj", "services/api/MedCore.Shared/"]
RUN dotnet restore "services/api/MedCore.API/MedCore.API.csproj"

# Copy the rest of the source code and build
COPY services/api/ ./services/api/
WORKDIR "/src/services/api/MedCore.API"
RUN dotnet build "MedCore.API.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "MedCore.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Build the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY --from=publish /app/publish .

# Run as non-root user for security
RUN adduser --disabled-password \
  --home /app \
  --gecos '' dotnetuser && chown -R dotnetuser /app
USER dotnetuser

ENTRYPOINT ["dotnet", "MedCore.API.dll"]
