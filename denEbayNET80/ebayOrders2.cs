namespace denEbayNET80;

public class ebayOrders2
{

    public class Rootobject
    {
        public string href { get; set; }
        public string limit { get; set; }
        public string next { get; set; }
        public string offset { get; set; }
        public Order[] orders { get; set; }
        public string prev { get; set; }
        public string total { get; set; }
        public Warning[] warnings { get; set; }
    }

    public class Order
    {
        public Buyer buyer { get; set; }
        public string buyerCheckoutNotes { get; set; }
        public Cancelstatus cancelStatus { get; set; }
        public string creationDate { get; set; }
        public string ebayCollectAndRemitTax { get; set; }
        public string[] fulfillmentHrefs { get; set; }
        public Fulfillmentstartinstruction[] fulfillmentStartInstructions { get; set; }
        public string lastModifiedDate { get; set; }
        public string legacyOrderId { get; set; }
        public Lineitem[] lineItems { get; set; }
        public string orderFulfillmentStatus { get; set; }
        public string orderId { get; set; }
        public string orderPaymentStatus { get; set; }
        public Paymentsummary paymentSummary { get; set; }
        public Pricingsummary pricingSummary { get; set; }
        public string salesRecordReference { get; set; }
        public string sellerId { get; set; }
    }

    public class Buyer
    {
        public Taxidentifier taxIdentifier { get; set; }
        public string username { get; set; }
    }

    public class Taxidentifier
    {
        public string taxpayerId { get; set; }
        public string taxIdentifierType { get; set; }
        public string issuingCountry { get; set; }
    }

    public class Cancelstatus
    {
        public string cancelledDate { get; set; }
        public Cancelrequest[] cancelRequests { get; set; }
        public string cancelState { get; set; }
    }

    public class Cancelrequest
    {
        public string cancelCompletedDate { get; set; }
        public string cancelInitiator { get; set; }
        public string cancelReason { get; set; }
        public string cancelRequestedDate { get; set; }
        public string cancelRequestId { get; set; }
        public string cancelRequestState { get; set; }
    }

    public class Paymentsummary
    {
        public Payment[] payments { get; set; }
        public Refund[] refunds { get; set; }
        public Totaldueseller totalDueSeller { get; set; }
    }

    public class Totaldueseller
    {
        public string convertedFromCurrency { get; set; }
        public string convertedFromValue { get; set; }
        public string currency { get; set; }
        public string value { get; set; }
    }

    public class Payment
    {
        public Amount amount { get; set; }
        public string paymentDate { get; set; }
        public Paymenthold[] paymentHolds { get; set; }
        public string paymentMethod { get; set; }
        public string paymentReferenceId { get; set; }
        public string paymentStatus { get; set; }
    }

    public class Amount
    {
        public string convertedFromCurrency { get; set; }
        public string convertedFromValue { get; set; }
        public string currency { get; set; }
        public string value { get; set; }
    }

    public class Paymenthold
    {
        public string expectedReleaseDate { get; set; }
        public Holdamount holdAmount { get; set; }
        public string holdReason { get; set; }
        public string holdState { get; set; }
        public string releaseDate { get; set; }
        public Selleractionstorelease[] sellerActionsToRelease { get; set; }
    }

    public class Holdamount
    {
        public string convertedFromCurrency { get; set; }
        public string convertedFromValue { get; set; }
        public string currency { get; set; }
        public string value { get; set; }
    }

    public class Selleractionstorelease
    {
        public string sellerActionToRelease { get; set; }
    }

    public class Refund
    {
        public Amount1 amount { get; set; }
        public string refundDate { get; set; }
        public string refundId { get; set; }
        public string refundReferenceId { get; set; }
        public string refundStatus { get; set; }
    }

    public class Amount1
    {
        public string convertedFromCurrency { get; set; }
        public string convertedFromValue { get; set; }
        public string currency { get; set; }
        public string value { get; set; }
    }

    public class Pricingsummary
    {
        public Adjustment adjustment { get; set; }
        public Deliverycost deliveryCost { get; set; }
        public Deliverydiscount deliveryDiscount { get; set; }
        public Fee fee { get; set; }
        public Pricediscountsubtotal priceDiscountSubtotal { get; set; }
        public Pricesubtotal priceSubtotal { get; set; }
        public Tax tax { get; set; }
        public Total total { get; set; }
    }

    public class Adjustment
    {
        public string convertedFromCurrency { get; set; }
        public string convertedFromValue { get; set; }
        public string currency { get; set; }
        public string value { get; set; }
    }

    public class Deliverycost
    {
        public string convertedFromCurrency { get; set; }
        public string convertedFromValue { get; set; }
        public string currency { get; set; }
        public string value { get; set; }
    }

    public class Deliverydiscount
    {
        public string convertedFromCurrency { get; set; }
        public string convertedFromValue { get; set; }
        public string currency { get; set; }
        public string value { get; set; }
    }

    public class Fee
    {
        public string convertedFromCurrency { get; set; }
        public string convertedFromValue { get; set; }
        public string currency { get; set; }
        public string value { get; set; }
    }

    public class Pricediscountsubtotal
    {
        public string convertedFromCurrency { get; set; }
        public string convertedFromValue { get; set; }
        public string currency { get; set; }
        public string value { get; set; }
    }

    public class Pricesubtotal
    {
        public string convertedFromCurrency { get; set; }
        public string convertedFromValue { get; set; }
        public string currency { get; set; }
        public string value { get; set; }
    }

    public class Tax
    {
        public string convertedFromCurrency { get; set; }
        public string convertedFromValue { get; set; }
        public string currency { get; set; }
        public string value { get; set; }
    }

    public class Total
    {
        public string convertedFromCurrency { get; set; }
        public string convertedFromValue { get; set; }
        public string currency { get; set; }
        public string value { get; set; }
    }

    public class Fulfillmentstartinstruction
    {
        public string destinationTimeZone { get; set; }
        public string ebaySupportedFulfillment { get; set; }
        public Finaldestinationaddress finalDestinationAddress { get; set; }
        public string fulfillmentInstructionsType { get; set; }
        public string maxEstimatedDeliveryDate { get; set; }
        public string minEstimatedDeliveryDate { get; set; }
        public Pickupstep pickupStep { get; set; }
        public Shippingstep shippingStep { get; set; }
    }

    public class Finaldestinationaddress
    {
        public string addressLine1 { get; set; }
        public string addressLine2 { get; set; }
        public string city { get; set; }
        public string countryCode { get; set; }
        public string county { get; set; }
        public string postalCode { get; set; }
        public string stateOrProvince { get; set; }
    }

    public class Pickupstep
    {
        public string merchantLocationKey { get; set; }
    }

    public class Shippingstep
    {
        public string shippingCarrierCode { get; set; }
        public string shippingServiceCode { get; set; }
        public Shipto shipTo { get; set; }
        public string shipToReferenceId { get; set; }
    }

    public class Shipto
    {
        public string companyName { get; set; }
        public Contactaddress contactAddress { get; set; }
        public string email { get; set; }
        public string fullName { get; set; }
        public Primaryphone primaryPhone { get; set; }
    }

    public class Contactaddress
    {
        public string addressLine1 { get; set; }
        public string addressLine2 { get; set; }
        public string city { get; set; }
        public string countryCode { get; set; }
        public string county { get; set; }
        public string postalCode { get; set; }
        public string stateOrProvince { get; set; }
    }

    public class Primaryphone
    {
        public string phoneNumber { get; set; }
    }

    public class Lineitem
    {
        public Appliedpromotion[] appliedPromotions { get; set; }
        public Deliverycost1 deliveryCost { get; set; }
        public Discountedlineitemcost discountedLineItemCost { get; set; }
        public Ebaycollectandremittax[] ebayCollectAndRemitTaxes { get; set; }
        public Giftdetails giftDetails { get; set; }
        public string legacyItemId { get; set; }
        public string legacyVariationId { get; set; }
        public Lineitemcost lineItemCost { get; set; }
        public Lineitemfulfillmentinstructions lineItemFulfillmentInstructions { get; set; }
        public string lineItemFulfillmentStatus { get; set; }
        public string lineItemId { get; set; }
        public string listingMarketplaceId { get; set; }
        public Properties properties { get; set; }
        public string purchaseMarketplaceId { get; set; }
        public string quantity { get; set; }
        public Refund1[] refunds { get; set; }
        public string sku { get; set; }
        public string soldFormat { get; set; }
        public Tax1[] taxes { get; set; }
        public string title { get; set; }
        public Total1 total { get; set; }
    }

    public class Deliverycost1
    {
        public Importcharges importCharges { get; set; }
        public Shippingcost shippingCost { get; set; }
        public Shippingintermediationfee shippingIntermediationFee { get; set; }
    }

    public class Importcharges
    {
        public string convertedFromCurrency { get; set; }
        public string convertedFromValue { get; set; }
        public string currency { get; set; }
        public string value { get; set; }
    }

    public class Shippingcost
    {
        public string convertedFromCurrency { get; set; }
        public string convertedFromValue { get; set; }
        public string currency { get; set; }
        public string value { get; set; }
    }

    public class Shippingintermediationfee
    {
        public string convertedFromCurrency { get; set; }
        public string convertedFromValue { get; set; }
        public string currency { get; set; }
        public string value { get; set; }
    }

    public class Discountedlineitemcost
    {
        public string convertedFromCurrency { get; set; }
        public string convertedFromValue { get; set; }
        public string currency { get; set; }
        public string value { get; set; }
    }

    public class Giftdetails
    {
        public string message { get; set; }
        public string recipientEmail { get; set; }
        public string senderName { get; set; }
    }

    public class Lineitemcost
    {
        public string convertedFromCurrency { get; set; }
        public string convertedFromValue { get; set; }
        public string currency { get; set; }
        public string value { get; set; }
    }

    public class Lineitemfulfillmentinstructions
    {
        public string destinationTimeZone { get; set; }
        public string guaranteedDelivery { get; set; }
        public string maxEstimatedDeliveryDate { get; set; }
        public string minEstimatedDeliveryDate { get; set; }
        public string shipByDate { get; set; }
        public string sourceTimeZone { get; set; }
    }

    public class Properties
    {
        public string buyerProtection { get; set; }
        public string fromBestOffer { get; set; }
        public string soldViaAdCampaign { get; set; }
    }

    public class Total1
    {
        public string convertedFromCurrency { get; set; }
        public string convertedFromValue { get; set; }
        public string currency { get; set; }
        public string value { get; set; }
    }

    public class Appliedpromotion
    {
        public string description { get; set; }
        public Discountamount discountAmount { get; set; }
        public string promotionId { get; set; }
    }

    public class Discountamount
    {
        public string convertedFromCurrency { get; set; }
        public string convertedFromValue { get; set; }
        public string currency { get; set; }
        public string value { get; set; }
    }

    public class Ebaycollectandremittax
    {
        public Amount2 amount { get; set; }
        public string taxType { get; set; }
        public string collectionMethod { get; set; }
    }

    public class Amount2
    {
        public string convertedFromCurrency { get; set; }
        public string convertedFromValue { get; set; }
        public string currency { get; set; }
        public string value { get; set; }
    }

    public class Refund1
    {
        public Amount3 amount { get; set; }
        public string refundDate { get; set; }
        public string refundId { get; set; }
        public string refundReferenceId { get; set; }
    }

    public class Amount3
    {
        public string convertedFromCurrency { get; set; }
        public string convertedFromValue { get; set; }
        public string currency { get; set; }
        public string value { get; set; }
    }

    public class Tax1
    {
        public Amount4 amount { get; set; }
    }

    public class Amount4
    {
        public string convertedFromCurrency { get; set; }
        public string convertedFromValue { get; set; }
        public string currency { get; set; }
        public string value { get; set; }
    }

    public class Warning
    {
        public string category { get; set; }
        public string domain { get; set; }
        public string errorId { get; set; }
        public string[] inputRefIds { get; set; }
        public string longMessage { get; set; }
        public string message { get; set; }
        public string[] outputRefIds { get; set; }
        public Parameter[] parameters { get; set; }
        public string subdomain { get; set; }
    }

    public class Parameter
    {
        public string name { get; set; }
        public string value { get; set; }
    }

}