using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Facturacion.Infrastructure.Persistence.Documents;

public sealed class ClienteDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("facturas")]
    public List<FacturaDocument> Facturas { get; set; } = [];
}
