using Newtonsoft.Json;

namespace AmazonSPAPIClient;

public class ListingsItemPatchRequest
{
    [JsonProperty("productType")]
    public string productType { get; set; }

    [JsonProperty("patches")]
    public List<PatchOperation> patches { get; set; }
}

public class PatchOperation
{
    [JsonProperty("op")]
    public string op { get; set; }

    [JsonProperty("path")]
    public string path { get; set; }

    [JsonProperty("value")]
    public List<FulfillmentAvailability> value { get; set; }
}

public class FulfillmentAvailability
{
    [JsonProperty("fulfillment_channel_code")]
    public string fulfillment_channel_code
    { get; set; }

    [JsonProperty("quantity")]
    public int quantity { get; set; }
}