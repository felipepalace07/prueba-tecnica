using Facturacion.Domain.Enums;

namespace Facturacion.Application.Dtos;

public sealed record ResultadoFacturaDto(
    string Cliente,
    string NumeroFactura,
    EstadoFactura EstadoAnterior,
    EstadoFactura? EstadoNuevo,
    bool Exitoso,
    string? Error);
