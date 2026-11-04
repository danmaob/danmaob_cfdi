namespace DanmaobCfdi.Core.Catalogs;

public enum SatAddressIssue
{
    EstadoNotFound,
    CodigoPostalNotFound,
    CodigoPostalEstadoMismatch,
    CodigoPostalMunicipioMismatch,
    CodigoPostalLocalidadMismatch,
    MunicipioNotFound,
    LocalidadNotFound,
    ColoniaNotFound
}
