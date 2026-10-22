using Facturacion.Application.Abstractions;
using Facturacion.Application.Dtos;

namespace Facturacion.Application.Services;

public sealed class ResumenFacturasService(IClienteRepository repositorio) : IResumenFacturasService
{
    public async Task<ResumenFacturasDto> ObtenerAsync(CancellationToken ct = default)
    {
        var clientes = await repositorio.ObtenerTodosAsync(ct);

        var facturas = clientes
            .SelectMany(c => c.Facturas.Select(f => new FacturaResumenDto(
                c.Id, c.Nombre, c.Email, f.Numero, f.Valor, f.FechaVencimiento, f.Estado)))
            .OrderBy(f => f.Numero)
            .ToList();

        var totales = facturas
            .GroupBy(f => f.Estado)
            .Select(g => new TotalEstadoDto(g.Key, g.Count(), g.Sum(f => f.Valor)))
            .OrderBy(t => t.Estado)
            .ToList();

        return new ResumenFacturasDto(facturas, totales);
    }
}
