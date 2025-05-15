using Credit.API.Models;
using Credit.API.Services;
var builder = WebApplication.CreateBuilder(args);

// ✅ Configuration de l'URL du service
builder.WebHost.UseUrls("http://*:4011");
var notificationApiUrl = builder.Configuration["NOTIFICATION_API_URL"];
builder.Services.AddHttpClient("NotificationApi", client =>
{
    client.BaseAddress = new Uri(notificationApiUrl);
});

// ✅ Ajout des services nécessaires
builder.Services.AddControllers();
builder.Services.Configure<CreditDatabaseSettings>(
    builder.Configuration.GetSection("DatabaseSettings"));
builder.Services.AddScoped<ICreditService, CreditService>();
builder.Services.AddHostedService<CreditHostedService>();
builder.Services.AddHostedService<RabbitMQConsumer>();
builder.Services.AddScoped<CloudinaryService>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ Configuration du CORS pour autoriser le frontend Angular
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ✅ Construction de l'application
var app = builder.Build();

// ✅ Middleware CORS
app.UseCors("AllowAngular");

// ✅ Middleware Swagger uniquement en développement
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ✅ Redirection HTTP vers HTTPS (utile en prod)
app.UseHttpsRedirection();

// ✅ Middleware d'autorisation
app.UseAuthorization();

// ✅ Routing des contrôleurs
app.MapControllers();

// ✅ Lancement de l'application
app.Run();
