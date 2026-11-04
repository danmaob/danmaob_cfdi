using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DanmaobCfdi.Core.Catalogs;

public sealed class SatAddressRelations
{
    private const string Mexico = "MEX";
    private static readonly string[] CountriesWithStates = new string[] { "MEX", "USA", "CAN" };
    private readonly ISatCatalogQuery _query;

    public SatAddressRelations(ISatCatalogQuery query)
    {
        ArgumentNullException.ThrowIfNull(query);
        _query = query;
    }

    public async Task<IReadOnlyList<SatAddressIssue>> CheckAsync(SatAddress address, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(address);
        List<SatAddressIssue> issues = new List<SatAddressIssue>();
        if (Array.IndexOf(CountriesWithStates, address.Pais) < 0)
        {
            return issues;
        }
        SatCatalogEntry? estado = await _query.FindAsync(SatCatalog.Estado, address.Estado, address.Pais, cancellationToken);
        if (estado is null)
        {
            issues.Add(SatAddressIssue.EstadoNotFound);
        }
        if (address.Pais != Mexico)
        {
            return issues;
        }
        await CheckCodigoPostalAsync(address, issues, cancellationToken);
        await CheckChildAsync(SatCatalog.Municipio, address.Municipio, address.Estado, SatAddressIssue.MunicipioNotFound, issues, cancellationToken);
        await CheckChildAsync(SatCatalog.Localidad, address.Localidad, address.Estado, SatAddressIssue.LocalidadNotFound, issues, cancellationToken);
        await CheckChildAsync(SatCatalog.Colonia, address.Colonia, address.CodigoPostal, SatAddressIssue.ColoniaNotFound, issues, cancellationToken);
        return issues;
    }

    private async Task CheckCodigoPostalAsync(SatAddress address, List<SatAddressIssue> issues, CancellationToken cancellationToken)
    {
        SatCodigoPostalEntry? codigoPostal = await _query.FindCodigoPostalAsync(address.CodigoPostal, cancellationToken);
        if (codigoPostal is null)
        {
            issues.Add(SatAddressIssue.CodigoPostalNotFound);
            return;
        }
        if (codigoPostal.Estado != address.Estado)
        {
            issues.Add(SatAddressIssue.CodigoPostalEstadoMismatch);
        }
        if (address.Municipio is not null && codigoPostal.Municipio != address.Municipio)
        {
            issues.Add(SatAddressIssue.CodigoPostalMunicipioMismatch);
        }
        if (address.Localidad is not null && codigoPostal.Localidad != address.Localidad)
        {
            issues.Add(SatAddressIssue.CodigoPostalLocalidadMismatch);
        }
    }

    private async Task CheckChildAsync(SatCatalog catalog, string? clave, string clavePadre, SatAddressIssue issue, List<SatAddressIssue> issues, CancellationToken cancellationToken)
    {
        if (clave is null)
        {
            return;
        }
        SatCatalogEntry? entry = await _query.FindAsync(catalog, clave, clavePadre, cancellationToken);
        if (entry is null)
        {
            issues.Add(issue);
        }
    }
}
