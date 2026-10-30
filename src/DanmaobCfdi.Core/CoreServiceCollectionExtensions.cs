using DanmaobCfdi.Core.Reading;
using DanmaobCfdi.Core.Reading.Cfdi40;
using DanmaobCfdi.Core.Resources;
using Microsoft.Extensions.DependencyInjection;

namespace DanmaobCfdi.Core;

public static class CoreServiceCollectionExtensions
{
    public static IServiceCollection AddDanmaobCfdiCore(this IServiceCollection services)
    {
        services.AddSingleton<SatResourceResolver>();
        services.AddSingleton<SatCompiledResources>();
        services.AddSingleton<SecureXmlLoader>();
        services.AddSingleton<DocumentDetector>();
        services.AddSingleton<XmlStructuralValidator>();
        services.AddSingleton<Cfdi40Extractor>();
        services.AddSingleton<IThirdPartyXmlReader, ThirdPartyXmlReader>();
        return services;
    }
}
