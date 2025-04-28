using DinkToPdf;
using DinkToPdf.Contracts;
using Validation.API.Services;
using Validation.API.Repositories;
using System.Reflection;
using System.Runtime.Loader;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    WebRootPath = "wwwroot",
    Args = args
});


// Charger la DLL native wkhtmltox
var wkHtmlToPdfPath = Path.Combine(Directory.GetCurrentDirectory(), "DinkToPdfLib", "64 bit", "libwkhtmltox.dll");
var context = new CustomAssemblyLoadContext();
context.LoadUnmanagedLibrary(wkHtmlToPdfPath);
//builder.WebHost.UseWebRoot("wwwroot");
// Add services to the container
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
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

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles();

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
