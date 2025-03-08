#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Calia.Services.ProductAPI/Calia.Services.ProductAPI.csproj", "Calia.Services.ProductAPI/"]
RUN dotnet restore "./Calia.Services.ProductAPI/Calia.Services.ProductAPI.csproj"
COPY . .
WORKDIR "/src/Calia.Services.ProductAPI"
RUN dotnet build "./Calia.Services.ProductAPI.csproj" -c Release -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Calia.Services.ProductAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Calia.Services.ProductAPI.dll"]