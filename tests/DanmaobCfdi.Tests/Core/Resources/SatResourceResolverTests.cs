using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Xml;
using DanmaobCfdi.Core.Resources;
using Xunit;

namespace DanmaobCfdi.Tests.Core.Resources;

public sealed class SatResourceResolverTests
{
    [Fact]
    public void GetEntity_SatAddress_ReturnsEmbeddedResource()
    {
        SatResourceResolver resolver = new SatResourceResolver();
        using Stream stream = Assert.IsAssignableFrom<Stream>(resolver.GetEntity(new Uri("http://www.sat.gob.mx/sitio_internet/cfd/4/cfdv40.xsd"), null, typeof(Stream)));
        Assert.True(stream.Length > 0);
    }

    [Fact]
    public void GetEntity_OtherHost_ThrowsXmlException()
    {
        SatResourceResolver resolver = new SatResourceResolver();
        Uri address = new Uri("http://www.example.com/sitio_internet/cfd/4/cfdv40.xsd");
        Assert.Throws<XmlException>(() => resolver.GetEntity(address, null, typeof(Stream)));
    }

    [Fact]
    public void GetEntity_UnknownSatPath_ThrowsXmlException()
    {
        SatResourceResolver resolver = new SatResourceResolver();
        Uri address = new Uri("http://www.sat.gob.mx/sitio_internet/cfd/4/unknown.xsd");
        Assert.Throws<XmlException>(() => resolver.GetEntity(address, null, typeof(Stream)));
    }

    [Fact]
    public void EmbeddedResources_ComparedWithSha256Sums_AllMatch()
    {
        SatResourceResolver resolver = new SatResourceResolver();
        string[] lines = File.ReadAllLines(Path.Combine(AppContext.BaseDirectory, "SatResources", "SHA256SUMS"));
        Assert.Equal("# juego SAT 2026-10-16", lines[0]);
        HashSet<string> expectedNames = new HashSet<string>();
        for (int index = 1; index < lines.Length; index++)
        {
            string expectedHash = lines[index].Substring(0, 64);
            string name = lines[index].Substring(66);
            expectedNames.Add(name);
            using Stream stream = Assert.IsAssignableFrom<Stream>(resolver.GetEntity(new Uri("http://www.sat.gob.mx/" + name), null, typeof(Stream)));
            Assert.Equal(expectedHash, Convert.ToHexStringLower(SHA256.HashData(stream)));
        }
        Assert.Equal(60, expectedNames.Count);
        string[] embeddedNames = typeof(SatResourceResolver).Assembly.GetManifestResourceNames();
        Assert.Equal(expectedNames.Count, embeddedNames.Length);
        foreach (string embeddedName in embeddedNames)
        {
            Assert.True(expectedNames.Contains(embeddedName), "Unexpected embedded resource: " + embeddedName);
        }
    }
}
