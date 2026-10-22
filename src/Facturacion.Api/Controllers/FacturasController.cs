using Facturacion.Application.Abstractions;
using Facturacion.Application.Dtos;
using Facturacion.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Facturacion.Api.Controllers;

[ApiController]
[Route("api/facturas")]
[Produces("application/json")]
public sealed class FacturasController(
    IResumenFacturasService resumenService,
    IProcesarRecordatoriosService procesarService,
    IInicializadorDatos inicializadorDatos) : ControllerBase
{
    /// <summary>Resumen de todas las facturas de todos los clientes.</summary>
    [HttpGet("resumen")]
    [ProducesResponseType<ResumenFacturasDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<ResumenFacturasDto>> ObtenerResumen(CancellationToken ct) =>
        Ok(await resumenService.ObtenerAsync(ct));

    /// <summary>
    /// Envía el correo a cada cliente con facturas en primer o segundo recordatorio
    /// y actualiza su estado.
    /// </summary>
    [HttpPost("procesar-recordatorios")]
    [ProducesResponseType<ResultadoProcesoDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<ResultadoProcesoDto>> ProcesarRecordatorios(CancellationToken ct) =>
        Ok(await procesarService.EjecutarAsync(ct));

    /// <summary>Restaura los datos de prueba de los 3 clientes.</summary>
    [HttpPost("reiniciar-datos")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ReiniciarDatos(CancellationToken ct)
    {
        await inicializadorDatos.ReiniciarAsync(ct);
        return NoContent();
    }
}
