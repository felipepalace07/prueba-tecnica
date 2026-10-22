using Facturacion.Domain.Enums;

namespace Facturacion.Application.Dtos;

public sealed record TotalEstadoDto(EstadoFactura Estado, int Cantidad, decimal Valor);
