# Étape 1 : Utilisation de l'image de base .NET SDK pour la construction
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Définir le répertoire de travail
WORKDIR /src

# Copier le fichier .csproj du projet DepotDocuments.API
COPY ./DepotDocuments.API.csproj ./

# Restaurer les dépendances du projet
RUN dotnet restore "DepotDocuments.API.csproj"

# Copier le reste des fichiers du projet dans le conteneur
COPY . .

# Publier le projet en mode Release
RUN dotnet publish "DepotDocuments.API.csproj" -c Release -o /app/publish

# Étape 2 : Créer l'image de l'application finale
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

# Définir le répertoire de travail
WORKDIR /app

# Copier les fichiers publiés depuis l'étape précédente
COPY --from=build /app/publish .

# Exposer le port sur lequel l'application écoutera
EXPOSE 80

# Définir le point d'entrée de l'application
ENTRYPOINT ["dotnet", "DepotDocuments.API.dll"]
