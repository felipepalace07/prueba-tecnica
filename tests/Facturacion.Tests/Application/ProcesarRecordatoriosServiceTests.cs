using Facturacion.Application.Abstractions;
using Facturacion.Application.Notificaciones;
using Facturacion.Application.Services;
using Facturacion.Domain.Entities;
using Facturacion.Domain.Enums;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Facturacion.Tests.Application;

public class ProcesarRecordatoriosServiceTests
{
    private const string ClienteId = "id-1";
    private const string Email = "c1@test.com";

    private readonly Mock<IClienteRepository> _repositorio = new();
    private readonly Mock<IEmailSender> _emailSender = new();

    private ProcesarRecordatoriosService CrearServicio() => new(
        _repositorio.Object,
        _emailSender.Object,
        [new PlantillaPrimerRecordatorio(), new PlantillaSegundoRecordatorio()],
        NullLogger<ProcesarRecordatoriosService>.Instance);

    private static Cliente ClienteCon(params Factura[] facturas) =>
        new(ClienteId, "Cliente Uno", Email, facturas);

    private static Factura FacturaEn(EstadoFactura estado, string numero = "F-1") =>
        new(numero, 100000, DateTime.UtcNow, estado);

    private void DadoQueExisten(params Cliente[] clientes) =>
        _repositorio
            .Setup(r => r.ObtenerConFacturasPorNotificarAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(clientes.ToList());

    private void DadoQueLaActualizacionResponde(bool resultado) =>
        _repositorio
            .Setup(r => r.ActualizarEstadoFacturaAsync(
                It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<EstadoFactura>(), It.IsAny<EstadoFactura>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(resultado);

    [Fact]
    public async Task PrimerRecordatorio_EnviaCorreoDeSegundoRecordatorioYActualizaEstado()
    {
        DadoQueExisten(ClienteCon(FacturaEn(EstadoFactura.PrimerRecordatorio)));
        DadoQueLaActualizacionResponde(true);

        var resultado = await CrearServicio().EjecutarAsync();

        var factura = Assert.Single(resultado.Resultados);
        Assert.True(factura.Exitoso);
        Assert.Equal(EstadoFactura.SegundoRecordatorio, factura.EstadoNuevo);

        _emailSender.Verify(e => e.EnviarAsync(
            It.Is<MensajeEmail>(m => m.Para == Email && m.Asunto.Contains("segundo recordatorio")),
            It.IsAny<CancellationToken>()), Times.Once);

        _repositorio.Verify(r => r.ActualizarEstadoFacturaAsync(
            ClienteId, "F-1", EstadoFactura.PrimerRecordatorio, EstadoFactura.SegundoRecordatorio,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SegundoRecordatorio_EnviaCorreoDeDesactivacionYDesactiva()
    {
        DadoQueExisten(ClienteCon(FacturaEn(EstadoFactura.SegundoRecordatorio)));
        DadoQueLaActualizacionResponde(true);

        var resultado = await CrearServicio().EjecutarAsync();

        Assert.Equal(EstadoFactura.Desactivado, Assert.Single(resultado.Resultados).EstadoNuevo);

        _emailSender.Verify(e => e.EnviarAsync(
            It.Is<MensajeEmail>(m => m.Asunto.Contains("desactivado")),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SiFallaElEnvioDelCorreo_NoActualizaElEstado()
    {
        DadoQueExisten(ClienteCon(FacturaEn(EstadoFactura.PrimerRecordatorio)));
        _emailSender
            .Setup(e => e.EnviarAsync(It.IsAny<MensajeEmail>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("SMTP no disponible"));

        var resultado = await CrearServicio().EjecutarAsync();

        var factura = Assert.Single(resultado.Resultados);
        Assert.False(factura.Exitoso);
        Assert.Equal("SMTP no disponible", factura.Error);

        _repositorio.Verify(r => r.ActualizarEstadoFacturaAsync(
            It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<EstadoFactura>(), It.IsAny<EstadoFactura>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task SiUnaFacturaFalla_ContinuaConLasDemas()
    {
        DadoQueExisten(ClienteCon(
            FacturaEn(EstadoFactura.PrimerRecordatorio, "F-1"),
            FacturaEn(EstadoFactura.SegundoRecordatorio, "F-2")));
        DadoQueLaActualizacionResponde(true);

        _emailSender
            .SetupSequence(e => e.EnviarAsync(It.IsAny<MensajeEmail>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Error temporal"))
            .Returns(Task.CompletedTask);

        var resultado = await CrearServicio().EjecutarAsync();

        Assert.Equal(2, resultado.Procesadas);
        Assert.Equal(1, resultado.Exitosas);
        Assert.Equal(1, resultado.Fallidas);
    }

    [Fact]
    public async Task SiLaFacturaYaCambioDeEstadoEnBaseDeDatos_SeReportaComoFallo()
    {
        DadoQueExisten(ClienteCon(FacturaEn(EstadoFactura.PrimerRecordatorio)));
        DadoQueLaActualizacionResponde(false);

        var resultado = await CrearServicio().EjecutarAsync();

        Assert.False(Assert.Single(resultado.Resultados).Exitoso);
    }

    [Fact]
    public async Task FacturasPendientes_NoSeNotifican()
    {
        DadoQueExisten(ClienteCon(FacturaEn(EstadoFactura.Pendiente)));

        var resultado = await CrearServicio().EjecutarAsync();

        Assert.Empty(resultado.Resultados);
        _emailSender.Verify(e => e.EnviarAsync(
            It.IsAny<MensajeEmail>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ProcesaFacturasDeVariosClientes()
    {
        DadoQueExisten(
            new Cliente("1", "Uno", "uno@test.com", [FacturaEn(EstadoFactura.PrimerRecordatorio, "F-1")]),
            new Cliente("2", "Dos", "dos@test.com", [FacturaEn(EstadoFactura.SegundoRecordatorio, "F-2")]),
            new Cliente("3", "Tres", "tres@test.com", [FacturaEn(EstadoFactura.PrimerRecordatorio, "F-3")]));
        DadoQueLaActualizacionResponde(true);

        var resultado = await CrearServicio().EjecutarAsync();

        Assert.Equal(3, resultado.Exitosas);
        _emailSender.Verify(e => e.EnviarAsync(
            It.IsAny<MensajeEmail>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
    }
}
