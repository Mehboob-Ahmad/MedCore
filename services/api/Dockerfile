# Use the official .NET 9 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and restore dependencies
COPY ["MedicHp.API/MedicHp.API.csproj", "MedicHp.API/"]
COPY ["MedicHp.Application/MedicHp.Application.csproj", "MedicHp.Application/"]
COPY ["MedicHp.Domain/MedicHp.Domain.csproj", "MedicHp.Domain/"]
COPY ["MedicHp.Infrastructure/MedicHp.Infrastructure.csproj", "MedicHp.Infrastructure/"]
COPY ["MedicHp.Persistence/MedicHp.Persistence.csproj", "MedicHp.Persistence/"]
COPY ["MedicHp.Shared/MedicHp.Shared.csproj", "MedicHp.Shared/"]
RUN dotnet restore "MedicHp.API/MedicHp.API.csproj"

# Copy the rest of the source code and build
COPY . .
WORKDIR "/src/MedicHp.API"
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
