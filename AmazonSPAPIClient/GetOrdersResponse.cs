using System.Text.Json.Serialization;


namespace AmazonSPAPIClient;

public class GetOrdersResponse : IHasNextToken
{
    [JsonPropertyName("payload")]
    public ResponsePayload Payload { get; set; }

    public void Merge(IHasNextToken anotherResponse)
    {
        var anotherRootObj = anotherResponse as GetOrdersResponse;
        if (anotherRootObj?.Payload?.Orders != null && anotherRootObj.Payload.Orders.Length > 0)
        {
            this.Payload.Orders = this.Payload.Orders.Concat(anotherRootObj.Payload.Orders).ToArray();
        }

        this.Payload.NextToken = anotherRootObj?.Payload?.NextToken;
    }

    public string GetNextToken()
    {
        return Payload?.NextToken;
    }

    public class ResponsePayload
    {
        [JsonPropertyName("Orders")]
        public Order[] Orders { get; set; }

        [JsonPropertyName("NextToken")]
        public string NextToken { get; set; }

        [JsonPropertyName("CreatedBefore")]
        public DateTime CreatedBefore { get; set; }
    }

    public class Order
    {
        [JsonPropertyName("BuyerInfo")]
        public BuyerInfo BuyerInfo { get; set; }

        [JsonPropertyName("AmazonOrderId")]
        public string AmazonOrderId { get; set; }

        [JsonPropertyName("EarliestDeliveryDate")]
        public DateTime EarliestDeliveryDate { get; set; }

        [JsonPropertyName("EarliestShipDate")]
        public DateTime EarliestShipDate { get; set; }

        [JsonPropertyName("SalesChannel")]
        public string SalesChannel { get; set; }

        [JsonPropertyName("AutomatedShippingSettings")]
        public AutomatedShippingSettings AutomatedShippingSettings { get; set; }

        [JsonPropertyName("OrderStatus")]
        public string OrderStatus { get; set; }

        [JsonPropertyName("NumberOfItemsShipped")]
        public int NumberOfItemsShipped { get; set; }

        [JsonPropertyName("OrderType")]
        public string OrderType { get; set; }

        [JsonPropertyName("IsPremiumOrder")]
        public bool IsPremiumOrder { get; set; }

        [JsonPropertyName("IsPrime")]
        public bool IsPrime { get; set; }

        [JsonPropertyName("FulfillmentChannel")]
        public string FulfillmentChannel { get; set; }

        [JsonPropertyName("NumberOfItemsUnshipped")]
        public int NumberOfItemsUnshipped { get; set; }

        [JsonPropertyName("HasRegulatedItems")]
        public bool HasRegulatedItems { get; set; }

        [JsonPropertyName("IsReplacementOrder")]
        public string IsReplacementOrder { get; set; }

        [JsonPropertyName("IsSoldByAB")]
        public bool IsSoldByAB { get; set; }

        [JsonPropertyName("LatestShipDate")]
        public DateTime LatestShipDate { get; set; }

        [JsonPropertyName("ShipServiceLevel")]
        public string ShipServiceLevel { get; set; }

        [JsonPropertyName("DefaultShipFromLocationAddress")]
        public DefaultShipFromLocationAddress DefaultShipFromLocationAddress { get; set; }

        [JsonPropertyName("IsISPU")]
        public bool IsISPU { get; set; }

        [JsonPropertyName("MarketplaceId")]
        public string MarketplaceId { get; set; }

        [JsonPropertyName("LatestDeliveryDate")]
        public DateTime LatestDeliveryDate { get; set; }

        [JsonPropertyName("PurchaseDate")]
        public DateTime PurchaseDate { get; set; }

        [JsonPropertyName("ShippingAddress")]
        public ShippingAddress ShippingAddress { get; set; }

        [JsonPropertyName("IsAccessPointOrder")]
        public bool IsAccessPointOrder { get; set; }

        [JsonPropertyName("PaymentMethod")]
        public string PaymentMethod { get; set; }

        [JsonPropertyName("IsBusinessOrder")]
        public bool IsBusinessOrder { get; set; }

        [JsonPropertyName("OrderTotal")]
        public OrderTotal OrderTotal { get; set; }

        [JsonPropertyName("PaymentMethodDetails")]
        public string[] PaymentMethodDetails { get; set; }

        [JsonPropertyName("IsGlobalExpressEnabled")]
        public bool IsGlobalExpressEnabled { get; set; }

        [JsonPropertyName("LastUpdateDate")]
        public DateTime LastUpdateDate { get; set; }

        [JsonPropertyName("ShipmentServiceLevelCategory")]
        public string ShipmentServiceLevelCategory { get; set; }
    }

    public class BuyerInfo
    {
        [JsonPropertyName("BuyerEmail")]
        public string BuyerEmail { get; set; }
    }

    public class AutomatedShippingSettings
    {
        [JsonPropertyName("HasAutomatedShippingSettings")]
        public bool HasAutomatedShippingSettings { get; set; }
    }

    public class DefaultShipFromLocationAddress
    {
        [JsonPropertyName("StateOrRegion")]
        public string StateOrRegion { get; set; }

        [JsonPropertyName("AddressLine1")]
        public string AddressLine1 { get; set; }

        [JsonPropertyName("PostalCode")]
        public string PostalCode { get; set; }

        [JsonPropertyName("City")]
        public string City { get; set; }

        [JsonPropertyName("CountryCode")]
        public string CountryCode { get; set; }

        [JsonPropertyName("Name")]
        public string Name { get; set; }
    }

    public class ShippingAddress
    {
        [JsonPropertyName("PostalCode")]
        public string PostalCode { get; set; }

        [JsonPropertyName("City")]
        public string City { get; set; }

        [JsonPropertyName("CountryCode")]
        public string CountryCode { get; set; }
        [JsonPropertyName("StateOrRegion")]
        public string StateOrRegion { get; set; }
    }

    public class OrderTotal
    {
        [JsonPropertyName("CurrencyCode")]
        public string CurrencyCode { get; set; }

        [JsonPropertyName("Amount")]
        public string Amount { get; set; }
    }
}



//public class GetOrdersResoponse : IHasNextToken
//{
//    public void Merge(IHasNextToken anotherResponse)
//    {
//        GetOrdersResoponse anotherRootObj = anotherResponse as GetOrdersResoponse;
//        if (anotherRootObj?.payload?.Orders != null && anotherRootObj.payload.Orders.Length > 0)
//        {
//            this.payload.Orders = this.payload.Orders.Concat(anotherRootObj.payload.Orders).ToArray();
//        }

//        // Dodajemy aktualizację NextToken
//        this.payload.NextToken = anotherRootObj?.payload?.NextToken;
//    }
//    public Payload payload { get; set; }
//    public class Rootobject
//    {
//        public Payload payload { get; set; }
//    }
//    public string GetNextToken()
//    {
//        return payload?.NextToken;
//    }
//    public class Payload
//    {
//        public Order[] Orders { get; set; }
//        public string NextToken { get; set; }
//        public DateTime CreatedBefore { get; set; }
//    }

//    public class Order
//    {
//        public Buyerinfo BuyerInfo { get; set; }
//        public string AmazonOrderId { get; set; }
//        public DateTime EarliestDeliveryDate { get; set; }
//        public DateTime EarliestShipDate { get; set; }
//        public string SalesChannel { get; set; }
//        public Automatedshippingsettings AutomatedShippingSettings { get; set; }
//        public string OrderStatus { get; set; }
//        public int NumberOfItemsShipped { get; set; }
//        public string OrderType { get; set; }
//        public bool IsPremiumOrder { get; set; }
//        public bool IsPrime { get; set; }
//        public string FulfillmentChannel { get; set; }
//        public int NumberOfItemsUnshipped { get; set; }
//        public bool HasRegulatedItems { get; set; }
//        public string IsReplacementOrder { get; set; }
//        public bool IsSoldByAB { get; set; }
//        public DateTime LatestShipDate { get; set; }
//        public string ShipServiceLevel { get; set; }
//        public Defaultshipfromlocationaddress DefaultShipFromLocationAddress { get; set; }
//        public bool IsISPU { get; set; }
//        public string MarketplaceId { get; set; }
//        public DateTime LatestDeliveryDate { get; set; }
//        public DateTime PurchaseDate { get; set; }
//        public Shippingaddress ShippingAddress { get; set; }
//        public bool IsAccessPointOrder { get; set; }
//        public string PaymentMethod { get; set; }
//        public bool IsBusinessOrder { get; set; }
//        public Ordertotal OrderTotal { get; set; }
//        public string[] PaymentMethodDetails { get; set; }
//        public bool IsGlobalExpressEnabled { get; set; }
//        public DateTime LastUpdateDate { get; set; }
//        public string ShipmentServiceLevelCategory { get; set; }
//    }

//    public class Buyerinfo
//    {
//        public string BuyerEmail { get; set; }
//    }

//    public class Automatedshippingsettings
//    {
//        public bool HasAutomatedShippingSettings { get; set; }
//        public string AutomatedShipMethodName { get; set; }
//        public string AutomatedCarrierName { get; set; }
//    }

//    public class Defaultshipfromlocationaddress
//    {
//        public string StateOrRegion { get; set; }
//        public string AddressLine1 { get; set; }
//        public string PostalCode { get; set; }
//        public string City { get; set; }
//        public string CountryCode { get; set; }
//        public string Name { get; set; }
//        public string Phone { get; set; }
//    }

//    public class Shippingaddress
//    {
//        public string StateOrRegion { get; set; }
//        public string PostalCode { get; set; }
//        public string City { get; set; }
//        public string CountryCode { get; set; }
//        public string County { get; set; }
//    }

//    public class Ordertotal
//    {
//        public string CurrencyCode { get; set; }
//        public string Amount { get; set; }
//    }

//}