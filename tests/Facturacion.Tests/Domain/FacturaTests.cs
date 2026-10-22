using Facturacion.Domain.Entities;
using Facturacion.Domain.Enums;
using Facturacion.Domain.Exceptions;

namespace Facturacion.Tests.Domain;

public class FacturaTests
{
    private static Factura CrearFactura(EstadoFactura estado) =>
        new("F-1", 1000, DateTime.UtcNow, estado);

    [Theory]
    [InlineData(EstadoFactura.PrimerRecordatorio, EstadoFactura.SegundoRecordatorio)]
    [InlineData(EstadoFactura.SegundoRecordatorio, EstadoFactura.Desactivado)]
    public void AvanzarEstado_DesdeRecordatorio_PasaAlSiguienteEstado(EstadoFactura inicial, EstadoFactura esperado)
    {
        var factura = CrearFactura(inicial);

        factura.AvanzarEstado();

        Assert.Equal(esperado, factura.Estado);
    }

    [Theory]
    [InlineData(EstadoFactura.Pendiente)]
    [InlineData(EstadoFactura.Desactivado)]
    public void AvanzarEstado_DesdeEstadoNoNotificable_LanzaExcepcion(EstadoFactura estado)
    {
        var factura = CrearFactura(estado);

        Assert.Throws<TransicionEstadoInvalidaException>(() => factura.AvanzarEstado());
    }

    [Theory]
    [InlineData(EstadoFactura.PrimerRecordatorio, true)]
    [InlineData(EstadoFactura.SegundoRecordatorio, true)]
    [InlineData(EstadoFactura.Pendiente, false)]
    [InlineData(EstadoFactura.Desactivado, false)]
    public void RequiereNotificacion_DependeDelEstado(EstadoFactura estado, bool esperado)
    {
        var factura = CrearFactura(estado);

        Assert.Equal(esperado, factura.RequiereNotificacion);
    }

    [Fact]
    public void Constructor_ConValorNegativo_LanzaExcepcion()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new Factura("F-1", -1, DateTime.UtcNow, EstadoFactura.Pendiente));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_SinNumero_LanzaExcepcion(string numero)
    {
        Assert.Throws<ArgumentException>(() =>
            new Factura(numero, 1000, DateTime.UtcNow, EstadoFactura.Pendiente));
    }
}
