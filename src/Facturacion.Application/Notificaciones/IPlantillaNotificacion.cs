using Facturacion.Application.Abstractions;
using Facturacion.Domain.Entities;
using Facturacion.Domain.Enums;

namespace Facturacion.Application.Notificaciones;

/// <summary>
/// Estrategia de notificación según el estado de la factura.
/// Agregar un nuevo tipo de recordatorio solo requiere una nueva implementación (Open/Closed).
/// </summary>
public interface IPlantillaNotificacion
{
    EstadoFactura EstadoOrigen { get; }

    MensajeEmail Crear(Cliente cliente, Factura factura);
}
