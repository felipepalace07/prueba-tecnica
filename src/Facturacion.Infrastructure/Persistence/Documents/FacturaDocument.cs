using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Facturacion.Infrastructure.Persistence.Documents;

public sealed class FacturaDocument
{
    [BsonElement("numero")]
    public string Numero { get; set; } = string.Empty;

    [BsonElement("valor")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal Valor { get; set; }

    [BsonElement("fechaVencimiento")]
    public DateTime FechaVencimiento { get; set; }

    [BsonElement("estado")]
    public string Estado { get; set; } = string.Empty;
}
