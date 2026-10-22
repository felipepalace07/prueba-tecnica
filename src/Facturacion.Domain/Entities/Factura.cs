using Facturacion.Domain.Enums;
using Facturacion.Domain.Exceptions;

namespace Facturacion.Domain.Entities;

public sealed class Factura
{
    public Factura(string numero, decimal valor, DateTime fechaVencimiento, EstadoFactura estado)
    {
        if (string.IsNullOrWhiteSpace(numero)) throw new ArgumentException("El número es obligatorio.", nameof(numero));
        if (valor < 0) throw new ArgumentOutOfRangeException(nameof(valor), "El valor no puede ser negativo.");

        Numero = numero;
        Valor = valor;
        FechaVencimiento = fechaVencimiento;
        Estado = estado;
    }

    public string Numero { get; }
    public decimal Valor { get; }
    public DateTime FechaVencimiento { get; }
    public EstadoFactura Estado { get; private set; }

    public bool RequiereNotificacion =>
        Estado is EstadoFactura.PrimerRecordatorio or EstadoFactura.SegundoRecordatorio;

    public EstadoFactura ObtenerSiguienteEstado() => Estado switch
    {
        EstadoFactura.PrimerRecordatorio => EstadoFactura.SegundoRecordatorio,
        EstadoFactura.SegundoRecordatorio => EstadoFactura.Desactivado,
        _ => throw new TransicionEstadoInvalidaException(Numero, Estado)
    };

    public void AvanzarEstado() => Estado = ObtenerSiguienteEstado();
}
