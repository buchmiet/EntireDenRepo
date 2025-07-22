using DataServicesNET80.Models;

namespace denSharedLibrary;

public interface ICurrentKomplety
{
    Dictionary<int, Complete> CompletesDict { get; }
    Dictionary<int, int> retCusts { get; set; }

    event EventHandler<CompletesChangedEventArgs> CompletesChanged;
    Task<int> GetNumberOfOrders2Download();
    Task FetchKomplety(Dictionary<int, string> stareOrdy);
    Complete[] GetKomplety();
    Task OrdersNeedRefreshing(List<Complete> completes);
    void SetLocation(int locationId);
}