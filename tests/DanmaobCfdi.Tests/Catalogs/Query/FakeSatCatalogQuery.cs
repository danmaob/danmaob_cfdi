using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DanmaobCfdi.Core.Catalogs;

namespace DanmaobCfdi.Tests.Catalogs.Query;

public sealed class FakeSatCatalogQuery : ISatCatalogQuery
{
    private readonly Dictionary<SatCatalog, List<SatCatalogEntry>> _entries = new Dictionary<SatCatalog, List<SatCatalogEntry>>();

    public void Add(SatCatalog catalog, SatCatalogEntry entry)
    {
        if (!_entries.ContainsKey(catalog))
        {
            _entries[catalog] = new List<SatCatalogEntry>();
        }
        _entries[catalog].Add(entry);
    }

    public Task<SatCatalogEntry?> FindAsync(SatCatalog catalog, string clave, string? clavePadre, CancellationToken cancellationToken)
    {
        SatCatalogEntry? found = EntriesOf(catalog).FirstOrDefault(entry => entry.Clave == clave && entry.ClavePadre == clavePadre);
        return Task.FromResult<SatCatalogEntry?>(found);
    }

    public Task<IReadOnlyList<SatCatalogEntry>> SearchAsync(SatCatalog catalog, string text, string? clavePadre, int limit, CancellationToken cancellationToken)
    {
        SatCatalogRules.ValidateSearch(text, limit);
        string trimmed = text.Trim();
        List<SatCatalogEntry> found = EntriesOf(catalog).Where(entry => MatchesParent(entry, clavePadre)).Where(entry => MatchesText(entry, trimmed)).OrderBy(entry => entry.Clave, StringComparer.Ordinal).Take(limit).ToList();
        return Task.FromResult<IReadOnlyList<SatCatalogEntry>>(found);
    }

    public Task<IReadOnlyList<SatCatalogEntry>> ListAsync(SatCatalog catalog, CancellationToken cancellationToken)
    {
        if (!SatCatalogRules.IsSmall(catalog))
        {
            throw new InvalidOperationException("ListAsync is only for small catalogs.");
        }
        List<SatCatalogEntry> all = EntriesOf(catalog).OrderBy(entry => entry.Clave, StringComparer.Ordinal).ToList();
        return Task.FromResult<IReadOnlyList<SatCatalogEntry>>(all);
    }

    private IEnumerable<SatCatalogEntry> EntriesOf(SatCatalog catalog)
    {
        if (_entries.TryGetValue(catalog, out List<SatCatalogEntry>? list))
        {
            return list;
        }
        return Enumerable.Empty<SatCatalogEntry>();
    }

    private static bool MatchesParent(SatCatalogEntry entry, string? clavePadre)
    {
        if (clavePadre is null)
        {
            return true;
        }
        return entry.ClavePadre == clavePadre;
    }

    private static bool MatchesText(SatCatalogEntry entry, string text)
    {
        if (entry.Clave.StartsWith(text, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }
        return entry.Descripcion is not null && entry.Descripcion.Contains(text, StringComparison.OrdinalIgnoreCase);
    }
}
