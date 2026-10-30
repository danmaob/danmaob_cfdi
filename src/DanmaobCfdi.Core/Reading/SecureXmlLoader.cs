using System;
using System.IO;
using System.Text.Unicode;
using System.Xml;
using System.Xml.Linq;

namespace DanmaobCfdi.Core.Reading;

internal sealed class SecureXmlLoader
{
    internal const int MaxSizeInBytes = 10485760;
    internal const int MaxDepth = 64;

    public SecureXmlLoadResult Load(byte[] content)
    {
        if (content.Length > MaxSizeInBytes)
        {
            return SecureXmlLoadResult.Rejected(RejectionReason.TooLarge);
        }

        if (!Utf8.IsValid(content))
        {
            return SecureXmlLoadResult.Rejected(RejectionReason.InvalidEncoding);
        }

        try
        {
            RejectionReason? reason = ReadSafely(content);
            if (reason is not null)
            {
                return SecureXmlLoadResult.Rejected(reason.Value);
            }

            XDocument document = LoadDocument(content);
            return SecureXmlLoadResult.Loaded(document);
        }
        catch (XmlException)
        {
            if (content.AsSpan().IndexOf("<!DOCTYPE"u8) >= 0)
            {
                return SecureXmlLoadResult.Rejected(RejectionReason.DtdOrEntity);
            }

            return SecureXmlLoadResult.Rejected(RejectionReason.Malformed);
        }
    }

    private static RejectionReason? ReadSafely(byte[] content)
    {
        using (XmlReader reader = CreateReader(content))
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.XmlDeclaration)
                {
                    string? encoding = reader.GetAttribute("encoding");
                    if (encoding is not null && !string.Equals(encoding, "UTF-8", StringComparison.OrdinalIgnoreCase))
                    {
                        return RejectionReason.InvalidEncoding;
                    }
                }

                if (reader.NodeType == XmlNodeType.Element && reader.Depth >= MaxDepth)
                {
                    return RejectionReason.TooDeep;
                }
            }
        }

        return null;
    }

    private static XDocument LoadDocument(byte[] content)
    {
        using (XmlReader reader = CreateReader(content))
        {
            return XDocument.Load(reader, LoadOptions.SetLineInfo);
        }
    }

    private static XmlReader CreateReader(byte[] content)
    {
        XmlReaderSettings settings = new XmlReaderSettings();
        settings.DtdProcessing = DtdProcessing.Prohibit;
        settings.XmlResolver = null;
        settings.CloseInput = true;
        return XmlReader.Create(new MemoryStream(content, false), settings);
    }
}
