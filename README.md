# Umbraco CMS Web Application

A modern Umbraco CMS application with Docker Compose, SQL Server, and devcontainer support for consistent development environments.

## 🚀 Quick Start

### Prerequisites

- [Docker](https://docs.docker.com/get-docker/) and [Docker Compose](https://docs.docker.com/compose/install/)
- [VS Code](https://code.visualstudio.com/) with [Dev Containers extension](https://marketplace.visualstudio.com/items?itemName=ms-vscode-remote.remote-containers) (recommended)
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) (if running locally)

### Development with Dev Container (Recommended)

1. Clone the repository:
   ```bash
   git clone https://github.com/markcoleman/web.git
   cd web
   ```

2. Copy environment variables:
   ```bash
   cp .env.example .env
   ```

3. Open in VS Code and reopen in container:
   ```bash
   code .
   ```
   Then press `Ctrl+Shift+P`, select "Dev Containers: Reopen in Container"

4. The application will be available at:
   - HTTP: http://localhost:5000
   - HTTPS: https://localhost:5001
   - Umbraco BackOffice: http://localhost:5000/umbraco

### Local Development with Docker Compose

1. Clone and setup:
   ```bash
   git clone https://github.com/markcoleman/web.git
   cd web
   cp .env.example .env
   ```

2. Start services:
   ```bash
   docker-compose up -d
   ```

3. View logs:
   ```bash
   docker-compose logs -f web
   ```

### Local Development without Docker

1. Setup SQL Server (Docker):
   ```bash
   docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=MyStrongPassword123!" \
     -p 1433:1433 --name sql-server \
     -d mcr.microsoft.com/mssql/server:2022-latest
   ```

2. Run the application:
   ```bash
   cd src/UmbracoWeb/UmbracoWeb
   dotnet run
   ```

## 🏗️ Architecture

### Stack
- **CMS**: Umbraco 15.0
- **Runtime**: .NET 9.0
- **Database**: Microsoft SQL Server 2022
- **Logging**: Serilog (Console + File)
- **Container**: Docker with multi-stage build
- **Development**: VS Code Dev Containers

### Project Structure
```
├── .devcontainer/              # VS Code dev container configuration
├── .github/                    # GitHub Actions workflows & Dependabot
├── src/UmbracoWeb/            # Main application source
│   ├── UmbracoWeb/            # Umbraco CMS project
│   └── Dockerfile             # Production container image
├── logs/                      # Application logs (gitignored)
├── docker-compose.yml         # Multi-service development stack
├── .env.example              # Environment variables template
└── README.md                 # This file
```

## ⚙️ Configuration

### Environment Variables

Copy `.env.example` to `.env` and configure:

| Variable | Description | Default |
|----------|-------------|---------|
| `SA_PASSWORD` | SQL Server SA password | `MyStrongPassword123!` |
| `UMBRACO_GLOBAL_ID` | Unique site identifier | `local-dev-site` |
| `UMBRACO_BACKOFFICE_HOST` | BackOffice host URL | `localhost:5000` |
| `CONNECTION_STRING` | Database connection | Auto-configured |
| `ASPNETCORE_ENVIRONMENT` | Application environment | `Development` |

### Configuration Files

- `appsettings.json` - Production configuration with placeholders
- `appsettings.Development.json` - Development-specific settings
- `docker-compose.yml` - Development services orchestration
- `.devcontainer/devcontainer.json` - VS Code dev container settings

## 🔧 Development

### Building

```bash
# Restore packages
dotnet restore src/UmbracoWeb/UmbracoWeb/UmbracoWeb.csproj

# Build application
dotnet build src/UmbracoWeb/UmbracoWeb/UmbracoWeb.csproj --configuration Release

# Run locally
dotnet run --project src/UmbracoWeb/UmbracoWeb/UmbracoWeb.csproj
```

### Docker Commands

```bash
# Build and start all services
docker-compose up -d

# View application logs
docker-compose logs -f web

# View database logs
docker-compose logs -f mssql

# Stop all services
docker-compose down

# Rebuild services
docker-compose up -d --build

# Clean up volumes (⚠️ deletes database data)
docker-compose down -v
```

### Database Management

The SQL Server database persists data in a Docker volume. To reset:

```bash
# Stop services and remove volumes
docker-compose down -v

# Start fresh
docker-compose up -d
```

## 🚀 Deployment

### GitHub Actions CI/CD

The repository includes automated workflows:

- **CI**: Build, test, and security scanning on every push/PR
- **CD**: Docker image build and push to GitHub Container Registry
- **Dependabot**: Automated dependency updates

### Production Environment Variables

For production deployment, configure these environment variables:

```bash
# Database
CONNECTION_STRING="Server=your-sql-server;Database=UmbracoWeb;User Id=user;Password=password;TrustServerCertificate=true"

# Umbraco
UMBRACO_GLOBAL_ID="your-production-site-id"
UMBRACO_BACKOFFICE_HOST="your-domain.com"

# Environment
ASPNETCORE_ENVIRONMENT="Production"
```

### Docker Production Image

```bash
# Build production image
docker build -f src/UmbracoWeb/Dockerfile -t umbraco-web .

# Run production container
docker run -d \
  -p 80:8080 \
  -e CONNECTION_STRING="your-connection-string" \
  -e UMBRACO_GLOBAL_ID="your-site-id" \
  -e UMBRACO_BACKOFFICE_HOST="your-domain.com" \
  --name umbraco-web \
  umbraco-web
```

## 📝 Logging

Logs are configured with Serilog and output to:
- **Console**: Structured JSON for container logs
- **Files**: `logs/log-YYYY-MM-DD.txt` (daily rolling)

Log levels:
- **Development**: Debug level
- **Production**: Information level

## 🔒 Security

### Best Practices Implemented

- ✅ Non-root container user
- ✅ Environment variable externalization
- ✅ Secrets excluded from source control
- ✅ SQL Server with strong passwords
- ✅ HTTPS support configured
- ✅ Security scanning in CI/CD
- ✅ Dependabot for vulnerability management

### Security Considerations

- Change default SQL Server password in production
- Use proper SSL certificates for HTTPS
- Configure Umbraco security settings for production
- Regularly update dependencies via Dependabot

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/amazing-feature`
3. Make changes and test locally
4. Commit changes: `git commit -m 'Add amazing feature'`
5. Push to branch: `git push origin feature/amazing-feature`
6. Open a Pull Request

## 📋 Troubleshooting

### Common Issues

**Database Connection Failed**
```bash
# Check SQL Server container
docker-compose logs mssql

# Verify connection string in logs
docker-compose logs web | grep "Connection"
```

**Umbraco Installation Issues**
- Navigate to `/umbraco` to access the installation wizard
- Use the connection string from `appsettings.Development.json`
- Default admin credentials will be set during installation

**Port Conflicts**
```bash
# Check if ports are in use
netstat -tulpn | grep :5000
netstat -tulpn | grep :1433

# Modify ports in docker-compose.yml if needed
```

**Dev Container Issues**
- Ensure Docker is running
- Rebuild container: `Ctrl+Shift+P` → "Dev Containers: Rebuild Container"
- Check `.devcontainer/devcontainer.json` configuration

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🔗 Links

- [Umbraco Documentation](https://docs.umbraco.com/)
- [.NET 9 Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [Docker Documentation](https://docs.docker.com/)
- [VS Code Dev Containers](https://code.visualstudio.com/docs/remote/containers)