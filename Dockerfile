# Etapa de construcción
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar csproj y restaurar dependencias para optimizar caché
COPY ["Clima_API/Clima_API.csproj", "Clima_API/"]
RUN dotnet restore "Clima_API/Clima_API.csproj"

# Copiar el resto del código
COPY . .
WORKDIR "/src/Clima_API"
RUN dotnet publish "Clima_API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa final
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Configuración para Render
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Clima_API.dll"]