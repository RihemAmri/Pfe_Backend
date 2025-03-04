using Authentication.API.Data;
using Authentication.API.Repository;
using Authentication.API.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//interface+class
builder.Services.Configure<DatabaseSettings>(op => builder.Configuration.GetSection("DatabaseSettings").Bind(op));
builder.Services.AddSingleton<IAuthContext, AuthenticationContext>();
builder.Services.AddScoped<IUtilisateurRepository, UtilisateurRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
