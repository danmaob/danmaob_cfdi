using DanmaobCfdi.Core.Resources;
using Microsoft.Extensions.DependencyInjection;

namespace DanmaobCfdi.Core;

public static class CoreServiceCollectionExtensions
{
    public static IServiceCollection AddDanmaobCfdiCore(this IServiceCollection services)
    {
        services.AddSingleton<SatResourceResolver>();
        services.AddSingleton<SatCompiledResources>();
        return services;
    }
}
