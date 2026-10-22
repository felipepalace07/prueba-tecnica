using Facturacion.Domain.Enums;

namespace Facturacion.Domain.Exceptions;

public sealed class TransicionEstadoInvalidaException(string numeroFactura, EstadoFactura estadoActual)
    : Exception($"La factura {numeroFactura} en estado {estadoActual} no admite avanzar de estado.");
