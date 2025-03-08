# .NET Runtime (Çalışma Zamanı)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build Aşaması
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Calia.Services.AuthAPI/Calia.Services.AuthAPI.csproj", "Calia.Services.AuthAPI/"]
RUN dotnet restore "./Calia.Services.AuthAPI/Calia.Services.AuthAPI.csproj"
COPY . .
WORKDIR "/src/Calia.Services.AuthAPI"
RUN dotnet build "./Calia.Services.AuthAPI.csproj" -c Release -o /app/build

# Publish Aşaması
FROM build AS publish
RUN dotnet publish "./Calia.Services.AuthAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final Runtime
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Calia.Services.AuthAPI.dll"]
#digerlerinden farkli olan bu 