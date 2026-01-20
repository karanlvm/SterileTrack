# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["SterileTrack.sln", "./"]
COPY ["src/SterileTrack.Domain/SterileTrack.Domain.csproj", "src/SterileTrack.Domain/"]
COPY ["src/SterileTrack.Application/SterileTrack.Application.csproj", "src/SterileTrack.Application/"]
COPY ["src/SterileTrack.Infrastructure/SterileTrack.Infrastructure.csproj", "src/SterileTrack.Infrastructure/"]
COPY ["src/SterileTrack.API/SterileTrack.API.csproj", "src/SterileTrack.API/"]

# Restore dependencies
RUN dotnet restore "SterileTrack.sln"

# Copy everything else and build
COPY . .
WORKDIR "/src/src/SterileTrack.API"
RUN dotnet build "SterileTrack.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "SterileTrack.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SterileTrack.API.dll"]
