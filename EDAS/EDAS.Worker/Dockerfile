# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Docker
WORKDIR /src
COPY ["EDAS.Worker/EDAS.Worker.csproj", "EDAS.Worker/"]
COPY ["EDAS.BacktrackingCombinatronics/EDAS.BacktrackingCombinatronics.csproj", "EDAS.BacktrackingCombinatronics/"]
COPY ["EDAS.Common/EDAS.Common.csproj", "EDAS.Common/"]
COPY ["EDAS.Sorting/EDAS.Sorting.csproj", "EDAS.Sorting/"]
RUN dotnet restore "./EDAS.Worker/EDAS.Worker.csproj"
COPY . .
WORKDIR "/src/EDAS.Worker"
RUN dotnet build "./EDAS.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Docker
RUN dotnet publish "./EDAS.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EDAS.Worker.dll"]