namespace Facturacion.Infrastructure.Configuration;

public sealed class SmtpSettings
{
    public const string Seccion = "Smtp";

    public string Host { get; init; } = "localhost";
    public int Port { get; init; } = 1025;
    public bool UsarSsl { get; init; }
    public string? Usuario { get; init; }
    public string? Clave { get; init; }
    public string Remitente { get; init; } = "facturacion@empresa.com";
    public string NombreRemitente { get; init; } = "Facturación";
}
