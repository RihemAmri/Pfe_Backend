using Authentication.API.Data;
using Authentication.API.Repository;
using Authentication.API.BusinessLogic;
using Authentication.API.Shared;
using Authentication.API.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://*:4000");
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSingleton<CloudinaryService>();  // CloudinaryService ajouté comme singleton

// Swagger/OpenAPI setup
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurer la connexion à la base de données (MongoDB)
builder.Services.Configure<DatabaseSettings>(options => builder.Configuration.GetSection("DatabaseSettings").Bind(options));

// Configurer le contexte de la base de données
builder.Services.AddSingleton<IAuthContext, AuthenticationContext>();

// Ajouter le repository
builder.Services.AddScoped<IUtilisateurRepository, UtilisateurRepository>();
builder.Services.AddScoped<ICompteRepository, CompteRepository>();

// Ajouter le service
builder.Services.AddScoped<IUtilisateurService, UtilisateurService>();

// Ajouter le service TokenService
builder.Services.AddSingleton<TokenService>(new TokenService("TaCléSecrèteTrèsLongueEtSécurisée")); // Clé secrète pour la génération du token

// Ajouter la configuration CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost",
        builder => builder
            .WithOrigins("http://localhost:4200") // Frontend Angular
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();  // Swagger doit être utilisé avant UseSwaggerUI
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
    });
}

// Appliquer la politique CORS
app.UseCors("AllowLocalhost");

app.UseAuthorization();

app.MapControllers();

app.Run();