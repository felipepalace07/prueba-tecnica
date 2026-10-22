using System.Globalization;
using Facturacion.Application.Abstractions;
using Facturacion.Domain.Entities;
using Facturacion.Domain.Enums;

namespace Facturacion.Application.Notificaciones;

public sealed class PlantillaSegundoRecordatorio : IPlantillaNotificacion
{
    private static readonly CultureInfo Cultura = CultureInfo.GetCultureInfo("es-CO");

    public EstadoFactura EstadoOrigen => EstadoFactura.SegundoRecordatorio;

    public MensajeEmail Crear(Cliente cliente, Factura factura) => new(
        cliente.Email,
        $"Factura {factura.Numero}: su servicio será desactivado",
        $"""
        <p>Hola {cliente.Nombre},</p>
        <p>Su factura <b>{factura.Numero}</b> por <b>{factura.Valor.ToString("C0", Cultura)}</b>
        no ha sido pagada después del segundo recordatorio, por lo que el servicio
        <b>va a ser desactivado</b>.</p>
        """);
}
