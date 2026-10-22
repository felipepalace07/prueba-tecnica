using Facturacion.Application.Abstractions;
using Facturacion.Application.Services;
using Facturacion.Domain.Entities;
using Facturacion.Domain.Enums;
using Moq;

namespace Facturacion.Tests.Application;

public class ResumenFacturasServiceTests
{
    [Fact]
    public async Task ObtenerAsync_AgrupaFacturasDeTodosLosClientesPorEstado()
    {
        var repositorio = new Mock<IClienteRepository>();
        repositorio
            .Setup(r => r.ObtenerTodosAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Cliente>
            {
                new("1", "Uno", "uno@test.com",
                [
                    new Factura("F-1", 100, DateTime.UtcNow, EstadoFactura.Pendiente),
                    new Factura("F-2", 200, DateTime.UtcNow, EstadoFactura.Desactivado)
                ]),
                new("2", "Dos", "dos@test.com",
                [
                    new Factura("F-3", 300, DateTime.UtcNow, EstadoFactura.Pendiente)
                ])
            });

        var resumen = await new ResumenFacturasService(repositorio.Object).ObtenerAsync();

        Assert.Equal(3, resumen.Facturas.Count);

        var pendientes = Assert.Single(resumen.TotalesPorEstado, t => t.Estado == EstadoFactura.Pendiente);
        Assert.Equal(2, pendientes.Cantidad);
        Assert.Equal(400, pendientes.Valor);

        var desactivadas = Assert.Single(resumen.TotalesPorEstado, t => t.Estado == EstadoFactura.Desactivado);
        Assert.Equal(1, desactivadas.Cantidad);
    }
}
