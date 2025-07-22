namespace denQuickbooksNET80.Models;

public class QBCustomerQuery
{
    public class Rootobject
    {
        public Queryresponse QueryResponse { get; set; }
        public DateTime time { get; set; }
    }

    public class Queryresponse
    {
        public Customer[] Customer { get; set; }
        public int startPosition { get; set; }
        public int maxResults { get; set; }
    }

    public class Customer
    {
        public bool Taxable { get; set; }
        public Billaddr BillAddr { get; set; }
        public bool Job { get; set; }
        public bool BillWithParent { get; set; }
        public float Balance { get; set; }
        public float BalanceWithJobs { get; set; }
        public Currencyref CurrencyRef { get; set; }
        public string PreferredDeliveryMethod { get; set; }
        public bool IsProject { get; set; }
        public string ClientEntityId { get; set; }
        public string domain { get; set; }
        public bool sparse { get; set; }
        public string Id { get; set; }
        public string SyncToken { get; set; }
        public Metadata MetaData { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string FullyQualifiedName { get; set; }
        public string DisplayName { get; set; }
        public string PrintOnCheckName { get; set; }
        public bool Active { get; set; }
        public string V4IDPseudonym { get; set; }
        public Primaryphone PrimaryPhone { get; set; }
        public Primaryemailaddr PrimaryEmailAddr { get; set; }
        public string CompanyName { get; set; }
    }

    public class Billaddr
    {
        public string Id { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string CountrySubDivisionCode { get; set; }
    }

    public class Currencyref
    {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class Metadata
    {
        public DateTime CreateTime { get; set; }
        public DateTime LastUpdatedTime { get; set; }
    }

    public class Primaryphone
    {
        public string FreeFormNumber { get; set; }
    }

    public class Primaryemailaddr
    {
        public string Address { get; set; }
    }



}