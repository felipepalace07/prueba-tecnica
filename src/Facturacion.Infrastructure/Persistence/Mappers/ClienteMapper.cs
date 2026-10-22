using Facturacion.Domain.Entities;
using Facturacion.Infrastructure.Persistence.Documents;

namespace Facturacion.Infrastructure.Persistence.Mappers;

internal static class ClienteMapper
{
    public static Cliente ADominio(this ClienteDocument documento) => new(
        documento.Id,
        documento.Nombre,
        documento.Email,
        documento.Facturas.Select(f => new Factura(
            f.Numero,
            f.Valor,
            f.FechaVencimiento,
            EstadoFacturaMapper.DesdeTexto(f.Estado))));
}
