using Facturacion.Domain.Entities;
using Facturacion.Domain.Enums;

namespace Facturacion.Application.Abstractions;

public interface IClienteRepository
{
    Task<IReadOnlyList<Cliente>> ObtenerTodosAsync(CancellationToken ct = default);

    Task<IReadOnlyList<Cliente>> ObtenerConFacturasPorNotificarAsync(CancellationToken ct = default);

    /// <summary>
    /// Actualiza el estado solo si la factura sigue en el estado esperado.
    /// Devuelve false si no se modificó (por ejemplo, porque otro proceso ya la actualizó).
    /// </summary>
    Task<bool> ActualizarEstadoFacturaAsync(
        string clienteId,
        string numeroFactura,
        EstadoFactura estadoEsperado,
        EstadoFactura estadoNuevo,
        CancellationToken ct = default);
}
