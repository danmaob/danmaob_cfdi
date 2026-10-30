namespace DanmaobCfdi.Core.Reading;

public interface IThirdPartyXmlReader
{
    public XmlReadResult Read(byte[] content);
}
