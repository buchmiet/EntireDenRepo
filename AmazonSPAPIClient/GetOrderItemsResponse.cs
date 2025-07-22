namespace AmazonSPAPIClient;

public class GetOrderItemsResponse : IHasNextToken
{

    public void Merge(IHasNextToken anotherResponse)
    {
        GetOrderItemsResponse anotherRootObj = anotherResponse as GetOrderItemsResponse;
        if (anotherRootObj?.payload?.OrderItems != null && anotherRootObj.payload.OrderItems.Length > 0)
        {
            this.payload.OrderItems = this.payload.OrderItems.Concat(anotherRootObj.payload.OrderItems).ToArray();
        }

        // Dodajemy aktualizację NextToken
        this.payload.NextToken = anotherRootObj?.payload?.NextToken;
    }

    public string GetNextToken()
    {
        return payload?.NextToken;
    }


    public class Rootobject
    {
        public Payload payload { get; set; }
    }
    public Payload payload { get; set; }
    public class Payload
    {
        public Orderitem[] OrderItems { get; set; }
        public string AmazonOrderId { get; set; }
        public string NextToken { get; set; }
    }

    public class Orderitem
    {
        public Productinfo ProductInfo { get; set; }
        public string IossNumber { get; set; }
        public Buyerinfo BuyerInfo { get; set; }
        public Itemtax ItemTax { get; set; }
        public int QuantityShipped { get; set; }
        public Buyerrequestedcancel BuyerRequestedCancel { get; set; }
        public Itemprice ItemPrice { get; set; }
        public string ASIN { get; set; }
        public string SellerSKU { get; set; }
        public string Title { get; set; }
        public Shippingtax ShippingTax { get; set; }
        public string IsGift { get; set; }
        public Shippingprice ShippingPrice { get; set; }
        public string ConditionSubtypeId { get; set; }
        public Shippingdiscount ShippingDiscount { get; set; }
        public Shippingdiscounttax ShippingDiscountTax { get; set; }
        public bool IsTransparency { get; set; }
        public int QuantityOrdered { get; set; }
        public Promotiondiscounttax PromotionDiscountTax { get; set; }
        public string ConditionId { get; set; }
        public Promotiondiscount PromotionDiscount { get; set; }
        public string OrderItemId { get; set; }
        public string DeemedResellerCategory { get; set; }
    }

    public class Productinfo
    {
        public string NumberOfItems { get; set; }
    }

    public class Buyerinfo
    {
    }

    public class Itemtax
    {
        public string CurrencyCode { get; set; }
        public string Amount { get; set; }
    }

    public class Buyerrequestedcancel
    {
        public string IsBuyerRequestedCancel { get; set; }
        public string BuyerCancelReason { get; set; }
    }

    public class Itemprice
    {
        public string CurrencyCode { get; set; }
        public string Amount { get; set; }
    }

    public class Shippingtax
    {
        public string CurrencyCode { get; set; }
        public string Amount { get; set; }
    }

    public class Shippingprice
    {
        public string CurrencyCode { get; set; }
        public string Amount { get; set; }
    }

    public class Shippingdiscount
    {
        public string CurrencyCode { get; set; }
        public string Amount { get; set; }
    }

    public class Shippingdiscounttax
    {
        public string CurrencyCode { get; set; }
        public string Amount { get; set; }
    }

    public class Promotiondiscounttax
    {
        public string CurrencyCode { get; set; }
        public string Amount { get; set; }
    }

    public class Promotiondiscount
    {
        public string CurrencyCode { get; set; }
        public string Amount { get; set; }
    }

}