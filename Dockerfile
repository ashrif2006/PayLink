
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

# Copy project file
COPY ["src/PayLink.Api/PayLink.Api.csproj", "src/PayLink.Api/"]

# Restore dependencies
RUN dotnet restore "src/PayLink.Api/PayLink.Api.csproj"

# Copy source code
COPY . .

# Build and publish
RUN dotnet publish "src/PayLink.Api/PayLink.Api.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false


# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_HTTP_PORTS=8080

ENTRYPOINT ["dotnet", "PayLink.Api.dll"]
