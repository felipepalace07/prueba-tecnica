using System.Text.Json.Serialization;
using Facturacion.Application;
using Facturacion.Application.Abstractions;
using Facturacion.Infrastructure;

const string PoliticaCors = "Frontend";

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Composition root: aquí se conectan las capas
builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Services.AddCors(o => o.AddPolicy(PoliticaCors, politica => politica
    .WithOrigins(builder.Configuration["Cors:Origen"] ?? "http://localhost:4200")
    .AllowAnyHeader()
    .AllowAnyMethod()));

var app = builder.Build();

app.UseExceptionHandler();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors(PoliticaCors);
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var inicializador = scope.ServiceProvider.GetRequiredService<IInicializadorDatos>();
    await inicializador.SembrarSiVacioAsync();
}

app.Run();
