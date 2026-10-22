using Facturacion.Application.Dtos;

namespace Facturacion.Application.Services;

public interface IResumenFacturasService
{
    Task<ResumenFacturasDto> ObtenerAsync(CancellationToken ct = default);
}
