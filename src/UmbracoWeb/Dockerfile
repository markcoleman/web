# Use the official .NET SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /app

# Copy project files and restore dependencies
COPY UmbracoWeb/*.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY UmbracoWeb/. ./
RUN dotnet publish -c Release -o out

# Use the official ASP.NET Core runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Create logs directory
RUN mkdir -p /app/logs

# Copy the published application
COPY --from=build-env /app/out .

# Create a non-root user
RUN groupadd -r umbraco && useradd -r -g umbraco umbraco
RUN chown -R umbraco:umbraco /app
USER umbraco

# Expose ports
EXPOSE 8080 8081

# Set the entry point
ENTRYPOINT ["dotnet", "UmbracoWeb.dll"]