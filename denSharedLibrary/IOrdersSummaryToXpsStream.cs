using DataServicesNET80.Models;

namespace denSharedLibrary;

public interface IOrdersSummaryToXpsStream
{
    Task<MemoryStream> GenerateStream(List<Complete> Komplety, SummaryPrintoutDataPack summaryPrintoutDataPack);
}