using Facturacion.Application.Abstractions;
using Facturacion.Application.Dtos;
using Facturacion.Application.Notificaciones;
using Facturacion.Domain.Entities;
using Facturacion.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Facturacion.Application.Services;

public sealed class ProcesarRecordatoriosService(
    IClienteRepository repositorio,
    IEmailSender emailSender,
    IEnumerable<IPlantillaNotificacion> plantillas,
    ILogger<ProcesarRecordatoriosService> logger) : IProcesarRecordatoriosService
{
    private readonly IReadOnlyDictionary<EstadoFactura, IPlantillaNotificacion> _plantillas =
        plantillas.ToDictionary(p => p.EstadoOrigen);

    public async Task<ResultadoProcesoDto> EjecutarAsync(CancellationToken ct = default)
    {
        var clientes = await repositorio.ObtenerConFacturasPorNotificarAsync(ct);
        var resultados = new List<ResultadoFacturaDto>();

        foreach (var cliente in clientes)
        {
            foreach (var factura in cliente.Facturas.Where(f => f.RequiereNotificacion))
            {
                resultados.Add(await ProcesarFacturaAsync(cliente, factura, ct));
            }
        }

        return new ResultadoProcesoDto(resultados);
    }

    private async Task<ResultadoFacturaDto> ProcesarFacturaAsync(Cliente cliente, Factura factura, CancellationToken ct)
    {
        var estadoAnterior = factura.Estado;
        var estadoNuevo = factura.ObtenerSiguienteEstado();

        try
        {
            var mensaje = _plantillas[estadoAnterior].Crear(cliente, factura);
            await emailSender.EnviarAsync(mensaje, ct);

            var actualizado = await repositorio.ActualizarEstadoFacturaAsync(
                cliente.Id, factura.Numero, estadoAnterior, estadoNuevo, ct);

            if (!actualizado)
            {
                return Fallo(cliente, factura, estadoAnterior, "La factura cambió de estado durante el proceso.");
            }

            factura.AvanzarEstado();
            logger.LogInformation(
                "Factura {Numero} de {Cliente}: {EstadoAnterior} -> {EstadoNuevo}",
                factura.Numero, cliente.Nombre, estadoAnterior, estadoNuevo);

            return new ResultadoFacturaDto(cliente.Nombre, factura.Numero, estadoAnterior, estadoNuevo, true, null);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Error procesando la factura {Numero}", factura.Numero);
            return Fallo(cliente, factura, estadoAnterior, ex.Message);
        }
    }

    private static ResultadoFacturaDto Fallo(Cliente cliente, Factura factura, EstadoFactura estadoAnterior, string error) =>
        new(cliente.Nombre, factura.Numero, estadoAnterior, null, false, error);
}
