# Use aspnet for .NET runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

EXPOSE 5000

# Use the official .NET 8 SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory for the build process
WORKDIR /app

# Copy the .sln file and the project files into the container
COPY ["Rise.sln", "./"]
COPY ["Rise.Client/Rise.Client.csproj", "Rise.Client/"]
COPY ["Rise.Client.Tests/Rise.Client.Tests.csproj", "Rise.Client.Tests/"]
COPY ["Rise.Domain/Rise.Domain.csproj", "Rise.Domain/"]
COPY ["Rise.Domain.Tests/Rise.Domain.Tests.csproj", "Rise.Domain.Tests/"]
COPY ["Rise.Persistence/Rise.Persistence.csproj", "Rise.Persistence/"]
COPY ["Rise.PlaywrightTests/Rise.PlaywrightTests.csproj", "Rise.PlaywrightTests/"]
COPY ["Rise.Server/Rise.Server.csproj", "Rise.Server/"]
COPY ["Rise.Server.Tests/Rise.Server.Tests.csproj", "Rise.Server.Tests/"]
COPY ["Rise.Services/Rise.Services.csproj", "Rise.Services/"]
COPY ["Rise.Shared/Rise.Shared.csproj", "Rise.Shared/"]

# Restore as distinct layers
RUN dotnet restore "Rise.Client/Rise.Client.csproj" \
    && dotnet restore "Rise.Client.Tests/Rise.Client.Tests.csproj" \
    && dotnet restore "Rise.Domain/Rise.Domain.csproj" \
    && dotnet restore "Rise.Domain.Tests/Rise.Domain.Tests.csproj" \
    && dotnet restore "Rise.Persistence/Rise.Persistence.csproj" \
    && dotnet restore "Rise.PlaywrightTests/Rise.PlaywrightTests.csproj" \
    && dotnet restore "Rise.Server/Rise.Server.csproj" \
    && dotnet restore "Rise.Server.Tests/Rise.Server.Tests.csproj" \
    && dotnet restore "Rise.Services/Rise.Services.csproj" \
    && dotnet restore "Rise.Shared/Rise.Shared.csproj"


# Copy remaining files
COPY . .

# Change repository
WORKDIR /app/Rise.Server

# Build the application
RUN dotnet build "Rise.Server.csproj" -c Release -o /app/build

WORKDIR /app

# Install dotnet tools in build image
RUN dotnet tool install --global dotnet-ef

# Add tools path to environment
ENV PATH="\${PATH}:/root/.dotnet/tools"

# Apply database migrations
RUN dotnet-ef database update --startup-project Rise.Server --project Rise.Persistence

# Publish the application as user app
WORKDIR /app/Rise.Server
RUN dotnet publish "Rise.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app

# Copy the published files
COPY --from=build /app/publish .

# Add tools path to environment
ENV PATH="\${PATH}:/root/.dotnet/tools"
ENV ASPNETCORE_URLS=http://+:5000

# Start the application
ENTRYPOINT ["dotnet", "Rise.Server.dll"]