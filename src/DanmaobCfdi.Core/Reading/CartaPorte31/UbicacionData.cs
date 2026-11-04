using System;

namespace DanmaobCfdi.Core.Reading.CartaPorte31;

public sealed class UbicacionData
{
    public string TipoUbicacion { get; internal init; } = string.Empty;
    public string? IDUbicacion { get; internal init; }
    public string RFCRemitenteDestinatario { get; internal init; } = string.Empty;
    public string? NombreRemitenteDestinatario { get; internal init; }
    public string? NumRegIdTrib { get; internal init; }
    public string? ResidenciaFiscal { get; internal init; }
    public DateTime FechaHoraSalidaLlegada { get; internal init; }
    public decimal? DistanciaRecorrida { get; internal init; }
    public DomicilioData? Domicilio { get; internal init; }
}
