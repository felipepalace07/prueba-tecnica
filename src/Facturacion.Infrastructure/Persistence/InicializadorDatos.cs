using System.Globalization;
using Facturacion.Application.Abstractions;
using Facturacion.Infrastructure.Configuration;
using Facturacion.Infrastructure.Persistence.Documents;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Facturacion.Infrastructure.Persistence;

public sealed class InicializadorDatos(IMongoDatabase database, IOptions<MongoSettings> settings) : IInicializadorDatos
{
    private IMongoCollection<ClienteDocument> Clientes =>
        database.GetCollection<ClienteDocument>(settings.Value.ColeccionClientes);

    public async Task SembrarSiVacioAsync(CancellationToken ct = default)
    {
        var cantidad = await Clientes.CountDocumentsAsync(FilterDefinition<ClienteDocument>.Empty, cancellationToken: ct);
        if (cantidad == 0)
        {
            await Clientes.InsertManyAsync(DatosIniciales(), cancellationToken: ct);
        }
    }

    public async Task ReiniciarAsync(CancellationToken ct = default)
    {
        await Clientes.DeleteManyAsync(FilterDefinition<ClienteDocument>.Empty, ct);
        await Clientes.InsertManyAsync(DatosIniciales(), cancellationToken: ct);
    }

    private static List<ClienteDocument> DatosIniciales() =>
    [
        CrearCliente("Cliente Uno", "cliente1@test.com",
            CrearFactura("F-001", 150000, "2026-09-01", "primerrecordatorio"),
            CrearFactura("F-002", 80000, "2026-10-01", "pendiente")),

        CrearCliente("Cliente Dos", "cliente2@test.com",
            CrearFactura("F-003", 320000, "2026-08-15", "segundorecordatorio"),
            CrearFactura("F-004", 45000, "2026-09-20", "primerrecordatorio")),

        CrearCliente("Cliente Tres", "cliente3@test.com",
            CrearFactura("F-005", 210000, "2026-07-30", "segundorecordatorio"),
            CrearFactura("F-006", 99000, "2026-10-10", "pendiente")),
    ];

    private static ClienteDocument CrearCliente(string nombre, string email, params FacturaDocument[] facturas) =>
        new() { Nombre = nombre, Email = email, Facturas = [.. facturas] };

    private static FacturaDocument CrearFactura(string numero, decimal valor, string fecha, string estado) =>
        new()
        {
            Numero = numero,
            Valor = valor,
            FechaVencimiento = DateTime.SpecifyKind(
                DateTime.Parse(fecha, CultureInfo.InvariantCulture), DateTimeKind.Utc),
            Estado = estado
        };
}
