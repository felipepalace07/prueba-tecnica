namespace Facturacion.Application.Abstractions;

public interface IEmailSender
{
    Task EnviarAsync(MensajeEmail mensaje, CancellationToken ct = default);
}
