namespace Facturacion.Application.Dtos;

public sealed record ResultadoProcesoDto(IReadOnlyList<ResultadoFacturaDto> Resultados)
{
    public int Procesadas => Resultados.Count;
    public int Exitosas => Resultados.Count(r => r.Exitoso);
    public int Fallidas => Resultados.Count(r => !r.Exitoso);
}
