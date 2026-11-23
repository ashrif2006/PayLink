# syntax=docker/dockerfile:1

# ---- build ----
FROM mcr.microsoft.com/dotnet/sdk:11.0 AS build
WORKDIR /src

# Copy only the project file first so restore is cached between builds.
COPY ["src/PayLink.Api/PayLink.Api.csproj", "src/PayLink.Api/"]
RUN dotnet restore "src/PayLink.Api/PayLink.Api.csproj"

COPY ["src/PayLink.Api/", "src/PayLink.Api/"]
RUN dotnet publish "src/PayLink.Api/PayLink.Api.csproj" \
    --configuration Release \
    --no-restore \
    --output /app/publish \
    /p:UseAppHost=false

# ---- runtime ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080 \
    DOTNET_RUNNING_IN_CONTAINER=true
EXPOSE 8080
COPY --from=build /app/publish .
USER app
ENTRYPOINT ["dotnet", "PayLink.Api.dll"]
