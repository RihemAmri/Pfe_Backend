using Microsoft.Extensions.Options;
using Notification.API;
using Notification.API.Services;

var builder = WebApplication.CreateBuilder(args);

// 1️⃣ Forcer le port HTTP 4007
builder.WebHost.UseUrls("http://*:4007");


// 2️⃣ Bind de la section "DatabaseSettings" du appsettings.json
builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("DatabaseSettings"));

// 3️⃣ Enregistre NotificationService pour qu'il soit injecté dans le controller
builder.Services.AddScoped<NotificationService>();

// 4️⃣ Services ASP.NET Core habituels
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
// 5️⃣ Swagger en dev
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
