using Credit.API.Models;
using Credit.API.Services;
using DinkToPdf;
using DinkToPdf.Contracts;

// ✅ Chargement de la librairie native wkhtmltox avant la création du builder
var context = new CustomAssemblyLoadContext();
var wkhtmlPath = Path.Combine(AppContext.BaseDirectory, "DinkToPdfLib", "linux64", "libwkhtmltox.so");
context.LoadUnmanagedLibrary(wkhtmlPath);

// ✅ Création du builder avec la configuration de WebRootPath
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    WebRootPath = "wwwroot",
    Args = args
});

// ✅ Enregistrement du convertisseur PDF
var converter = new SynchronizedConverter(new PdfTools());
builder.Services.AddSingleton<IConverter>(converter);
builder.Services.AddSingleton<MailService>();
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
builder.Services.AddSingleton<PdfService>();

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
