namespace denModels.MarketplaceServices;

public class UpdateQuantityResponse
{
    public UpdateQuantityStatus Status { get; set; }
    public string ItemNumber { get; set; }
    public ServerResponse Response { get; set; }

}