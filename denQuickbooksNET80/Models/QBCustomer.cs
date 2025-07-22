namespace denQuickBooksNET80.Models;

public class QBCustomer
{

    public class Rootobject
    {
        public Customer Customer { get; set; }
        public DateTime time { get; set; }
    }

    public class Customer
    {
        public Primaryemailaddr PrimaryEmailAddr { get; set; }
        public string SyncToken { get; set; }
        public string domain { get; set; }
        public string GivenName { get; set; }
        public string MiddleName { get; set; }
        public string DisplayName { get; set; }
        public bool BillWithParent { get; set; }
        public string FullyQualifiedName { get; set; }
        public string CompanyName { get; set; }
        public string FamilyName { get; set; }
        public bool sparse { get; set; }
        public Currencyref CurrencyRef { get; set; }
        public Primaryphone PrimaryPhone { get; set; }
        public bool Active { get; set; }
        public bool Job { get; set; }
        public float BalanceWithJobs { get; set; }
        public Billaddr BillAddr { get; set; }
        public string PreferredDeliveryMethod { get; set; }
        public bool Taxable { get; set; }
        public string PrintOnCheckName { get; set; }
        public float Balance { get; set; }
        public string Id { get; set; }
        public Metadata MetaData { get; set; }
    }

    public class Currencyref
    {
        public string name { get; set; }
        public string value { get; set; }
    }

    public class Primaryemailaddr
    {
        public string Address { get; set; }
    }

    public class Primaryphone
    {
        public string FreeFormNumber { get; set; }
    }

    public class Billaddr
    {
        public string City { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string PostalCode { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
        public string CountrySubDivisionCode { get; set; }
        public string Country { get; set; }
        public string Id { get; set; }
    }

    public class Metadata
    {
        public DateTime CreateTime { get; set; }
        public DateTime LastUpdatedTime { get; set; }
    }

}

//public class QBCustomer
// {
//     // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
//     /// <remarks/>
//     [System.SerializableAttribute()]
//     [System.ComponentModel.DesignerCategoryAttribute("code")]
//     [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schema.intuit.com/finance/v3")]
//     [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schema.intuit.com/finance/v3", IsNullable = false)]
//     public partial class IntuitResponse
//     {

//         private IntuitResponseCustomer customerField;

//         private System.DateTime timeField;

//         /// <remarks/>
//         public IntuitResponseCustomer Customer
//         {
//             get
//             {
//                 return customerField;
//             }
//             set
//             {
//                 customerField = value;
//             }
//         }

//         /// <remarks/>
//         [System.Xml.Serialization.XmlAttributeAttribute()]
//         public System.DateTime time
//         {
//             get
//             {
//                 return timeField;
//             }
//             set
//             {
//                 timeField = value;
//             }
//         }
//     }

//     /// <remarks/>
//     [System.SerializableAttribute()]
//     [System.ComponentModel.DesignerCategoryAttribute("code")]
//     [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schema.intuit.com/finance/v3")]
//     public partial class IntuitResponseCustomer
//     {

//         private ushort idField;

//         private byte syncTokenField;

//         private IntuitResponseCustomerMetaData metaDataField;

//         private string givenNameField;

//         private string familyNameField;

//         private string fullyQualifiedNameField;

//         private string displayNameField;

//         private string printOnCheckNameField;

//         private bool activeField;

//         private IntuitResponseCustomerPrimaryPhone primaryPhoneField;

//         private IntuitResponseCustomerPrimaryEmailAddr primaryEmailAddrField;

//         private bool taxableField;

//         private IntuitResponseCustomerBillAddr billAddrField;

//         private bool jobField;

//         private bool billWithParentField;

//         private decimal balanceField;

//         private decimal balanceWithJobsField;

//         private IntuitResponseCustomerCurrencyRef currencyRefField;

//         private string preferredDeliveryMethodField;

//         private string domainField;

//         private bool sparseField;

//         /// <remarks/>
//         public ushort Id
//         {
//             get
//             {
//                 return idField;
//             }
//             set
//             {
//                 idField = value;
//             }
//         }

//         /// <remarks/>
//         public byte SyncToken
//         {
//             get
//             {
//                 return syncTokenField;
//             }
//             set
//             {
//                 syncTokenField = value;
//             }
//         }

//         /// <remarks/>
//         public IntuitResponseCustomerMetaData MetaData
//         {
//             get
//             {
//                 return metaDataField;
//             }
//             set
//             {
//                 metaDataField = value;
//             }
//         }

//         /// <remarks/>
//         public string GivenName
//         {
//             get
//             {
//                 return givenNameField;
//             }
//             set
//             {
//                 givenNameField = value;
//             }
//         }

//         /// <remarks/>
//         public string FamilyName
//         {
//             get
//             {
//                 return familyNameField;
//             }
//             set
//             {
//                 familyNameField = value;
//             }
//         }

//         /// <remarks/>
//         public string FullyQualifiedName
//         {
//             get
//             {
//                 return fullyQualifiedNameField;
//             }
//             set
//             {
//                 fullyQualifiedNameField = value;
//             }
//         }

//         /// <remarks/>
//         public string DisplayName
//         {
//             get
//             {
//                 return displayNameField;
//             }
//             set
//             {
//                 displayNameField = value;
//             }
//         }

//         /// <remarks/>
//         public string PrintOnCheckName
//         {
//             get
//             {
//                 return printOnCheckNameField;
//             }
//             set
//             {
//                 printOnCheckNameField = value;
//             }
//         }

//         /// <remarks/>
//         public bool Active
//         {
//             get
//             {
//                 return activeField;
//             }
//             set
//             {
//                 activeField = value;
//             }
//         }

//         /// <remarks/>
//         public IntuitResponseCustomerPrimaryPhone PrimaryPhone
//         {
//             get
//             {
//                 return primaryPhoneField;
//             }
//             set
//             {
//                 primaryPhoneField = value;
//             }
//         }

//         /// <remarks/>
//         public IntuitResponseCustomerPrimaryEmailAddr PrimaryEmailAddr
//         {
//             get
//             {
//                 return primaryEmailAddrField;
//             }
//             set
//             {
//                 primaryEmailAddrField = value;
//             }
//         }

//         /// <remarks/>
//         public bool Taxable
//         {
//             get
//             {
//                 return taxableField;
//             }
//             set
//             {
//                 taxableField = value;
//             }
//         }

//         /// <remarks/>
//         public IntuitResponseCustomerBillAddr BillAddr
//         {
//             get
//             {
//                 return billAddrField;
//             }
//             set
//             {
//                 billAddrField = value;
//             }
//         }

//         /// <remarks/>
//         public bool Job
//         {
//             get
//             {
//                 return jobField;
//             }
//             set
//             {
//                 jobField = value;
//             }
//         }

//         /// <remarks/>
//         public bool BillWithParent
//         {
//             get
//             {
//                 return billWithParentField;
//             }
//             set
//             {
//                 billWithParentField = value;
//             }
//         }

//         /// <remarks/>
//         public decimal Balance
//         {
//             get
//             {
//                 return balanceField;
//             }
//             set
//             {
//                 balanceField = value;
//             }
//         }

//         /// <remarks/>
//         public decimal BalanceWithJobs
//         {
//             get
//             {
//                 return balanceWithJobsField;
//             }
//             set
//             {
//                 balanceWithJobsField = value;
//             }
//         }

//         /// <remarks/>
//         public IntuitResponseCustomerCurrencyRef CurrencyRef
//         {
//             get
//             {
//                 return currencyRefField;
//             }
//             set
//             {
//                 currencyRefField = value;
//             }
//         }

//         /// <remarks/>
//         public string PreferredDeliveryMethod
//         {
//             get
//             {
//                 return preferredDeliveryMethodField;
//             }
//             set
//             {
//                 preferredDeliveryMethodField = value;
//             }
//         }

//         /// <remarks/>
//         [System.Xml.Serialization.XmlAttributeAttribute()]
//         public string domain
//         {
//             get
//             {
//                 return domainField;
//             }
//             set
//             {
//                 domainField = value;
//             }
//         }

//         /// <remarks/>
//         [System.Xml.Serialization.XmlAttributeAttribute()]
//         public bool sparse
//         {
//             get
//             {
//                 return sparseField;
//             }
//             set
//             {
//                 sparseField = value;
//             }
//         }
//     }

//     /// <remarks/>
//     [System.SerializableAttribute()]
//     [System.ComponentModel.DesignerCategoryAttribute("code")]
//     [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schema.intuit.com/finance/v3")]
//     public partial class IntuitResponseCustomerMetaData
//     {

//         private System.DateTime createTimeField;

//         private System.DateTime lastUpdatedTimeField;

//         /// <remarks/>
//         public System.DateTime CreateTime
//         {
//             get
//             {
//                 return createTimeField;
//             }
//             set
//             {
//                 createTimeField = value;
//             }
//         }

//         /// <remarks/>
//         public System.DateTime LastUpdatedTime
//         {
//             get
//             {
//                 return lastUpdatedTimeField;
//             }
//             set
//             {
//                 lastUpdatedTimeField = value;
//             }
//         }
//     }

//     /// <remarks/>
//     [System.SerializableAttribute()]
//     [System.ComponentModel.DesignerCategoryAttribute("code")]
//     [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schema.intuit.com/finance/v3")]
//     public partial class IntuitResponseCustomerPrimaryPhone
//     {

//         private string freeFormNumberField;

//         /// <remarks/>
//         public string FreeFormNumber
//         {
//             get
//             {
//                 return freeFormNumberField;
//             }
//             set
//             {
//                 freeFormNumberField = value;
//             }
//         }
//     }

//     /// <remarks/>
//     [System.SerializableAttribute()]
//     [System.ComponentModel.DesignerCategoryAttribute("code")]
//     [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schema.intuit.com/finance/v3")]
//     public partial class IntuitResponseCustomerPrimaryEmailAddr
//     {

//         private string addressField;

//         /// <remarks/>
//         public string Address
//         {
//             get
//             {
//                 return addressField;
//             }
//             set
//             {
//                 addressField = value;
//             }
//         }
//     }

//     /// <remarks/>
//     [System.SerializableAttribute()]
//     [System.ComponentModel.DesignerCategoryAttribute("code")]
//     [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schema.intuit.com/finance/v3")]
//     public partial class IntuitResponseCustomerBillAddr
//     {

//         private int idField;

//         private string line1Field;

//         private string line2Field;

//         private string cityField;

//         private string countrySubDivisionCodeField;

//         private string countryField;

//         private string postalCodeField;

//         /// <remarks/>
//         public int Id
//         {
//             get
//             {
//                 return idField;
//             }
//             set
//             {
//                 idField = value;
//             }
//         }

//         /// <remarks/>
//         public string Line1
//         {
//             get
//             {
//                 return line1Field;
//             }
//             set
//             {
//                 line1Field = value;
//             }
//         }

//         /// <remarks/>
//         public string Line2
//         {
//             get
//             {
//                 return line2Field;
//             }
//             set
//             {
//                 line2Field = value;
//             }
//         }

//         /// <remarks/>
//         public string City
//         {
//             get
//             {
//                 return cityField;
//             }
//             set
//             {
//                 cityField = value;
//             }
//         }

//         /// <remarks/>
//         public string Country
//         {
//             get
//             {
//                 return countryField;
//             }
//             set
//             {
//                 countryField = value;
//             }
//         }
//         /// <remarks/>
//         public string CountrySubDivisionCode
//         {
//             get
//             {
//                 return countrySubDivisionCodeField;
//             }
//             set
//             {
//                 countrySubDivisionCodeField = value;
//             }
//         }
//         /// <remarks/>
//         public string PostalCode
//         {
//             get
//             {
//                 return postalCodeField;
//             }
//             set
//             {
//                 postalCodeField = value;
//             }
//         }
//     }

//     /// <remarks/>
//     [System.SerializableAttribute()]
//     [System.ComponentModel.DesignerCategoryAttribute("code")]
//     [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schema.intuit.com/finance/v3")]
//     public partial class IntuitResponseCustomerCurrencyRef
//     {

//         private string nameField;

//         private string valueField;

//         /// <remarks/>
//         [System.Xml.Serialization.XmlAttributeAttribute()]
//         public string name
//         {
//             get
//             {
//                 return nameField;
//             }
//             set
//             {
//                 nameField = value;
//             }
//         }

//         /// <remarks/>
//         [System.Xml.Serialization.XmlTextAttribute()]
//         public string Value
//         {
//             get
//             {
//                 return valueField;
//             }
//             set
//             {
//                 valueField = value;
//             }
//         }
//     }
// }