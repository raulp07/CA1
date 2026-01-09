# Esta es la fase de "Construcción" (Build)
# Usamos la imagen del SDK para compilar el código
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# 1. Copiamos los archivos de proyecto (.csproj) y restauramos dependencias
# Esto optimiza el caché de Docker; si no cambian las dependencias, este paso se salta
COPY ["CA1.WebAPI/CA1.WebAPI.csproj", "CA1.WebAPI/"]
COPY ["CA1.Application/CA1.Application.csproj", "CA1.Application/"]
COPY ["CA1.Domain/CA1.Domain.csproj", "CA1.Domain/"]
COPY ["CA1.Infrastructure/CA1.Infrastructure.csproj", "CA1.Infrastructure/"]

# Ejecutamos restore para bajar los paquetes NuGet
RUN dotnet restore "CA1.WebAPI/CA1.WebAPI.csproj"

# 2. Copiamos el resto del código fuente
COPY . .

# 3. Compilamos y Publicamos
WORKDIR "/src/CA1.WebAPI"
RUN dotnet build "CA1.WebAPI.csproj" -c Release -o /app/build
RUN dotnet publish "CA1.WebAPI.csproj" -c Release -o /app/publish

# Esta es la fase "Final" (Runtime)
# Usamos una imagen más ligera solo con lo necesario para ejecutar (sin compilador)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Copiamos los archivos compilados desde la fase anterior
COPY --from=build /app/publish .

# Configuramos el puerto y la ejecución
# Por defecto .NET en contenedores usa el puerto 8080
ENV ASPNETCORE_HTTP_PORTS=8080
ENV ASPNETCORE_ENVIRONMENT=Development
EXPOSE 8080

ENTRYPOINT ["dotnet", "CA1.WebAPI.dll"]
