using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DepotDocuments.API.Services.Ocr;
using DepotDocuments.API.Services.Ocr.Interfaces;
using DepotDocuments.API.Data;
using DepotDocuments.API.Router;
using DepotDocuments.API.Repositories;
using DepotDocuments.API.Services;
using DinkToPdf;
using DinkToPdf.Contracts;
using System.Runtime.InteropServices;

var context = new CustomAssemblyLoadContext();
var wkhtmlPath = Path.Combine(AppContext.BaseDirectory, "DinkToPdfLib", "linux64", "libwkhtmltox.so");
context.LoadUnmanagedLibrary(wkhtmlPath);

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://*:4002");

// Services
builder.Services.AddControllers();
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

// Choisis UNE SEULE implémentation de IOcrProcessor
builder.Services.AddScoped<IOcrProcessor, CinOcrProcessor>();  // Tu peux ajouter plus d'implementations pour OCR selon tes besoins
 builder.Services.AddScoped<IOcrProcessor, FichePaieOcrProcessor>();
builder.Services.AddScoped<IOcrProcessor, AttestationSalaireOcrProcessor>();

builder.Services.AddScoped<OcrConfigService>();
builder.Services.AddScoped<OcrDispatcherService>();
builder.Services.AddScoped<CloudinaryService>();
builder.Services.AddScoped<MailService>();
builder.Services.AddScoped<ICreditDocumentService, CreditDocumentService>();
builder.Services.AddScoped<DocuSignService>();
builder.Services.AddScoped<YousignService>();
builder.Services.AddScoped<IDocumentContext, DocumentContext>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddSingleton<PdfService>();

// Clients HTTP
builder.Services.AddHttpClient();
var notificationApiUrl = builder.Configuration["NOTIFICATION_API_URL"];
builder.Services.AddHttpClient("NotificationApi", client =>
{
    client.BaseAddress = new Uri(notificationApiUrl);
});
builder.Services.AddHttpClient("AzureOcr", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["AZURE_OCR_ENDPOINT"]);
    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", builder.Configuration["AZURE_OCR_KEY"]);
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
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

app.UseAuthorization();
app.MapControllers();
app.Run();

public class CustomAssemblyLoadContext : System.Runtime.Loader.AssemblyLoadContext
{
    public IntPtr LoadUnmanagedLibrary(string absolutePath)
    {
        return LoadUnmanagedDll(absolutePath);
    }

    protected override IntPtr LoadUnmanagedDll(string unmanagedDllPath)
    {
        return LoadUnmanagedDllFromPath(unmanagedDllPath);
    }

    protected override System.Reflection.Assembly Load(System.Reflection.AssemblyName assemblyName)
    {
        return null;
    }
}
