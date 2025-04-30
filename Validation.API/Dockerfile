# Étape 1 : Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ./Validation.API.csproj ./
RUN dotnet restore "Validation.API.csproj"

COPY . ./
RUN dotnet publish "Validation.API.csproj" -c Release -o /app/publish

# Étape 2 : Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# ❗ Installer les dépendances nécessaires pour wkhtmltopdf
RUN apt-get update && \
    apt-get install -y libgdiplus libx11-dev && \
    apt-get clean

# 📁 Ajouter wkhtmltopdf (important pour DinkToPdf)
COPY ./DinkToPdfLib/linux64/libwkhtmltox.so /usr/lib/libwkhtmltox.so
COPY DinkToPdfLib/linux64/libwkhtmltox.so /app/DinkToPdfLib/linux64/

COPY --from=build /app/publish .

EXPOSE 80
ENTRYPOINT ["dotnet", "Validation.API.dll"]
