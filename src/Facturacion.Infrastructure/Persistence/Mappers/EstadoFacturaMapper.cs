using Facturacion.Domain.Enums;

namespace Facturacion.Infrastructure.Persistence.Mappers;

/// <summary>
/// Traduce el enum del dominio a los valores de texto que exige la base de datos
/// ("primerrecordatorio", "segundorecordatorio", "desactivado").
/// </summary>
internal static class EstadoFacturaMapper
{
    private static readonly Dictionary<EstadoFactura, string> ATextoMap = new()
    {
        [EstadoFactura.Pendiente] = "pendiente",
        [EstadoFactura.PrimerRecordatorio] = "primerrecordatorio",
        [EstadoFactura.SegundoRecordatorio] = "segundorecordatorio",
        [EstadoFactura.Desactivado] = "desactivado",
    };

    private static readonly Dictionary<string, EstadoFactura> DesdeTextoMap =
        ATextoMap.ToDictionary(par => par.Value, par => par.Key);

    public static string ATexto(EstadoFactura estado) => ATextoMap[estado];

    public static EstadoFactura DesdeTexto(string valor) =>
        DesdeTextoMap.TryGetValue(valor.Trim().ToLowerInvariant(), out var estado)
            ? estado
            : throw new InvalidOperationException($"Estado desconocido en la base de datos: '{valor}'.");
}
