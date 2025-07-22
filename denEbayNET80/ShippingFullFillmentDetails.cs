namespace denEbayNET80;

public class ShippingFullFillmentDetails
{

    public class Rootobject
    {
        public Lineitem[] lineItems { get; set; }
        public string shippedDate { get; set; }
        public string shippingCarrierCode { get; set; }
        public string trackingNumber { get; set; }
    }

    public class Lineitem
    {
        public string lineItemId { get; set; }
        public int quantity { get; set; }
    }


}