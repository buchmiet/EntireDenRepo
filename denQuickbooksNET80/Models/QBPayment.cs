using Newtonsoft.Json;

namespace denQuickBooksNET80.Models;

public class QBPayment
{


    public class Rootobject
    {
        public Payment Payment { get; set; }
        public DateTime time { get; set; }
    }

    public class Payment
    {
        public string SyncToken { get; set; }
        public string domain { get; set; }
        public Deposittoaccountref DepositToAccountRef { get; set; }
        public float UnappliedAmt { get; set; }
        public string TxnDate { get; set; }
        public float TotalAmt { get; set; }
        public Projectref ProjectRef { get; set; }
        public bool ProcessPayment { get; set; }
        public bool sparse { get; set; }
        public Line[] Line { get; set; }
        public Customerref CustomerRef { get; set; }
        public string Id { get; set; }
        public Metadata MetaData { get; set; }
    }

    public class Deposittoaccountref
    {
        public string value { get; set; }
    }

    public class Projectref
    {
        public string value { get; set; }
    }

    public class Customerref
    {
        public string name { get; set; }
        public string value { get; set; }
    }

    public class Metadata
    {
        public DateTime CreateTime { get; set; }
        public DateTime LastUpdatedTime { get; set; }
    }

    public class Line
    {
        public float Amount { get; set; }
        public Lineex LineEx { get; set; }
        public Linkedtxn[] LinkedTxn { get; set; }
    }

    public class Lineex
    {
        public Any[] any { get; set; }
    }

    public class Any
    {
        public string name { get; set; }
        public bool nil { get; set; }
        public Value value { get; set; }
        public string declaredType { get; set; }
        public string scope { get; set; }
        public bool globalScope { get; set; }
        public bool typeSubstituted { get; set; }
    }

    public class Value
    {
        public string Name { get; set; }
        [JsonProperty("Value")]
        public string ValueData { get; set; }
    }

    public class Linkedtxn
    {
        public string TxnId { get; set; }
        public string TxnType { get; set; }
    }


}

//public class QBPayment
// {

//     // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
//     /// <remarks/>
//     [System.SerializableAttribute()]
//     [System.ComponentModel.DesignerCategoryAttribute("code")]
//     [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schema.intuit.com/finance/v3")]
//     [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schema.intuit.com/finance/v3", IsNullable = false)]
//     public partial class IntuitResponse
//     {

//         private IntuitResponsePayment paymentField;

//         private System.DateTime timeField;

//         /// <remarks/>
//         public IntuitResponsePayment Payment
//         {
//             get
//             {
//                 return this.paymentField;
//             }
//             set
//             {
//                 this.paymentField = value;
//             }
//         }

//         /// <remarks/>
//         [System.Xml.Serialization.XmlAttributeAttribute()]
//         public System.DateTime time
//         {
//             get
//             {
//                 return this.timeField;
//             }
//             set
//             {
//                 this.timeField = value;
//             }
//         }
//     }

//     /// <remarks/>
//     [System.SerializableAttribute()]
//     [System.ComponentModel.DesignerCategoryAttribute("code")]
//     [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schema.intuit.com/finance/v3")]
//     public partial class IntuitResponsePayment
//     {

//         private int idField;

//         private byte syncTokenField;

//         private IntuitResponsePaymentMetaData metaDataField;

//         private System.DateTime txnDateField;

//         private IntuitResponsePaymentCurrencyRef currencyRefField;

//         private byte exchangeRateField;

//         private IntuitResponsePaymentLine lineField;

//         private IntuitResponsePaymentCustomerRef customerRefField;

//         private byte depositToAccountRefField;

//         private decimal totalAmtField;

//         private decimal unappliedAmtField;

//         private bool processPaymentField;

//         private string domainField;

//         private bool sparseField;

//         /// <remarks/>
//         public int Id
//         {
//             get
//             {
//                 return this.idField;
//             }
//             set
//             {
//                 this.idField = value;
//             }
//         }

//         /// <remarks/>
//         public byte SyncToken
//         {
//             get
//             {
//                 return this.syncTokenField;
//             }
//             set
//             {
//                 this.syncTokenField = value;
//             }
//         }

//         /// <remarks/>
//         public IntuitResponsePaymentMetaData MetaData
//         {
//             get
//             {
//                 return this.metaDataField;
//             }
//             set
//             {
//                 this.metaDataField = value;
//             }
//         }

//         /// <remarks/>
//         [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
//         public System.DateTime TxnDate
//         {
//             get
//             {
//                 return this.txnDateField;
//             }
//             set
//             {
//                 this.txnDateField = value;
//             }
//         }

//         /// <remarks/>
//         public IntuitResponsePaymentCurrencyRef CurrencyRef
//         {
//             get
//             {
//                 return this.currencyRefField;
//             }
//             set
//             {
//                 this.currencyRefField = value;
//             }
//         }

//         /// <remarks/>
//         public byte ExchangeRate
//         {
//             get
//             {
//                 return this.exchangeRateField;
//             }
//             set
//             {
//                 this.exchangeRateField = value;
//             }
//         }

//         /// <remarks/>
//         public IntuitResponsePaymentLine Line
//         {
//             get
//             {
//                 return this.lineField;
//             }
//             set
//             {
//                 this.lineField = value;
//             }
//         }

//         /// <remarks/>
//         public IntuitResponsePaymentCustomerRef CustomerRef
//         {
//             get
//             {
//                 return this.customerRefField;
//             }
//             set
//             {
//                 this.customerRefField = value;
//             }
//         }

//         /// <remarks/>
//         public byte DepositToAccountRef
//         {
//             get
//             {
//                 return this.depositToAccountRefField;
//             }
//             set
//             {
//                 this.depositToAccountRefField = value;
//             }
//         }

//         /// <remarks/>
//         public decimal TotalAmt
//         {
//             get
//             {
//                 return this.totalAmtField;
//             }
//             set
//             {
//                 this.totalAmtField = value;
//             }
//         }

//         /// <remarks/>
//         public decimal UnappliedAmt
//         {
//             get
//             {
//                 return this.unappliedAmtField;
//             }
//             set
//             {
//                 this.unappliedAmtField = value;
//             }
//         }

//         /// <remarks/>
//         public bool ProcessPayment
//         {
//             get
//             {
//                 return this.processPaymentField;
//             }
//             set
//             {
//                 this.processPaymentField = value;
//             }
//         }

//         /// <remarks/>
//         [System.Xml.Serialization.XmlAttributeAttribute()]
//         public string domain
//         {
//             get
//             {
//                 return this.domainField;
//             }
//             set
//             {
//                 this.domainField = value;
//             }
//         }

//         /// <remarks/>
//         [System.Xml.Serialization.XmlAttributeAttribute()]
//         public bool sparse
//         {
//             get
//             {
//                 return this.sparseField;
//             }
//             set
//             {
//                 this.sparseField = value;
//             }
//         }
//     }

//     /// <remarks/>
//     [System.SerializableAttribute()]
//     [System.ComponentModel.DesignerCategoryAttribute("code")]
//     [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schema.intuit.com/finance/v3")]
//     public partial class IntuitResponsePaymentMetaData
//     {

//         private System.DateTime createTimeField;

//         private System.DateTime lastUpdatedTimeField;

//         /// <remarks/>
//         public System.DateTime CreateTime
//         {
//             get
//             {
//                 return this.createTimeField;
//             }
//             set
//             {
//                 this.createTimeField = value;
//             }
//         }

//         /// <remarks/>
//         public System.DateTime LastUpdatedTime
//         {
//             get
//             {
//                 return this.lastUpdatedTimeField;
//             }
//             set
//             {
//                 this.lastUpdatedTimeField = value;
//             }
//         }
//     }

//     /// <remarks/>
//     [System.SerializableAttribute()]
//     [System.ComponentModel.DesignerCategoryAttribute("code")]
//     [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schema.intuit.com/finance/v3")]
//     public partial class IntuitResponsePaymentCurrencyRef
//     {

//         private string nameField;

//         private string valueField;

//         /// <remarks/>
//         [System.Xml.Serialization.XmlAttributeAttribute()]
//         public string name
//         {
//             get
//             {
//                 return this.nameField;
//             }
//             set
//             {
//                 this.nameField = value;
//             }
//         }

//         /// <remarks/>
//         [System.Xml.Serialization.XmlTextAttribute()]
//         public string Value
//         {
//             get
//             {
//                 return this.valueField;
//             }
//             set
//             {
//                 this.valueField = value;
//             }
//         }
//     }

//     /// <remarks/>
//     [System.SerializableAttribute()]
//     [System.ComponentModel.DesignerCategoryAttribute("code")]
//     [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schema.intuit.com/finance/v3")]
//     public partial class IntuitResponsePaymentLine
//     {

//         private decimal amountField;

//         private IntuitResponsePaymentLineLinkedTxn linkedTxnField;

//         private IntuitResponsePaymentLineNameValue[] lineExField;

//         /// <remarks/>
//         public decimal Amount
//         {
//             get
//             {
//                 return this.amountField;
//             }
//             set
//             {
//                 this.amountField = value;
//             }
//         }

//         /// <remarks/>
//         public IntuitResponsePaymentLineLinkedTxn LinkedTxn
//         {
//             get
//             {
//                 return this.linkedTxnField;
//             }
//             set
//             {
//                 this.linkedTxnField = value;
//             }
//         }

//         /// <remarks/>
//         [System.Xml.Serialization.XmlArrayItemAttribute("NameValue", IsNullable = false)]
//         public IntuitResponsePaymentLineNameValue[] LineEx
//         {
//             get
//             {
//                 return this.lineExField;
//             }
//             set
//             {
//                 this.lineExField = value;
//             }
//         }
//     }

//     /// <remarks/>
//     [System.SerializableAttribute()]
//     [System.ComponentModel.DesignerCategoryAttribute("code")]
//     [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schema.intuit.com/finance/v3")]
//     public partial class IntuitResponsePaymentLineLinkedTxn
//     {

//         private int txnIdField;

//         private string txnTypeField;

//         /// <remarks/>
//         public int TxnId
//         {
//             get
//             {
//                 return this.txnIdField;
//             }
//             set
//             {
//                 this.txnIdField = value;
//             }
//         }

//         /// <remarks/>
//         public string TxnType
//         {
//             get
//             {
//                 return this.txnTypeField;
//             }
//             set
//             {
//                 this.txnTypeField = value;
//             }
//         }
//     }

//     /// <remarks/>
//     [System.SerializableAttribute()]
//     [System.ComponentModel.DesignerCategoryAttribute("code")]
//     [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schema.intuit.com/finance/v3")]
//     public partial class IntuitResponsePaymentLineNameValue
//     {

//         private string nameField;

//         private decimal valueField;

//         /// <remarks/>
//         public string Name
//         {
//             get
//             {
//                 return this.nameField;
//             }
//             set
//             {
//                 this.nameField = value;
//             }
//         }

//         /// <remarks/>
//         public decimal Value
//         {
//             get
//             {
//                 return this.valueField;
//             }
//             set
//             {
//                 this.valueField = value;
//             }
//         }
//     }

//     /// <remarks/>
//     [System.SerializableAttribute()]
//     [System.ComponentModel.DesignerCategoryAttribute("code")]
//     [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schema.intuit.com/finance/v3")]
//     public partial class IntuitResponsePaymentCustomerRef
//     {

//         private string nameField;

//         private int valueField;

//         /// <remarks/>
//         [System.Xml.Serialization.XmlAttributeAttribute()]
//         public string name
//         {
//             get
//             {
//                 return this.nameField;
//             }
//             set
//             {
//                 this.nameField = value;
//             }
//         }

//         /// <remarks/>
//         [System.Xml.Serialization.XmlTextAttribute()]
//         public int Value
//         {
//             get
//             {
//                 return this.valueField;
//             }
//             set
//             {
//                 this.valueField = value;
//             }
//         }
//     }


// }