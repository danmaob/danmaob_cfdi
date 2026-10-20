using System;
using System.IO;
using System.Xml;

namespace DanmaobCfdi.Core.Resources;

internal sealed class SatResourceResolver : XmlResolver
{
    private const string SatHost = "www.sat.gob.mx";

    public override object? GetEntity(Uri absoluteUri, string? role, Type? ofObjectToReturn)
    {
        if (ofObjectToReturn is not null && ofObjectToReturn != typeof(Stream) && ofObjectToReturn != typeof(object))
        {
            throw new XmlException("The SAT resource resolver only returns streams.");
        }
        string resourceName = GetResourceName(absoluteUri);
        Stream? stream = typeof(SatResourceResolver).Assembly.GetManifestResourceStream(resourceName);
        if (stream is null)
        {
            throw new XmlException("The SAT address has no local resource: " + resourceName);
        }
        return stream;
    }

    internal static string GetResourceName(Uri absoluteUri)
    {
        if (!absoluteUri.IsAbsoluteUri)
        {
            throw new XmlException("Only absolute SAT addresses can be resolved.");
        }
        bool isHttp = absoluteUri.Scheme == Uri.UriSchemeHttp || absoluteUri.Scheme == Uri.UriSchemeHttps;
        if (!isHttp || !string.Equals(absoluteUri.Host, SatHost, StringComparison.OrdinalIgnoreCase))
        {
            throw new XmlException("Only SAT addresses can be resolved: " + absoluteUri.Scheme + "://" + absoluteUri.Host);
        }
        return absoluteUri.AbsolutePath.TrimStart('/');
    }
}
