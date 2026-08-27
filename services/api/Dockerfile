# Use the official .NET 9 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and restore dependencies
COPY ["services/api/MedicHp.API/MedicHp.API.csproj", "services/api/MedicHp.API/"]
COPY ["services/api/MedicHp.Application/MedicHp.Application.csproj", "services/api/MedicHp.Application/"]
COPY ["services/api/MedicHp.Domain/MedicHp.Domain.csproj", "services/api/MedicHp.Domain/"]
COPY ["services/api/MedicHp.Infrastructure/MedicHp.Infrastructure.csproj", "services/api/MedicHp.Infrastructure/"]
COPY ["services/api/MedicHp.Persistence/MedicHp.Persistence.csproj", "services/api/MedicHp.Persistence/"]
COPY ["services/api/MedicHp.Shared/MedicHp.Shared.csproj", "services/api/MedicHp.Shared/"]
RUN dotnet restore "services/api/MedicHp.API/MedicHp.API.csproj"

# Copy the rest of the source code and build
COPY services/api/ ./services/api/
WORKDIR "/src/services/api/MedicHp.API"
RUN dotnet build "MedicHp.API.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "MedicHp.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

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

ENTRYPOINT ["dotnet", "MedicHp.API.dll"]
