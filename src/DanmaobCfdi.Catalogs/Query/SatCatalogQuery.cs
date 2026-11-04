using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DanmaobCfdi.Catalogs.Persistence;
using DanmaobCfdi.Core.Catalogs;
using Microsoft.EntityFrameworkCore;

namespace DanmaobCfdi.Catalogs.Query;

internal sealed class SatCatalogQuery : ISatCatalogQuery
{
    private readonly SatDbContext _context;

    public SatCatalogQuery(SatDbContext context)
    {
        _context = context;
    }

    public async Task<SatCatalogEntry?> FindAsync(SatCatalog catalog, string clave, string? clavePadre, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(clave);
        if (SatCatalogRules.HasParentKey(catalog) && clavePadre is null)
        {
            throw new ArgumentException("This catalog has a composite key: the parent key is required.", nameof(clavePadre));
        }

        IQueryable<SatCatalogEntity> query = Filtered(catalog, clavePadre).Where(entity => entity.Clave == clave);
        SatCatalogEntry? found = await Project(catalog, query).FirstOrDefaultAsync(cancellationToken);
        return found;
    }

    public async Task<IReadOnlyList<SatCatalogEntry>> SearchAsync(SatCatalog catalog, string text, string? clavePadre, int limit, CancellationToken cancellationToken)
    {
        SatCatalogRules.ValidateSearch(text, limit);
        string trimmed = text.Trim();
        IQueryable<SatCatalogEntity> query = Filtered(catalog, clavePadre).Where(entity => entity.Clave.StartsWith(trimmed) || (entity.Descripcion != null && entity.Descripcion.Contains(trimmed)));
        List<SatCatalogEntry> found = await Project(catalog, query.OrderBy(entity => entity.Clave).Take(limit)).ToListAsync(cancellationToken);
        return found;
    }

    public async Task<IReadOnlyList<SatCatalogEntry>> ListAsync(SatCatalog catalog, CancellationToken cancellationToken)
    {
        if (!SatCatalogRules.IsSmall(catalog))
        {
            throw new InvalidOperationException("ListAsync is only for small catalogs.");
        }

        List<SatCatalogEntry> all = await Project(catalog, Filtered(catalog, null).OrderBy(entity => entity.Clave)).ToListAsync(cancellationToken);
        return all;
    }

    private IQueryable<SatCatalogEntity> Filtered(SatCatalog catalog, string? clavePadre)
    {
        IQueryable<SatCatalogEntity> source = SatCatalogSources.For(_context, catalog).AsNoTracking();
        string? parentColumn = SatCatalogSources.ParentColumn(catalog);
        if (clavePadre is null)
        {
            return source;
        }

        if (parentColumn is null)
        {
            return source;
        }

        string column = parentColumn;
        string parent = clavePadre;
        return source.Where(entity => EF.Property<string>(entity, column) == parent);
    }

    private static IQueryable<SatCatalogEntry> Project(SatCatalog catalog, IQueryable<SatCatalogEntity> query)
    {
        string? parentColumn = SatCatalogSources.ParentColumn(catalog);
        if (parentColumn is null)
        {
            return query.Select(entity => new SatCatalogEntry(entity.Clave, null, entity.Descripcion, entity.FechaInicioVigencia, entity.FechaFinVigencia));
        }

        string column = parentColumn;
        return query.Select(entity => new SatCatalogEntry(entity.Clave, EF.Property<string>(entity, column), entity.Descripcion, entity.FechaInicioVigencia, entity.FechaFinVigencia));
    }
}
