using System;
using System.Collections.Generic;

namespace DanmaobCfdi.Core.Reading.CartaPorte31;

public sealed class AutotransporteData
{
    public string PermSCT { get; internal init; } = string.Empty;
    public string NumPermisoSCT { get; internal init; } = string.Empty;
    public IdentificacionVehicularData IdentificacionVehicular { get; internal init; } = new IdentificacionVehicularData();
    public SegurosData Seguros { get; internal init; } = new SegurosData();
    public IReadOnlyList<RemolqueData> Remolques { get; internal init; } = Array.Empty<RemolqueData>();
}
