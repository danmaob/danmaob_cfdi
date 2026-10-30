using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using DanmaobCfdi.Core.Reading;
using DanmaobCfdi.Core.Resources;
using Xunit;

namespace DanmaobCfdi.Tests.Core.Reading;

public sealed class XmlStructuralValidatorTests
{
    private static readonly XmlStructuralValidator Shared = new XmlStructuralValidator(new SatCompiledResources(new SatResourceResolver()));

    [Fact]
    public void Validate_ReferenceCfdi_ReturnsNoErrors()
    {
        XDocument document = LoadFixture("Cfdi40", "ingreso-basico.xml");
        IReadOnlyList<StructuralError> errors = Shared.Validate(document);
        Assert.Empty(errors);
    }

    [Fact]
    public void Validate_UnknownComplementAndAddenda_ReturnsNoErrors()
    {
        XDocument document = LoadFixture("Cfdi40", "con-extensiones-desconocidas.xml");
        IReadOnlyList<StructuralError> errors = Shared.Validate(document);
        Assert.Empty(errors);
    }

    [Fact]
    public void Validate_AttributeOutsideSchema_ReturnsOneErrorWithLine()
    {
        XDocument document = LoadFixture("Invalidos", "atributo-fuera-del-xsd.xml");
        IReadOnlyList<StructuralError> errors = Shared.Validate(document);
        StructuralError error = Assert.Single(errors);
        Assert.Equal(3, error.LineNumber);
        Assert.True(error.LinePosition > 0);
        Assert.Contains("Inventado", error.Message);
    }

    [Fact]
    public void Validate_SeveralErrors_ReturnsAllWithLines()
    {
        XDocument document = LoadFixture("Invalidos", "varios-errores.xml");
        IReadOnlyList<StructuralError> errors = Shared.Validate(document);
        Assert.Equal(2, errors.Count);
        Assert.Equal(4, errors[0].LineNumber);
        Assert.Equal(6, errors[1].LineNumber);
    }

    [Fact]
    public void Validate_AttributeOutsideSchemaInTimbre_ReturnsOneError()
    {
        XDocument document = LoadFixture("Invalidos", "tfd-atributo-fuera-del-xsd.xml");
        IReadOnlyList<StructuralError> errors = Shared.Validate(document);
        StructuralError error = Assert.Single(errors);
        Assert.Equal(20, error.LineNumber);
        Assert.Contains("Inventado", error.Message);
    }

    [Fact]
    public void Validate_MoreErrorsThanLimit_ReturnsMaxErrors()
    {
        XDocument document = LoadFixture("Cfdi40", "ingreso-basico.xml");
        XElement? root = document.Root;
        Assert.NotNull(root);
        for (int i = 0; i < 150; i++)
        {
            root.SetAttributeValue("Inventado" + i, "1");
        }
        IReadOnlyList<StructuralError> errors = Shared.Validate(document);
        Assert.Equal(100, errors.Count);
    }

    private static XDocument LoadFixture(string folder, string fileName)
    {
        byte[] content = File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Fixtures", folder, fileName));
        SecureXmlLoader loader = new SecureXmlLoader();
        SecureXmlLoadResult result = loader.Load(content);
        XDocument? document = result.Document;
        Assert.NotNull(document);
        return document;
    }
}
