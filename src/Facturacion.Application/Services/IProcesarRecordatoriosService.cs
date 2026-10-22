using Facturacion.Application.Dtos;

namespace Facturacion.Application.Services;

public interface IProcesarRecordatoriosService
{
    Task<ResultadoProcesoDto> EjecutarAsync(CancellationToken ct = default);
}
