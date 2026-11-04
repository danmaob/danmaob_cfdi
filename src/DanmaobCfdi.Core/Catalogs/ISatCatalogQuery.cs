using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DanmaobCfdi.Core.Catalogs;

public interface ISatCatalogQuery
{
    public Task<SatCatalogEntry?> FindAsync(SatCatalog catalog, string clave, string? clavePadre, CancellationToken cancellationToken);

    public Task<IReadOnlyList<SatCatalogEntry>> SearchAsync(SatCatalog catalog, string text, string? clavePadre, int limit, CancellationToken cancellationToken);

    public Task<IReadOnlyList<SatCatalogEntry>> ListAsync(SatCatalog catalog, CancellationToken cancellationToken);

    public Task<SatCatalogValidity> CheckValidityAsync(SatCatalog catalog, string clave, string? clavePadre, DateOnly date, CancellationToken cancellationToken);

    public Task<SatCodigoPostalEntry?> FindCodigoPostalAsync(string clave, CancellationToken cancellationToken);
}
