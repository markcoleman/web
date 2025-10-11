# Contributing to Umbraco Web

Thank you for your interest in contributing! This document provides guidelines and information for contributors.

## 🚀 Getting Started

### Development Setup

1. **Fork and Clone**
   ```bash
   git clone https://github.com/your-username/web.git
   cd web
   ```

2. **Environment Setup**
   ```bash
   cp .env.example .env
   # Edit .env with your settings
   ```

3. **Start Development Environment**
   - **Option A: Dev Container (Recommended)**
     - Open in VS Code
     - Reopen in container when prompted
   - **Option B: Docker Compose**
     ```bash
     docker compose up -d
     ```
   - **Option C: Local Development**
     ```bash
     # Start SQL Server
     docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=MyStrongPassword123!" -p 1433:1433 --name sql-server -d mcr.microsoft.com/mssql/server:2022-latest
     
     # Run application
     dotnet run --project src/UmbracoWeb/UmbracoWeb/UmbracoWeb.csproj
     ```

## 📋 Development Guidelines

### Code Style

- Follow [Microsoft C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- Use meaningful variable and method names
- Add XML documentation for public APIs
- Keep methods focused and small

### Commit Messages

Use conventional commit format:
```
type(scope): description

Examples:
feat(cms): add new content type for blog posts
fix(docker): resolve database connection timeout
docs(readme): update installation instructions
chore(deps): update Umbraco to latest version
```

Types:
- `feat`: New features
- `fix`: Bug fixes
- `docs`: Documentation changes
- `chore`: Maintenance tasks
- `refactor`: Code refactoring
- `test`: Adding or updating tests
- `ci`: CI/CD changes

### Branch Naming

- `feature/description-of-feature`
- `fix/description-of-fix`
- `docs/description-of-update`
- `chore/description-of-task`

## 🧪 Testing

### Running Tests

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Writing Tests

- Place tests in corresponding test projects
- Use descriptive test method names
- Follow Arrange-Act-Assert pattern
- Mock external dependencies

Example:
```csharp
[Test]
public void GetContentById_WithValidId_ReturnsContent()
{
    // Arrange
    var contentId = 123;
    var expectedContent = new Content { Id = contentId };
    
    // Act
    var result = contentService.GetById(contentId);
    
    // Assert
    Assert.That(result, Is.EqualTo(expectedContent));
}
```

## 🏗️ Architecture Guidelines

### Project Structure

- Keep controllers thin, move logic to services
- Use dependency injection for all dependencies
- Implement proper error handling and logging
- Follow SOLID principles

### Database Changes

- Use Umbraco migrations for schema changes
- Document any manual database updates
- Test migrations in development environment first

### Docker Best Practices

- Multi-stage builds for production images
- Non-root users in containers
- Proper health checks
- Minimal image layers

## 📝 Documentation

### Required Documentation

- Update README.md for significant changes
- Add XML documentation for public APIs
- Document configuration changes
- Update deployment guides if needed

### Documentation Style

- Use clear, concise language
- Include code examples
- Provide step-by-step instructions
- Keep documentation up-to-date

## 🔍 Pull Request Process

### Before Submitting

1. **Test Your Changes**
   ```bash
   # Build and test
   dotnet build --configuration Release
   dotnet test
   
   # Test Docker build
   docker compose up -d --build
   ```

2. **Update Documentation**
   - Update README.md if needed
   - Add/update code comments
   - Update API documentation

3. **Check Code Quality**
   - Run linters
   - Fix any warnings
   - Ensure consistent formatting

### PR Checklist

- [ ] Branch is up-to-date with main
- [ ] All tests pass
- [ ] Documentation updated
- [ ] Docker build succeeds
- [ ] No merge conflicts
- [ ] Descriptive PR title and description
- [ ] Screenshots for UI changes

### PR Template

```markdown
## Description
Brief description of changes

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Breaking change
- [ ] Documentation update

## Testing
- [ ] Local testing completed
- [ ] Docker testing completed
- [ ] All existing tests pass

## Screenshots (if applicable)
Add screenshots for UI changes

## Additional Notes
Any additional information or context
```

## 🐛 Bug Reports

### Before Reporting

1. Check existing issues
2. Reproduce in clean environment
3. Test with latest version

### Bug Report Template

```markdown
## Bug Description
Clear description of the bug

## Steps to Reproduce
1. Step 1
2. Step 2
3. Step 3

## Expected Behavior
What should happen

## Actual Behavior
What actually happens

## Environment
- OS: [e.g., Windows 11, Ubuntu 20.04]
- .NET Version: [e.g., 9.0]
- Umbraco Version: [e.g., 16.2.0]
- Docker Version: [if applicable]

## Additional Context
Any other relevant information
```

## ✨ Feature Requests

### Feature Request Template

```markdown
## Feature Description
Clear description of the proposed feature

## Problem Statement
What problem does this solve?

## Proposed Solution
How should this feature work?

## Alternatives Considered
Other solutions you've considered

## Additional Context
Any other relevant information
```

## 🤝 Community Guidelines

### Code of Conduct

- Be respectful and inclusive
- Welcome newcomers and help them learn
- Focus on constructive feedback
- Respect different opinions and approaches

### Communication

- Use clear, professional language
- Be patient with questions
- Provide helpful, detailed responses
- Follow up on discussions

## 📚 Resources

### Learning Resources

- [Umbraco Documentation](https://docs.umbraco.com/)
- [.NET Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [Docker Documentation](https://docs.docker.com/)
- [Git Best Practices](https://github.com/git-tips/tips)

### Development Tools

- [Visual Studio Code](https://code.visualstudio.com/)
- [Postman](https://www.postman.com/) for API testing
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [SQL Server Management Studio](https://docs.microsoft.com/en-us/sql/ssms/)

## ❓ Getting Help

- Check the [README.md](README.md) first
- Look through existing [Issues](https://github.com/markcoleman/web/issues)
- Create a new issue with detailed information
- Join community discussions

Thank you for contributing! 🎉