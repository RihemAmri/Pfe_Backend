using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Notification.API;

var builder = WebApplication.CreateBuilder(args);

// 1️⃣ Forcer le port HTTP 4008
builder.WebHost.UseUrls("http://*:4008");

// 2️⃣ Bind de la section "DatabaseSettings" du appsettings.json
builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("DatabaseSettings"));

// 3️⃣ Enregistrer MongoDB et NotificationService dans l'injection de dépendances
builder.Services.AddSingleton<IMongoDatabase>(serviceProvider =>
{
    var databaseSettings = serviceProvider.GetRequiredService<IOptions<DatabaseSettings>>().Value;
    var mongoClient = new MongoClient(databaseSettings.ConnectionString);
    return mongoClient.GetDatabase(databaseSettings.DatabaseName);
});

// 4️⃣ Enregistrer NotificationService
  // Si tu as un NotificationService

// 5️⃣ Services ASP.NET Core habituels
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // URL de ton frontend Angular
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();
app.UseCors("AllowAngular");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
