using Facturacion.Application.Abstractions;
using Facturacion.Infrastructure.Configuration;
using Facturacion.Infrastructure.Email;
using Facturacion.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Facturacion.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoSettings>(configuration.GetSection(MongoSettings.Seccion));
        services.Configure<SmtpSettings>(configuration.GetSection(SmtpSettings.Seccion));

        services.AddSingleton<IMongoClient>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<MongoSettings>>().Value;
            return new MongoClient(settings.ConnectionString);
        });

        services.AddSingleton(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<MongoSettings>>().Value;
            return sp.GetRequiredService<IMongoClient>().GetDatabase(settings.BaseDatos);
        });

        services.AddScoped<IClienteRepository, MongoClienteRepository>();
        services.AddScoped<IEmailSender, SmtpEmailSender>();
        services.AddScoped<IInicializadorDatos, InicializadorDatos>();

        return services;
    }
}
