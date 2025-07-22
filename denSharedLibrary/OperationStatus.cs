namespace denSharedLibrary;

public enum OperationStatus
{
    Success,
    NoTextInClipboard,
    InvalidClipboardFormat,
    NoDatabaseConnection,
    OrderNotProcessed,
    PartialOrderNotProcessed,
    NoOrdersDownloadedForProcessing
}