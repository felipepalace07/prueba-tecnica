namespace Facturacion.Application.Abstractions;

public interface IInicializadorDatos
{
    Task SembrarSiVacioAsync(CancellationToken ct = default);

    Task ReiniciarAsync(CancellationToken ct = default);
}
