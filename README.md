# DynamicConfigApp

A microservices-based dynamic configuration management system built with .NET 8, featuring real-time configuration updates across multiple services.

## 🏗️ Architecture

This project consists of multiple services that work together to provide dynamic configuration management:

### Services

- **DynamicConfig** - Core configuration library (class library)
- **DynamicConfig.Web** - Web UI components library (Razor class library)
- **ServiceA.Api** - Microservice A that consumes dynamic configurations
- **ServiceB.Api** - Microservice B that consumes dynamic configurations
- **PostgreSQL** - Database for storing configuration data

### Project Structure

```
DynamicConfigApp/
├── DynamicConfig/                 # Main web application
│   ├── Models/                    # Data models
│   ├── Services/                  # Business logic services
│   ├── Repositories/              # Data access layer
│   ├── Migrations/                # Entity Framework migrations
│   └── Dockerfile                 # Container configuration
├── DynamicConfig.Web/             # Web UI components
├── ServiceA.Api/                  # Microservice A
│   └── Dockerfile                 # Container configuration
├── ServiceB.Api/                  # Microservice B
│   └── Dockerfile                 # Container configuration
├── docker-compose.yml             # Multi-container orchestration
└── README.md                      # This file
```

## 🚀 Quick Start

### Prerequisites

- Docker Desktop
- .NET 8 SDK (for local development)

### Running with Docker Compose

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd DynamicConfigApp
   ```

2. **Start all services**
   ```bash
   docker-compose up --build
   ```

3. **Initial Setup**
   - PostgreSQL automatically creates Configs table and initializes with default data
   - ServiceA and ServiceB automatically run migrations if database doesn't exist
   - Default configurations are available for testing

4. **Access the services**
   - **ServiceA API**: http://localhost:5000
   - **ServiceB API**: http://localhost:5001
   - **PostgreSQL**: localhost:5432

### Default Configuration Data

The system comes with pre-configured test data:

| ID | Name | Type | Value | Active | Application |
|----|------|------|-------|--------|-------------|
| 1 | SiteName | string | soty.io | ✅ | SERVICE-A |
| 2 | IsBasketEnabled | bool | 1 | ✅ | SERVICE-B |
| 3 | MaxItemCount | int | 50 | ❌ | SERVICE-A |

**Test the configuration access:**
```bash
# Get SiteName for SERVICE-A
curl http://localhost:5000/cfg/SiteName

# Get IsBasketEnabled for SERVICE-B  
curl http://localhost:5001/cfg/IsBasketEnabled

# Get MaxItemCount for SERVICE-A
curl http://localhost:5000/cfg/MaxItemCount
```

### Environment Variables

The services are configured with the following environment variables:

```yaml
ASPNETCORE_ENVIRONMENT: "Development"
ConnectionStrings__DefaultConnection: "Host=postgres;Port=5432;Database=postgres;Username=postgres;Password=db1234"
DynamicConfig__ApplicationName: "SERVICE-A|SERVICE-B"
DynamicConfig__RefreshTimerIntervalInMs: "10000"
```

## 🔧 Configuration

### DynamicConfig Settings

Each service can be configured with:

- **ApplicationName**: Unique identifier for the service
- **RefreshTimerIntervalInMs**: How often to check for configuration updates (in milliseconds)

### Database Configuration

- **Database**: PostgreSQL 16
- **Host**: postgres (container name)
- **Port**: 5432
- **Database**: postgres
- **Username**: postgres
- **Password**: db1234

## 📊 API Endpoints

### ServiceA.Api (Port 5000)
- **Configuration Access**: `GET /cfg/{key}` - Get configuration value by key
- **Example**: `GET /cfg/test1` - Returns configuration value for "test1"
- **Response**: `{ "key": "test1", "value": "config_value", "runtimeType": "String" }`
- **Configuration Management**: Full CRUD operations and Web UI

### ServiceB.Api (Port 5001)
- **Configuration Access**: `GET /cfg/{key}` - Get configuration value by key
- **Example**: `GET /cfg/test1` - Returns configuration value for "test1"
- **Response**: `{ "key": "test1", "value": "config_value", "runtimeType": "String" }`
- **Configuration Management**: Full CRUD operations and Web UI

### ServiceA.Api (Port 5000) - Configuration Management API

#### CRUD Operations for Configuration Management

**1. Get All Configurations**
```http
GET /api/configs?app=SERVICE-A
GET /api/configs?app=SERVICE-B
```

**2. Get Configuration by ID**
```http
GET /api/configs/{id}?app=SERVICE-A
GET /api/configs/{id}?app=SERVICE-B
```

**3. Create New Configuration**
```http
POST /api/configs?app=SERVICE-A
Content-Type: application/json

{
  "name": "test1",
  "type": "String",
  "value": "Hello World",
  "isActive": true
}
```

**4. Update Configuration**
```http
PUT /api/configs/{id}?app=SERVICE-A
Content-Type: application/json

{
  "name": "test1",
  "type": "String", 
  "value": "Updated Value",
  "isActive": true
}
```

**5. Delete Configuration**
```http
DELETE /api/configs/{id}?app=SERVICE-A
```

#### Configuration Access Endpoints

**Get Configuration Value**
```http
GET /cfg/{key}
```

**Examples:**
- `GET /cfg/test1` - Get value for configuration key "test1"
- `GET /cfg/database_url` - Get database URL configuration
- `GET /cfg/max_connections` - Get max connections setting

**Response Format:**
```json
{
  "key": "test1",
  "value": "Hello World",
  "runtimeType": "String"
}
```

#### Web UI for Configuration Management
- **URL**: `http://localhost:5000/dynamic-config/SERVICE-A`
- **URL**: `http://localhost:5001/dynamic-config/SERVICE-B`
- **Features**: Create, Update, Delete configurations through web interface


## 🐳 Docker Configuration

### Build Contexts

All services use the root directory as build context to access shared projects:

```yaml
build:
  context: .
  dockerfile: ServiceA.Api/Dockerfile
```

### Multi-stage Builds

Each service uses multi-stage Docker builds:
1. **Build stage**: .NET 8 SDK for compilation
2. **Runtime stage**: .NET 8 ASP.NET runtime for execution

### Project Dependencies

Services reference shared projects:
- `DynamicConfig` - Core configuration logic
- `DynamicConfig.Web` - Web UI components

## 🛠️ Development

### Local Development

1. **Start PostgreSQL**
   ```bash
   docker-compose up postgres
   ```

2. **Run services individually**
   ```bash
   cd DynamicConfig
   dotnet run
   
   cd ../ServiceA.Api
   dotnet run
   
   cd ../ServiceB.Api
   dotnet run
   ```

### Database Migrations

```bash
cd DynamicConfig
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Building Individual Services

```bash
# Build ServiceA
docker build -f ServiceA.Api/Dockerfile -t servicea-api .

# Build ServiceB
docker build -f ServiceB.Api/Dockerfile -t serviceb-api .

# Build DynamicConfig
docker build -f DynamicConfig/Dockerfile -t dynamicconfig .
```

## 📝 Features

- **Real-time Configuration Updates**: Services automatically refresh configurations
- **Centralized Management**: Single web interface for all configuration management
- **Microservices Architecture**: Independent services with shared configuration
- **Containerized Deployment**: Full Docker support for easy deployment
- **Database Persistence**: PostgreSQL for reliable configuration storage

## 🔍 Troubleshooting

### Common Issues

1. **Port Conflicts**
   - Ensure ports 5000, 5001, 5002, and 5432 are available
   - Modify ports in `docker-compose.yml` if needed

2. **Database Connection Issues**
   - Verify PostgreSQL container is running
   - Check connection string in environment variables

3. **Build Failures**
   - Ensure all project files are present
   - Check Docker build context and file paths

### Logs

View service logs:
```bash
docker-compose logs servicea.api
docker-compose logs serviceb.api
docker-compose logs dynamicconfig
```

## 📄 License

This project is licensed under the MIT License.

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## Support

For support and questions, please open an issue in the repository. 