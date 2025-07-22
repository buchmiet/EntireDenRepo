namespace denEbayNET80;

public class ebayOrder2
{

    public class Rootobject
    {
        public string href { get; set; }
        public int total { get; set; }
        public string next { get; set; }
        public int limit { get; set; }
        public int offset { get; set; }
        public Order[] orders { get; set; }
    }

    public class Order
    {
        public string orderId { get; set; }
        public string legacyOrderId { get; set; }
        public DateTime creationDate { get; set; }
        public DateTime lastModifiedDate { get; set; }
        public string orderFulfillmentStatus { get; set; }
        public string orderPaymentStatus { get; set; }
        public string sellerId { get; set; }
        public Buyer buyer { get; set; }
        public Pricingsummary pricingSummary { get; set; }
        public Cancelstatus cancelStatus { get; set; }
        public Paymentsummary paymentSummary { get; set; }
        public Fulfillmentstartinstruction[] fulfillmentStartInstructions { get; set; }
        public string[] fulfillmentHrefs { get; set; }
        public Lineitem[] lineItems { get; set; }
        public string salesRecordReference { get; set; }
        public bool ebayCollectAndRemitTax { get; set; }
        public string buyerCheckoutNotes { get; set; }
    }

    public class Buyer
    {
        public string username { get; set; }
    }

    public class Pricingsummary
    {
        public Pricesubtotal priceSubtotal { get; set; }
        public Deliverycost deliveryCost { get; set; }
        public Total total { get; set; }
        public Deliverydiscount deliveryDiscount { get; set; }
        public Tax tax { get; set; }
    }

    public class Pricesubtotal
    {
        public string value { get; set; }
        public string currency { get; set; }
        public string convertedFromValue { get; set; }
        public string convertedFromCurrency { get; set; }
    }

    public class Deliverycost
    {
        public string value { get; set; }
        public string currency { get; set; }
        public string convertedFromValue { get; set; }
        public string convertedFromCurrency { get; set; }
    }

    public class Total
    {
        public string value { get; set; }
        public string currency { get; set; }
        public string convertedFromValue { get; set; }
        public string convertedFromCurrency { get; set; }
    }

    public class Deliverydiscount
    {
        public string value { get; set; }
        public string currency { get; set; }
    }

    public class Tax
    {
        public string value { get; set; }
        public string currency { get; set; }
    }

    public class Cancelstatus
    {
        public string cancelState { get; set; }
        public Cancelrequest[] cancelRequests { get; set; }
        public DateTime cancelledDate { get; set; }
    }

    public class Cancelrequest
    {
        public DateTime cancelRequestedDate { get; set; }
        public DateTime cancelCompletedDate { get; set; }
        public string cancelInitiator { get; set; }
        public string cancelRequestId { get; set; }
        public string cancelRequestState { get; set; }
    }

    public class Paymentsummary
    {
        public Totaldueseller totalDueSeller { get; set; }
        public Refund[] refunds { get; set; }
        public Payment[] payments { get; set; }
    }

    public class Totaldueseller
    {
        public string value { get; set; }
        public string currency { get; set; }
        public string convertedFromValue { get; set; }
        public string convertedFromCurrency { get; set; }
    }

    public class Refund
    {
        public DateTime refundDate { get; set; }
        public Amount amount { get; set; }
        public string refundStatus { get; set; }
        public string refundReferenceId { get; set; }
    }

    public class Amount
    {
        public string value { get; set; }
        public string currency { get; set; }
        public string convertedFromValue { get; set; }
        public string convertedFromCurrency { get; set; }
    }

    public class Payment
    {
        public string paymentMethod { get; set; }
        public string paymentReferenceId { get; set; }
        public DateTime paymentDate { get; set; }
        public Amount1 amount { get; set; }
        public string paymentStatus { get; set; }
        public Paymenthold[] paymentHolds { get; set; }
    }

    public class Amount1
    {
        public string value { get; set; }
        public string currency { get; set; }
        public string convertedFromValue { get; set; }
        public string convertedFromCurrency { get; set; }
    }

    public class Paymenthold
    {
        public string holdReason { get; set; }
        public Holdamount holdAmount { get; set; }
        public string holdState { get; set; }
        public DateTime releaseDate { get; set; }
    }

    public class Holdamount
    {
        public string value { get; set; }
        public string currency { get; set; }
        public string convertedFromValue { get; set; }
        public string convertedFromCurrency { get; set; }
    }

    public class Fulfillmentstartinstruction
    {
        public string fulfillmentInstructionsType { get; set; }
        public DateTime minEstimatedDeliveryDate { get; set; }
        public DateTime maxEstimatedDeliveryDate { get; set; }
        public bool ebaySupportedFulfillment { get; set; }
        public Shippingstep shippingStep { get; set; }
    }

    public class Shippingstep
    {
        public Shipto shipTo { get; set; }
        public string shippingCarrierCode { get; set; }
        public string shippingServiceCode { get; set; }
    }

    public class Shipto
    {
        public string fullName { get; set; }
        public Contactaddress contactAddress { get; set; }
        public Primaryphone primaryPhone { get; set; }
        public string email { get; set; }
    }

    public class Contactaddress
    {
        public string addressLine1 { get; set; }
        public string city { get; set; }
        public string stateOrProvince { get; set; }
        public string postalCode { get; set; }
        public string countryCode { get; set; }
        public string addressLine2 { get; set; }
    }

    public class Primaryphone
    {
        public string phoneNumber { get; set; }
    }

    public class Lineitem
    {
        public string lineItemId { get; set; }
        public string legacyItemId { get; set; }
        public string legacyVariationId { get; set; }
        public string sku { get; set; }
        public string title { get; set; }
        public Lineitemcost lineItemCost { get; set; }
        public int quantity { get; set; }
        public string soldFormat { get; set; }
        public string listingMarketplaceId { get; set; }
        public string purchaseMarketplaceId { get; set; }
        public string lineItemFulfillmentStatus { get; set; }
        public Total1 total { get; set; }
        public Deliverycost1 deliveryCost { get; set; }
        public object[] appliedPromotions { get; set; }
        public Tax1[] taxes { get; set; }
        public Properties properties { get; set; }
        public Lineitemfulfillmentinstructions lineItemFulfillmentInstructions { get; set; }
        public Ebaycollectandremittax[] ebayCollectAndRemitTaxes { get; set; }
        public Refund1[] refunds { get; set; }
    }

    public class Lineitemcost
    {
        public string value { get; set; }
        public string currency { get; set; }
        public string convertedFromValue { get; set; }
        public string convertedFromCurrency { get; set; }
    }

    public class Total1
    {
        public string value { get; set; }
        public string currency { get; set; }
        public string convertedFromValue { get; set; }
        public string convertedFromCurrency { get; set; }
    }

    public class Deliverycost1
    {
        public Shippingcost shippingCost { get; set; }
        public Discountamount discountAmount { get; set; }
    }

    public class Shippingcost
    {
        public string value { get; set; }
        public string currency { get; set; }
        public string convertedFromValue { get; set; }
        public string convertedFromCurrency { get; set; }
    }

    public class Discountamount
    {
        public string value { get; set; }
        public string currency { get; set; }
    }

    public class Properties
    {
        public bool buyerProtection { get; set; }
        public bool soldViaAdCampaign { get; set; }
    }

    public class Lineitemfulfillmentinstructions
    {
        public DateTime minEstimatedDeliveryDate { get; set; }
        public DateTime maxEstimatedDeliveryDate { get; set; }
        public DateTime shipByDate { get; set; }
        public bool guaranteedDelivery { get; set; }
    }

    public class Tax1
    {
        public Amount2 amount { get; set; }
    }

    public class Amount2
    {
        public string value { get; set; }
        public string currency { get; set; }
    }

    public class Ebaycollectandremittax
    {
        public string taxType { get; set; }
        public Amount3 amount { get; set; }
        public string collectionMethod { get; set; }
    }

    public class Amount3
    {
        public string value { get; set; }
        public string currency { get; set; }
    }

    public class Refund1
    {
        public DateTime refundDate { get; set; }
        public Amount4 amount { get; set; }
    }

    public class Amount4
    {
        public string value { get; set; }
        public string currency { get; set; }
    }



}