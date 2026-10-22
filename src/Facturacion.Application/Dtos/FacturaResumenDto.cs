using Facturacion.Domain.Enums;

namespace Facturacion.Application.Dtos;

public sealed record FacturaResumenDto(
    string ClienteId,
    string Cliente,
    string Email,
    string Numero,
    decimal Valor,
    DateTime FechaVencimiento,
    EstadoFactura Estado);
