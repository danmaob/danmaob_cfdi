using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using DanmaobCfdi.Core.Reading;
using Xunit;

namespace DanmaobCfdi.Tests.Core.Reading;

public sealed class SecureXmlLoaderTests
{
    [Fact]
    public void Load_ReferenceCfdi_ReturnsDocumentWithLineInfo()
    {
        byte[] content = File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Fixtures", "Cfdi40", "ingreso-basico.xml"));
        SecureXmlLoader loader = new SecureXmlLoader();
        SecureXmlLoadResult result = loader.Load(content);
        Assert.True(result.IsLoaded);
        Assert.Null(result.Reason);
        XDocument? document = result.Document;
        Assert.NotNull(document);
        XElement? root = document.Root;
        Assert.NotNull(root);
        Assert.Equal("Comprobante", root.Name.LocalName);
        IXmlLineInfo lineInfo = root;
        Assert.True(lineInfo.HasLineInfo());
        Assert.Equal(2, lineInfo.LineNumber);
    }

    [Theory]
    [InlineData("con-dtd.xml", RejectionReason.DtdOrEntity)]
    [InlineData("entidad-externa.xml", RejectionReason.DtdOrEntity)]
    [InlineData("mal-formado.xml", RejectionReason.Malformed)]
    [InlineData("declara-iso-8859-1.xml", RejectionReason.InvalidEncoding)]
    [InlineData("utf8-invalido.xml", RejectionReason.InvalidEncoding)]
    [InlineData("utf16.xml", RejectionReason.InvalidEncoding)]
    public void Load_InvalidFixture_ReturnsExpectedRejection(string fileName, RejectionReason expected)
    {
        byte[] content = File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Fixtures", "Invalidos", fileName));
        SecureXmlLoader loader = new SecureXmlLoader();
        SecureXmlLoadResult result = loader.Load(content);
        Assert.False(result.IsLoaded);
        Assert.Null(result.Document);
        Assert.Equal(expected, result.Reason);
    }

    [Fact]
    public void Load_EmptyContent_ReturnsMalformed()
    {
        SecureXmlLoader loader = new SecureXmlLoader();
        SecureXmlLoadResult result = loader.Load(Array.Empty<byte>());
        Assert.False(result.IsLoaded);
        Assert.Equal(RejectionReason.Malformed, result.Reason);
    }

    [Fact]
    public void Load_ContentAtSizeLimit_ReturnsDocument()
    {
        string xml = "<a>" + new string(' ', 10485753) + "</a>";
        byte[] content = Encoding.UTF8.GetBytes(xml);
        Assert.Equal(10485760, content.Length);
        SecureXmlLoader loader = new SecureXmlLoader();
        SecureXmlLoadResult result = loader.Load(content);
        Assert.True(result.IsLoaded);
    }

    [Fact]
    public void Load_ContentOverSizeLimit_ReturnsTooLarge()
    {
        byte[] content = new byte[10485761];
        SecureXmlLoader loader = new SecureXmlLoader();
        SecureXmlLoadResult result = loader.Load(content);
        Assert.False(result.IsLoaded);
        Assert.Equal(RejectionReason.TooLarge, result.Reason);
    }

    [Fact]
    public void Load_SixtyFourLevels_ReturnsDocument()
    {
        byte[] content = CreateNestedXml(64);
        SecureXmlLoader loader = new SecureXmlLoader();
        SecureXmlLoadResult result = loader.Load(content);
        Assert.True(result.IsLoaded);
    }

    [Fact]
    public void Load_SixtyFiveLevels_ReturnsTooDeep()
    {
        byte[] content = CreateNestedXml(65);
        SecureXmlLoader loader = new SecureXmlLoader();
        SecureXmlLoadResult result = loader.Load(content);
        Assert.False(result.IsLoaded);
        Assert.Equal(RejectionReason.TooDeep, result.Reason);
    }

    [Fact]
    public void Load_LowercaseUtf8Declaration_ReturnsDocument()
    {
        byte[] content = Encoding.UTF8.GetBytes("<?xml version='1.0' encoding='utf-8'?><a/>");
        SecureXmlLoader loader = new SecureXmlLoader();
        SecureXmlLoadResult result = loader.Load(content);
        Assert.True(result.IsLoaded);
    }

    private static byte[] CreateNestedXml(int levels)
    {
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < levels; i++)
        {
            builder.Append("<a>");
        }
        for (int i = 0; i < levels; i++)
        {
            builder.Append("</a>");
        }
        return Encoding.UTF8.GetBytes(builder.ToString());
    }
}
