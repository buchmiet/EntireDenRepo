namespace AmazonSPAPIClient;

internal class GetItemResponse
{


    public class Rootobject
    {
        public string sku { get; set; }
        public Summary[] summaries { get; set; }
    }

    public class Summary
    {
        public string marketplaceId { get; set; }
        public string asin { get; set; }
        public string productType { get; set; }
        public string conditionType { get; set; }
        public string[] status { get; set; }
        public string itemName { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime lastUpdatedDate { get; set; }
        public Mainimage mainImage { get; set; }
    }

    public class Mainimage
    {
        public string link { get; set; }
        public int height { get; set; }
        public int width { get; set; }
    }

}