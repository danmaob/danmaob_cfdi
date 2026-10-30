using System;
using System.Globalization;
using System.Xml.Linq;

namespace DanmaobCfdi.Core.Reading;

internal static class XmlAttributeReader
{
    private const NumberStyles DecimalStyles = NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign;
    private const string DateTimeFormat = "yyyy-MM-dd'T'HH:mm:ss";

    internal static string Required(XElement element, string name)
    {
        XAttribute? attribute = element.Attribute(name);
        if (attribute is null)
        {
            return string.Empty;
        }
        return attribute.Value;
    }

    internal static string? Optional(XElement element, string name)
    {
        XAttribute? attribute = element.Attribute(name);
        if (attribute is null)
        {
            return null;
        }
        return attribute.Value;
    }

    internal static decimal RequiredDecimal(XElement element, string name)
    {
        return decimal.Parse(Required(element, name), DecimalStyles, CultureInfo.InvariantCulture);
    }

    internal static decimal? OptionalDecimal(XElement element, string name)
    {
        string? value = Optional(element, name);
        if (value is null)
        {
            return null;
        }
        return decimal.Parse(value, DecimalStyles, CultureInfo.InvariantCulture);
    }

    internal static DateTime RequiredDateTime(XElement element, string name)
    {
        return DateTime.ParseExact(Required(element, name), DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None);
    }
}
