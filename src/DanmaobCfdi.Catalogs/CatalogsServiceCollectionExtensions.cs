using System;
using DanmaobCfdi.Catalogs.Persistence;
using DanmaobCfdi.Catalogs.Query;
using DanmaobCfdi.Core.Catalogs;
using Microsoft.Extensions.DependencyInjection;

namespace DanmaobCfdi.Catalogs;

public static class CatalogsServiceCollectionExtensions
{
    public static IServiceCollection AddDanmaobCfdiCatalogs(this IServiceCollection services, string connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
        services.AddDbContext<SatDbContext>(builder => SatDbContextOptions.Configure(builder, connectionString));
        services.AddScoped<ISatCatalogQuery, SatCatalogQuery>();
        return services;
    }
}
