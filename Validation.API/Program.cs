using DinkToPdf;
using DinkToPdf.Contracts;
using Validation.API.Services;
using Validation.API.Repositories;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    WebRootPath = "wwwroot",
    Args = args
});

// ✅ PAS besoin de charger manuellement la DLL avec CustomAssemblyLoadContext

builder.WebHost.UseUrls("http://*:4010");

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
var notificationApiUrl = builder.Configuration["NOTIFICATION_API_URL"];
builder.Services.AddHttpClient("NotificationApi", client =>
{
    client.BaseAddress = new Uri(notificationApiUrl);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IValidationService, ValidationService>();
builder.Services.AddScoped<IValidationRepository, ValidationRepository>();
builder.Services.AddScoped<PdfService>();
builder.Services.AddScoped<EmailService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.UseCors("AllowAngular");
app.UseAuthorization();
app.MapControllers();

app.Run();
