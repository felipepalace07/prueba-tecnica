namespace Facturacion.Domain.Entities;

public sealed class Cliente
{
    public Cliente(string id, string nombre, string email, IEnumerable<Factura> facturas)
    {
        Id = id;
        Nombre = nombre;
        Email = email;
        Facturas = facturas.ToList();
    }

    public string Id { get; }
    public string Nombre { get; }
    public string Email { get; }
    public IReadOnlyList<Factura> Facturas { get; }
}
