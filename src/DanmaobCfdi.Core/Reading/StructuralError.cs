namespace DanmaobCfdi.Core.Reading;

public sealed class StructuralError
{
    internal StructuralError(int lineNumber, int linePosition, string message)
    {
        LineNumber = lineNumber;
        LinePosition = linePosition;
        Message = message;
    }

    public int LineNumber { get; }

    public int LinePosition { get; }

    public string Message { get; }
}
