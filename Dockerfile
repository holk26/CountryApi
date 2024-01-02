# Establecer la imagen base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Establecer la imagen para compilar la aplicación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar el archivo .csproj y restaurar las dependencias
COPY ["CountryApi.csproj", "."]
RUN dotnet restore

# Copiar el resto de la aplicación y compilar
COPY . .
WORKDIR "/src/."
RUN dotnet build -c Release -o /app/build

# Instalar herramienta EF y agregar paquete Microsoft.EntityFrameworkCore.Design
RUN dotnet tool install --global dotnet-ef
RUN dotnet add package Microsoft.EntityFrameworkCore.Design

# Generar las migraciones y actualizar la base de datos
RUN dotnet ef migrations add InitialCreate --output-dir /Migrations
RUN dotnet ef database update

# Publicar la aplicación
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# Configurar la imagen final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CountryApi.dll"]
