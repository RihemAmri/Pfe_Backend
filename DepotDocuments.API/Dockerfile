# Étape 1 : Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ./DepotDocuments.API.csproj ./
RUN dotnet restore "DepotDocuments.API.csproj"

COPY . ./
RUN dotnet publish "DepotDocuments.API.csproj" -c Release -o /app/publish

# Étape 2 : Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# ✅ Installer les dépendances nécessaires pour wkhtmltopdf
RUN apt-get update && \
    apt-get install -y libgdiplus libx11-dev && \
    apt-get clean

# ✅ Copier la bibliothèque native dans le chemin système et projet
COPY ./DinkToPdfLib/linux64/libwkhtmltox.so /usr/lib/libwkhtmltox.so
COPY DinkToPdfLib/linux64/libwkhtmltox.so /app/DinkToPdfLib/linux64/

# ✅ Copier les fichiers de build
COPY --from=build /app/publish .

EXPOSE 80

ENTRYPOINT ["dotnet", "DepotDocuments.API.dll"]
