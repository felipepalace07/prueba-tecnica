using Facturacion.Application.Abstractions;
using Facturacion.Infrastructure.Configuration;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Facturacion.Infrastructure.Email;

public sealed class SmtpEmailSender(IOptions<SmtpSettings> options) : IEmailSender
{
    public async Task EnviarAsync(MensajeEmail mensaje, CancellationToken ct = default)
    {
        var config = options.Value;

        var mime = new MimeMessage();
        mime.From.Add(new MailboxAddress(config.NombreRemitente, config.Remitente));
        mime.To.Add(MailboxAddress.Parse(mensaje.Para));
        mime.Subject = mensaje.Asunto;
        mime.Body = new BodyBuilder { HtmlBody = mensaje.CuerpoHtml }.ToMessageBody();

        using var cliente = new SmtpClient();
        await cliente.ConnectAsync(config.Host, config.Port, ObtenerSeguridad(config), ct);

        if (!string.IsNullOrWhiteSpace(config.Usuario))
        {
            await cliente.AuthenticateAsync(config.Usuario, config.Clave, ct);
        }

        await cliente.SendAsync(mime, ct);
        await cliente.DisconnectAsync(true, ct);
    }

    private static SecureSocketOptions ObtenerSeguridad(SmtpSettings config) => config switch
    {
        { UsarSsl: false } => SecureSocketOptions.None,
        { Port: 465 } => SecureSocketOptions.SslOnConnect,
        _ => SecureSocketOptions.StartTls
    };
}
