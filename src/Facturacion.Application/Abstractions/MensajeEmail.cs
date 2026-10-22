namespace Facturacion.Application.Abstractions;

public sealed record MensajeEmail(string Para, string Asunto, string CuerpoHtml);
