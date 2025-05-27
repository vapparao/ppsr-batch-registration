FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Create all directories first
WORKDIR /src
RUN mkdir -p Api Application Infrastructure Domain

# Copy solution file
COPY ./*.sln ./

# Copy project files using absolute certainty
COPY ./Api/*.csproj ./Api/
COPY ./Application/*.csproj ./Application/
COPY ./Infrastructure/*.csproj ./Infrastructure/
COPY ./Domain/*.csproj ./Domain/

# Verify files were copied (debugging)
RUN ls -laR /src

# Restore NuGet packages
RUN dotnet restore "Api/Api.csproj"

# Copy remaining source code
COPY . .

# Build the project
WORKDIR "/src/Api"
RUN dotnet build "Api.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "Api.csproj" -c Release -o /app/publish

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Api.dll"]