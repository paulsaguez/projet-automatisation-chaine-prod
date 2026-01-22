# Étape 1: Build de l'application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copie du fichier projet et restauration des dépendances
COPY *.csproj ./
RUN dotnet restore

# Copie du code source et build
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Étape 2: Image runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

# Copie des fichiers publiés
COPY --from=build /app/publish .

# Expose le port de l'API
EXPOSE 5000

# Variables d'environnement
ENV ASPNETCORE_URLS=http://+:5000
ENV DOTNET_RUNNING_IN_CONTAINER=true

ENTRYPOINT ["dotnet", "API.dll"]
