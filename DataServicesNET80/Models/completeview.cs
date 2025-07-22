namespace DataServicesNET80.Models;

public partial class completeview
{
    public int orderID { get; set; }

    public string status { get; set; } = null!;

    public string CountryCode { get; set; } = null!;

    public bool quickbooked { get; set; }

    public int customerID { get; set; }

    public DateTime paidOn { get; set; }

    public DateTime dispatchedOn { get; set; }

    public string tracking { get; set; } = null!;

    public int market { get; set; }

    public int locationID { get; set; }

    public string? salecurrency { get; set; }

    public string? acquiredcurrency { get; set; }

    public decimal saletotal { get; set; }

    public decimal xchgrate { get; set; }

    public bool VAT { get; set; }

    public string? order_notes { get; set; }

    public decimal postagePrice { get; set; }

    public string? postageType { get; set; }

    public int VATRateID { get; set; }

    public string? Title { get; set; }

    public string? GivenName { get; set; }

    public string? MiddleName { get; set; }

    public string? FamilyName { get; set; }

    public string? CompanyName { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? DisplayName { get; set; }

    public string? customer_currency { get; set; }

    public string? customer_notes { get; set; }

    public int? billaddrID { get; set; }

    public string? Line1 { get; set; }

    public string? Line2 { get; set; }

    public string? City { get; set; }

    public string? CountrySubDivisionCode { get; set; }

    public string PostalCode { get; set; } = null!;

    public string? AddressAsAString { get; set; }

    public int? OrderItemId { get; set; }

    public string? itemName { get; set; }

    public int? quantity { get; set; }

    public int? itembodyID { get; set; }

    public int? OrderItemTypeId { get; set; }

    public decimal? price { get; set; }

    public int? itmMarketAssID { get; set; }

    public int? ItemWeight { get; set; }
}
