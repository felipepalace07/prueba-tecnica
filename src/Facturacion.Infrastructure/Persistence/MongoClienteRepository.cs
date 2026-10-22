using Facturacion.Application.Abstractions;
using Facturacion.Domain.Entities;
using Facturacion.Domain.Enums;
using Facturacion.Infrastructure.Configuration;
using Facturacion.Infrastructure.Persistence.Documents;
using Facturacion.Infrastructure.Persistence.Mappers;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Facturacion.Infrastructure.Persistence;

public sealed class MongoClienteRepository : IClienteRepository
{
    private readonly IMongoCollection<ClienteDocument> _clientes;

    public MongoClienteRepository(IMongoDatabase database, IOptions<MongoSettings> settings)
    {
        _clientes = database.GetCollection<ClienteDocument>(settings.Value.ColeccionClientes);
    }

    public async Task<IReadOnlyList<Cliente>> ObtenerTodosAsync(CancellationToken ct = default)
    {
        var documentos = await _clientes.Find(FilterDefinition<ClienteDocument>.Empty).ToListAsync(ct);
        return documentos.Select(d => d.ADominio()).ToList();
    }

    public async Task<IReadOnlyList<Cliente>> ObtenerConFacturasPorNotificarAsync(CancellationToken ct = default)
    {
        string[] estadosPorNotificar =
        [
            EstadoFacturaMapper.ATexto(EstadoFactura.PrimerRecordatorio),
            EstadoFacturaMapper.ATexto(EstadoFactura.SegundoRecordatorio)
        ];

        var filtro = Builders<ClienteDocument>.Filter.ElemMatch(
            c => c.Facturas,
            f => estadosPorNotificar.Contains(f.Estado));

        var documentos = await _clientes.Find(filtro).ToListAsync(ct);
        return documentos.Select(d => d.ADominio()).ToList();
    }

    public async Task<bool> ActualizarEstadoFacturaAsync(
        string clienteId,
        string numeroFactura,
        EstadoFactura estadoEsperado,
        EstadoFactura estadoNuevo,
        CancellationToken ct = default)
    {
        var textoEsperado = EstadoFacturaMapper.ATexto(estadoEsperado);
        var textoNuevo = EstadoFacturaMapper.ATexto(estadoNuevo);
        var filtros = Builders<ClienteDocument>.Filter;

        // Filtra por cliente y por la factura en el estado esperado:
        // si otro proceso ya la cambió, no se modifica nada (actualización idempotente).
        var filtro = filtros.And(
            filtros.Eq(c => c.Id, clienteId),
            filtros.ElemMatch(c => c.Facturas, f => f.Numero == numeroFactura && f.Estado == textoEsperado));

        var actualizacion = Builders<ClienteDocument>.Update.Set("facturas.$.estado", textoNuevo);

        var resultado = await _clientes.UpdateOneAsync(filtro, actualizacion, cancellationToken: ct);
        return resultado.ModifiedCount == 1;
    }
}
