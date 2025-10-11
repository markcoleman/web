# Testing Guide

This document provides comprehensive testing instructions for the Umbraco CMS setup.

## 🧪 Testing Checklist

### Prerequisites Verification

- [ ] Docker and Docker Compose installed
- [ ] .NET 9 SDK installed (if testing locally)
- [ ] VS Code with Dev Containers extension (if testing devcontainer)
- [ ] Git repository cloned

### Local .NET Build Testing

```bash
# Navigate to project
cd src/UmbracoWeb/UmbracoWeb

# Test restore
dotnet restore

# Test build
dotnet build --configuration Release

# Test publish
dotnet publish --configuration Release --output /tmp/test-publish

# Verify published files
ls -la /tmp/test-publish/
```

**✅ Expected Result**: Build succeeds without errors.

### Docker Compose Configuration Testing

```bash
# Test configuration syntax
docker compose config

# Test service definition parsing
docker compose config --services

# Test volume and network configuration
docker compose config --volumes
docker compose config --profiles
```

**✅ Expected Result**: Configuration is valid, services are properly defined.

### Environment Variables Testing

```bash
# Copy environment template
cp .env.example .env

# Edit .env file as needed
nano .env

# Test variable substitution
docker compose config | grep -E "(SA_PASSWORD|UMBRACO|CONNECTION_STRING)"
```

**✅ Expected Result**: Environment variables are properly substituted in the configuration.

### Development Container Testing

```bash
# Open in VS Code
code .

# In VS Code Command Palette (Ctrl+Shift+P):
# > Dev Containers: Reopen in Container
```

**✅ Expected Result**: Container builds successfully, VS Code connects, and development tools are available.

### Full Stack Integration Testing

```bash
# Start all services
docker compose up -d

# Check service status
docker compose ps

# View logs
docker compose logs web
docker compose logs mssql

# Test HTTP endpoint
curl -I http://localhost:5000

# Test database connectivity
docker compose exec mssql /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "MyStrongPassword123!" -C -Q "SELECT 1"
```

**✅ Expected Result**: All services start successfully, web responds on port 5000, database is accessible.

### GitHub Actions Testing

The CI/CD pipeline will test:

- [ ] .NET build and test
- [ ] Docker image build
- [ ] Security scanning
- [ ] Dependency checks

**✅ Expected Result**: All CI checks pass on push/PR.

## 🚀 Manual Verification Steps

### 1. Umbraco Installation

1. Start services: `docker compose up -d`
2. Navigate to: http://localhost:5000/umbraco
3. Complete Umbraco installation wizard
4. Create admin user
5. Verify backoffice access

### 2. Database Persistence

1. Create content in Umbraco
2. Stop services: `docker compose down`
3. Start services: `docker compose up -d`
4. Verify content persists

### 3. Logging Verification

1. Check logs directory: `ls -la logs/`
2. Verify log files are created
3. Check log content: `tail -f logs/log-*.txt`

### 4. Development Workflow

1. Make code changes
2. Build/test locally
3. Test in Docker
4. Commit and push
5. Verify CI/CD pipeline

## 🐛 Troubleshooting Tests

### Common Issues and Solutions

**Port Conflicts**
```bash
# Check port usage
netstat -tulpn | grep -E ":(5000|5001|1433)"

# Stop conflicting services
sudo systemctl stop mssql-server
```

**Database Connection Issues**
```bash
# Check SQL Server status
docker compose logs mssql

# Test connection manually
docker compose exec mssql /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "MyStrongPassword123!" -C -Q "SELECT 1"
```

**Build Failures**
```bash
# Clear Docker build cache
docker builder prune

# Rebuild with no cache
docker compose build --no-cache

# Check for .dockerignore issues
cat .dockerignore
```

**Dev Container Issues**
```bash
# Rebuild container
# VS Code: Ctrl+Shift+P > Dev Containers: Rebuild Container

# Check container logs
docker logs <container_name>
```

## 📊 Performance Testing

### Resource Usage

```bash
# Monitor resource usage
docker stats

# Check disk usage
docker system df
docker compose exec web df -h
```

### Load Testing (Optional)

```bash
# Install hey load testing tool
go install github.com/rakyll/hey@latest

# Basic load test
hey -n 100 -c 10 http://localhost:5000

# Load test with authentication
hey -n 50 -c 5 -H "Authorization: Bearer <token>" http://localhost:5000/umbraco
```

## ✅ Test Results Documentation

### Build Test Results

- [ ] .NET build: ✅ PASS / ❌ FAIL
- [ ] NuGet restore: ✅ PASS / ❌ FAIL
- [ ] Publish: ✅ PASS / ❌ FAIL
- [ ] Docker config: ✅ PASS / ❌ FAIL

### Integration Test Results

- [ ] Docker Compose up: ✅ PASS / ❌ FAIL
- [ ] Web service health: ✅ PASS / ❌ FAIL
- [ ] Database health: ✅ PASS / ❌ FAIL
- [ ] Umbraco installation: ✅ PASS / ❌ FAIL

### Development Test Results

- [ ] Dev container build: ✅ PASS / ❌ FAIL
- [ ] VS Code integration: ✅ PASS / ❌ FAIL
- [ ] Debugging: ✅ PASS / ❌ FAIL
- [ ] Hot reload: ✅ PASS / ❌ FAIL

### CI/CD Test Results

- [ ] GitHub Actions trigger: ✅ PASS / ❌ FAIL
- [ ] Build workflow: ✅ PASS / ❌ FAIL
- [ ] Security scan: ✅ PASS / ❌ FAIL
- [ ] Docker push: ✅ PASS / ❌ FAIL

## 📝 Test Environment Details

- **OS**: Ubuntu 20.04 LTS
- **.NET Version**: 9.0.x
- **Docker Version**: 24.0.x
- **Docker Compose Version**: v2.21.x
- **Umbraco Version**: 16.2.0
- **SQL Server Version**: 2022-latest

## 🔄 Automated Testing

For automated testing in CI/CD, see `.github/workflows/ci-cd.yml`:

- Unit tests (if added)
- Integration tests
- Security scans
- Docker build tests
- Dependency vulnerability checks

## 📋 Acceptance Criteria

The setup is considered successful when:

- [x] Project builds without errors
- [x] Docker Compose services start successfully
- [x] Umbraco installation completes
- [x] Database persists data across restarts
- [x] Dev container works in VS Code
- [x] CI/CD pipeline passes all checks
- [x] Documentation is comprehensive
- [x] Security best practices are followed