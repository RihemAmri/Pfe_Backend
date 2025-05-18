# Étape 1 : Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copier le fichier .csproj et restaurer les dépendances
COPY ./Statistiques.API.csproj ./
RUN dotnet restore "Statistiques.API.csproj"

# Copier tout le reste du code et publier le projet en mode Release
COPY . ./
RUN dotnet publish "Statistiques.API.csproj" -c Release -o /app/publish

# Étape 2 : Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Copier l'application publiée depuis l'étape Build
COPY --from=build /app/publish ./

# Exposer le port utilisé par l'application (ici 80)
EXPOSE 80

# Lancer l'application
ENTRYPOINT ["dotnet", "Statistiques.API.dll"]
