namespace Facturacion.Application.Dtos;

public sealed record ResumenFacturasDto(
    IReadOnlyList<FacturaResumenDto> Facturas,
    IReadOnlyList<TotalEstadoDto> TotalesPorEstado);
