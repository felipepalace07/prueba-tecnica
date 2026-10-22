using System.Globalization;
using Facturacion.Application.Abstractions;
using Facturacion.Domain.Entities;
using Facturacion.Domain.Enums;

namespace Facturacion.Application.Notificaciones;

public sealed class PlantillaPrimerRecordatorio : IPlantillaNotificacion
{
    private static readonly CultureInfo Cultura = CultureInfo.GetCultureInfo("es-CO");

    public EstadoFactura EstadoOrigen => EstadoFactura.PrimerRecordatorio;

    public MensajeEmail Crear(Cliente cliente, Factura factura) => new(
        cliente.Email,
        $"Factura {factura.Numero}: segundo recordatorio de pago",
        $"""
        <p>Hola {cliente.Nombre},</p>
        <p>Su factura <b>{factura.Numero}</b> por <b>{factura.Valor.ToString("C0", Cultura)}</b>
        continúa pendiente y ha pasado a <b>segundo recordatorio</b>.</p>
        <p>Le invitamos a realizar el pago lo antes posible.</p>
        """);
}
