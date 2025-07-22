using DataServicesNET80.Models;

namespace denSharedLibrary;

public interface ICompletesToXpsStream
{
    Task<MemoryStream> GenerateStream(List<Complete> Komplety, InvoicePrintoutDataPack invoicePrintoutDataPack);
}