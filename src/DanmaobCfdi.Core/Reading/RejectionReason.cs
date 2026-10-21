namespace DanmaobCfdi.Core.Reading;

public enum RejectionReason
{
    TooLarge = 1,
    DtdOrEntity = 2,
    TooDeep = 3,
    InvalidEncoding = 4,
    Malformed = 5
}
