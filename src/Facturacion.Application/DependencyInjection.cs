using Facturacion.Application.Notificaciones;
using Facturacion.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Facturacion.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IProcesarRecordatoriosService, ProcesarRecordatoriosService>();
        services.AddScoped<IResumenFacturasService, ResumenFacturasService>();

        services.AddSingleton<IPlantillaNotificacion, PlantillaPrimerRecordatorio>();
        services.AddSingleton<IPlantillaNotificacion, PlantillaSegundoRecordatorio>();

        return services;
    }
}
