namespace DanmaobCfdi.Core.Reading;

public sealed class UnreadComplement
{
    internal UnreadComplement(string namespaceName, string localName)
    {
        NamespaceName = namespaceName;
        LocalName = localName;
    }

    public string NamespaceName { get; }

    public string LocalName { get; }
}
