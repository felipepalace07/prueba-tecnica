namespace Facturacion.Infrastructure.Configuration;

public sealed class MongoSettings
{
    public const string Seccion = "Mongo";

    public string ConnectionString { get; init; } = string.Empty;
    public string BaseDatos { get; init; } = "facturacion";
    public string ColeccionClientes { get; init; } = "clientes";
}
