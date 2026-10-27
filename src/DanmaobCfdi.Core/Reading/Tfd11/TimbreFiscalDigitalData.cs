using System;

namespace DanmaobCfdi.Core.Reading.Tfd11;

public sealed class TimbreFiscalDigitalData
{
    public string Version { get; internal init; } = string.Empty;
    public string UUID { get; internal init; } = string.Empty;
    public DateTime FechaTimbrado { get; internal init; }
    public string RfcProvCertif { get; internal init; } = string.Empty;
    public string? Leyenda { get; internal init; }
    public string SelloCFD { get; internal init; } = string.Empty;
    public string NoCertificadoSAT { get; internal init; } = string.Empty;
    public string SelloSAT { get; internal init; } = string.Empty;
}
