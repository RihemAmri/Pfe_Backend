using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DepotDocuments.API.Services.Ocr;


using DepotDocuments.API.Services.Ocr.Interfaces;
using DepotDocuments.API.Data;  // Ajouter l'importation pour DocumentContext
using DepotDocuments.API.Router;
using DepotDocuments.API.Repositories; // 🔧 à ajouter tout en haut
using DepotDocuments.API.Services;
using DinkToPdf;
using DinkToPdf.Contracts;
using System.Runtime.InteropServices;

var context = new CustomAssemblyLoadContext();

var wkhtmlPath = Path.Combine(AppContext.BaseDirectory, "libs", "wkhtmltopdf", "libwkhtmltox.dll");
context.LoadUnmanagedLibrary(wkhtmlPath);
var builder = WebApplication.CreateBuilder(args);

// Configure la prise en charge de la connexion au service via l'URL
builder.WebHost.UseUrls("http://*:4002");

// Ajout des services à l'application
builder.Services.AddControllers();
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

// Enregistrer les services pour l'OCR et l'upload Cloudinary
builder.Services.AddScoped<IOcrProcessor, CinOcrProcessor>();  // Tu peux ajouter plus d'implementations pour OCR selon tes besoins
 builder.Services.AddScoped<IOcrProcessor, FichePaieOcrProcessor>();
builder.Services.AddScoped<IOcrProcessor, AttestationSalaireOcrProcessor>();
builder.Services.AddScoped<OcrConfigService>();
var notificationApiUrl = builder.Configuration["NOTIFICATION_API_URL"];
builder.Services.AddHttpClient("NotificationApi", client =>
{
    client.BaseAddress = new Uri(notificationApiUrl);
});

builder.Services.AddSingleton<PdfService>();
builder.Services.AddScoped<OcrDispatcherService>();
builder.Services.AddScoped<CloudinaryService>();
builder.Services.AddScoped<MailService>();
builder.Services.AddScoped<ICreditDocumentService, CreditDocumentService>();
//builder.Services.AddSingleton<BrevoHttpMailService>();
// Enregistrement du service de MongoDB
builder.Services.AddScoped<DocuSignService>();
builder.Services.AddScoped<YousignService>();
builder.Services.AddScoped<IDocumentContext, DocumentContext>();  // Enregistrer le DocumentContext pour l'accès à MongoDB
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();  // Enregistrer le repository de documents
builder.Services.AddHttpClient();
// Configurer Swagger/OpenAPI pour la documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS configuration (permettre à Angular d'accéder à l'API)
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

// Appliquer la politique CORS
app.UseCors("AllowAngular");

// Configurer le pipeline de requêtes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

// Mapper les contrôleurs
app.MapControllers();

app.Run();
public class CustomAssemblyLoadContext : System.Runtime.Loader.AssemblyLoadContext
{
    public IntPtr LoadUnmanagedLibrary(string absolutePath)
    {
        return LoadUnmanagedDll(absolutePath);
    }

    protected override IntPtr LoadUnmanagedDll(String unmanagedDllPath)
    {
        return LoadUnmanagedDllFromPath(unmanagedDllPath);
    }

    protected override System.Reflection.Assembly Load(System.Reflection.AssemblyName assemblyName)
    {
        return null;
    }
}
