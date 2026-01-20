# SterileTrack - Device Inventory Tracking System

SterileTrack is a full-stack internal web application built with **C# and ASP.NET Core** to track device availability and sterilization status. The system provides RESTful APIs with backend validation to enforce valid device state transitions, a relational **MySQL** database schema to persist inventory data and status change history, and a **React** dashboard for viewing and updating device status.

## Features

- **Device Inventory Management**: Track medical devices with unique identifiers
- **Status Tracking**: Monitor device status (Available, In Use, Pending Sterilization, Retired)
- **Status History**: Maintain immutable history of all status changes
- **State Transition Validation**: Backend validation ensures only valid state transitions
- **Sterilization Cycle Tracking**: Record and track sterilization cycles for reusable devices
- **React Dashboard**: Modern, responsive interface for viewing and updating device status

## Technology Stack

**Backend:**
- .NET 8.0 (C#)
- ASP.NET Core Web API
- Entity Framework Core with MySQL (Pomelo provider)
- Clean Architecture (Domain, Application, Infrastructure, API layers)

**Frontend:**
- React 18
- TypeScript
- Vite
- React Router
- Axios

**Database:**
- MySQL 8.0

**Infrastructure:**
- Docker & Docker Compose

## Architecture

```
SterileTrack/
├── src/
│   ├── SterileTrack.Domain/          # Domain entities and interfaces
│   ├── SterileTrack.Application/     # Application services and DTOs
│   ├── SterileTrack.Infrastructure/  # Data access (EF Core + MySQL)
│   └── SterileTrack.API/             # Web API controllers
├── frontend/                          # React + TypeScript frontend
└── docker-compose.yml                 # Docker orchestration
```

## Getting Started

### Prerequisites

- .NET 8.0 SDK
- Node.js 18+ and npm
- Docker Desktop (for containerized deployment)

### Option 1: Docker Compose (Recommended)

1. **Clone the repository:**
   ```bash
   git clone <repository-url>
   cd SurgiFlow
   ```

2. **Start the services:**
   ```bash
   docker-compose up -d
   ```

   This will start:
   - MySQL database on port 3306
   - API on port 5000

3. **Access the application:**
   - API: http://localhost:5000
   - Swagger UI: http://localhost:5000/swagger
   - Frontend: http://localhost:3000 (after starting frontend)

### Option 2: Local Development

#### Backend Setup

1. **Ensure MySQL is running** and create a database:
   ```sql
   CREATE DATABASE SterileTrack;
   ```

2. **Update connection string** in `src/SterileTrack.API/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=SterileTrack;User=root;Password=yourpassword;Port=3306;"
     }
   }
   ```

3. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

4. **Run the API:**
   ```bash
   cd src/SterileTrack.API
   dotnet run
   ```

   The API will be available at:
   - HTTP: http://localhost:5000
   - Swagger: http://localhost:5000/swagger

#### Frontend Setup

1. **Navigate to frontend directory:**
   ```bash
   cd frontend
   ```

2. **Install dependencies:**
   ```bash
   npm install
   ```

3. **Start development server:**
   ```bash
   npm run dev
   ```

   The frontend will be available at http://localhost:3000

## API Endpoints

### Devices

- `GET /api/devices` - Get all devices
- `GET /api/devices/{id}` - Get device by ID
- `GET /api/devices/identifier/{deviceIdentifier}` - Get device by identifier
- `GET /api/devices/status/{status}` - Get devices by status
- `POST /api/devices` - Create new device
- `PUT /api/devices/{id}/status` - Update device status
- `GET /api/devices/{id}/history` - Get device status history

### Sterilization Cycles

- `GET /api/sterilizationcycles/{id}` - Get cycle by ID
- `GET /api/sterilizationcycles/device/{deviceId}` - Get cycles for device
- `POST /api/sterilizationcycles` - Create new cycle
- `POST /api/sterilizationcycles/{id}/complete` - Complete cycle

## Device Status Flow

```
Available → InUse → PendingSterilization → Available
     ↓
Retired (terminal state)
```

### Valid State Transitions

- **Available** → InUse, Retired
- **InUse** → PendingSterilization
- **PendingSterilization** → Available (after sterilization)
- **Retired** → (no transitions allowed)

All state transitions are validated at the backend level to ensure data integrity.

## Database Schema

### Key Entities

- **Device**: Represents a medical device with status tracking
- **StatusHistory**: Immutable record of all status changes
- **SterilizationCycle**: Records sterilization events for devices
- **User**: System users (for future authentication)

## Development Notes

### State Transition Validation

The system enforces valid state transitions at the service layer (`DeviceService.ValidateDeviceStatusTransitionAsync`). Invalid transitions will return a 400 Bad Request error.

### Status History

Every status change is automatically recorded in the `StatusHistory` table, providing a complete audit trail of device state changes.

### Sterilization Cycles

When a sterilization cycle is completed, the device status is automatically updated from `PendingSterilization` to `Available`, and the status change is recorded in the history.

## Docker Configuration

The `docker-compose.yml` file includes:
- MySQL 8.0 container with persistent volume
- API container that waits for MySQL to be healthy before starting
- Automatic database creation on first run

## License

This project is for demonstration purposes only.

## Contact

For questions or issues, please refer to the project documentation or create an issue in the repository.
# SterileTrack
