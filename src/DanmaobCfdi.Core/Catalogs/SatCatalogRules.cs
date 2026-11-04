using System;

namespace DanmaobCfdi.Core.Catalogs;

public static class SatCatalogRules
{
    public const int MinSearchTextLength = 3;
    public const int DefaultSearchLimit = 20;
    public const int MaxSearchLimit = 50;
    private static readonly SatCatalog[] ParentKeyCatalogs = new SatCatalog[] { SatCatalog.Colonia, SatCatalog.Estado, SatCatalog.Municipio, SatCatalog.Localidad };

    public static void ValidateSearch(string text, int limit)
    {
        ArgumentNullException.ThrowIfNull(text);
        if (text.Trim().Length < MinSearchTextLength)
        {
            throw new ArgumentException("The search text must have at least 3 characters.", nameof(text));
        }
        ArgumentOutOfRangeException.ThrowIfLessThan(limit, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(limit, MaxSearchLimit);
    }

    public static bool HasParentKey(SatCatalog catalog)
    {
        return Array.IndexOf(ParentKeyCatalogs, catalog) >= 0;
    }
}
