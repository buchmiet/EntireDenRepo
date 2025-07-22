

namespace denQuickBooksNET80.Models;

public class QBInvoice
{

    public class Rootobject
    {
        public Invoice Invoice { get; set; }
        public DateTime time { get; set; }
    }

    public class Invoice
    {
        public string TxnDate { get; set; }
        public string domain { get; set; }
        public string PrintStatus { get; set; }
        public Salestermref SalesTermRef { get; set; }
        public float TotalAmt { get; set; }
        public Line[] Line { get; set; }
        public string DueDate { get; set; }
        public string GlobalTaxCalculation = "TaxInclusive";
        public string DocNumber { get; set; }
         
        public decimal ExchangeRate { get; set; }
        public Customermemo CustomerMemo { get; set; }
        public Projectref ProjectRef { get; set; }
          
        public Currencyref CurrencyRef { get; set; }
        
        public string HomeTotalAmt { get; set; }
        public Taxcoderef TxnTaxCodeRef { get; set; }

        public Customerref CustomerRef { get; set; }
        public Txntaxdetail TxnTaxDetail { get; set; }
        public string SyncToken { get; set; }
        public Linkedtxn[] LinkedTxn { get; set; }
        public Billemail BillEmail { get; set; }
        public Shipaddr ShipAddr { get; set; }
        public string EmailStatus { get; set; }
        public Billaddr BillAddr { get; set; }
        public Metadata MetaData { get; set; }
        public Customfield[] CustomField { get; set; }
        public string Id { get; set; }
    }

    public class Taxcoderef
    {
        public string name { get; set; }
        public string value { get; set; }
    }

    public class Currencyref
    {
        public string name { get; set; }
        public string value { get; set; }
    }

    public class Salestermref
    {
        public string value { get; set; }
    }

    public class Customermemo
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

    public class Txntaxdetail
    {
        public Txntaxcoderef TxnTaxCodeRef { get; set; }
        public float TotalTax { get; set; }
        public Taxline[] TaxLine { get; set; }
    }

    public class Txntaxcoderef
    {
        public string value { get; set; }
    }

    public class Taxline
    {
        public string DetailType { get; set; }
        public float Amount { get; set; }
        public Taxlinedetail TaxLineDetail { get; set; }
    }

    public class Taxlinedetail
    {
        public float NetAmountTaxable { get; set; }
        public int TaxPercent { get; set; }
        public Taxrateref TaxRateRef { get; set; }
        public bool PercentBased { get; set; }
    }

    public class Taxrateref
    {
        public string value { get; set; }
    }

    public class Billemail
    {
        public string Address { get; set; }
    }

    public class Shipaddr
    {
        public string City { get; set; }
        public string Line1 { get; set; }
        public string PostalCode { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
        public string CountrySubDivisionCode { get; set; }
        public string Id { get; set; }
    }

    public class Billaddr
    {
        public string Line4 { get; set; }
        public string Line3 { get; set; }
        public string Line2 { get; set; }
        public string Line1 { get; set; }
        public string Long { get; set; }
        public string Lat { get; set; }
        public string Id { get; set; }
    }

    public class Metadata
    {
        public DateTime CreateTime { get; set; }
        public DateTime LastUpdatedTime { get; set; }
    }

    public class Line
    {
        public string Description { get; set; }
        public string DetailType { get; set; }
        public Salesitemlinedetail SalesItemLineDetail { get; set; }
       
        public float Amount { get; set; }
        public string Id { get; set; }
        public Subtotallinedetail SubTotalLineDetail { get; set; }
    }

    public class Salesitemlinedetail
    {
        public string TaxInclusiveAmt { get; set; }
        public Taxcoderef TaxCodeRef { get; set; }
        public int Qty { get; set; }
        public float UnitPrice { get; set; }
        public Itemref ItemRef { get; set; }
    }

    //public class Taxcoderef
    //{
    //    public string value { get; set; }

    //    public static implicit operator Taxcoderef(Txntaxcoderef v)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    public class Itemref
    {
        public string name { get; set; }
        public string value { get; set; }
    }

    public class Subtotallinedetail
    {
    }

    public class Linkedtxn
    {
        public string TxnId { get; set; }
        public string TxnType { get; set; }
    }

    public class Customfield
    {
        public string DefinitionId { get; set; }
        public string StringValue { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
    }

}

//public class QBInvoice
// {
//     [System.SerializableAttribute()]
//     [System.ComponentModel.DesignerCategoryAttribute("code")]
//     [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schema.intuit.com/finance/v3")]
//     [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schema.intuit.com/finance/v3", IsNullable = false)]
//     public partial class IntuitResponse
//     {

//         private IntuitResponseInvoice invoiceField;

//         private System.DateTime timeField;

//         /// <remarks/>
//         public IntuitResponseInvoice Invoice
//         {
//             get
//             {
//                 return invoiceField;
//             }
//             set
//             {
//                 invoiceField = value;
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
//     public partial class IntuitResponseInvoice
//     {

//         private int idField;

//         private byte syncTokenField;

//         private IntuitResponseInvoiceMetaData metaDataField;

//         private IntuitResponseInvoiceCustomField customFieldField;

//         private System.DateTime txnDateField;

//         private IntuitResponseInvoiceCurrencyRef currencyRefField;

//         private decimal exchangeRateField;

//         private string privateNoteField;

//         private IntuitResponseInvoiceLinkedTxn linkedTxnField;

//         private IntuitResponseInvoiceLine[] lineField;

//         private IntuitResponseInvoiceTxnTaxDetail txnTaxDetailField;

//         private IntuitResponseInvoiceCustomerRef customerRefField;

//         private IntuitResponseInvoiceBillAddr billAddrField;

//         private System.DateTime dueDateField;

//         private string globalTaxCalculationField;

//         private decimal totalAmtField;

//         private decimal unappliedAmtField;


//         private bool processPaymentField;

//         private decimal homeTotalAmtField;

//         private string printStatusField;

//         private string emailStatusField;

//         private decimal balanceField;

//         private decimal homeBalanceField;

//         private byte depositField;

//         private bool allowIPNPaymentField;

//         private bool allowOnlinePaymentField;

//         private bool allowOnlineCreditCardPaymentField;

//         private bool allowOnlineACHPaymentField;

//         private string domainField;

//         private bool sparseField;

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
//         public IntuitResponseInvoiceMetaData MetaData
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
//         public IntuitResponseInvoiceCustomField CustomField
//         {
//             get
//             {
//                 return customFieldField;
//             }
//             set
//             {
//                 customFieldField = value;
//             }
//         }

//         /// <remarks/>
//         [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
//         public System.DateTime TxnDate
//         {
//             get
//             {
//                 return txnDateField;
//             }
//             set
//             {
//                 txnDateField = value;
//             }
//         }

//         /// <remarks/>
//         public IntuitResponseInvoiceCurrencyRef CurrencyRef
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
//         public decimal ExchangeRate
//         {
//             get
//             {
//                 return exchangeRateField;
//             }
//             set
//             {
//                 exchangeRateField = value;
//             }
//         }

//         public string PrivateNote
//         {
//             get
//             {
//                 return privateNoteField;
//             }
//             set
//             {
//                 privateNoteField = value;
//             }
//         }

//         public IntuitResponseInvoiceLinkedTxn LinkedTxn
//         {
//             get
//             {
//                 return linkedTxnField;
//             }
//             set
//             {
//                 linkedTxnField = value;
//             }
//         }

//         /// <remarks/>
//         [System.Xml.Serialization.XmlElementAttribute("Line")]
//         public IntuitResponseInvoiceLine[] Line
//         {
//             get
//             {
//                 return lineField;
//             }
//             set
//             {
//                 lineField = value;
//             }
//         }

//         /// <remarks/>
//         public IntuitResponseInvoiceTxnTaxDetail TxnTaxDetail
//         {
//             get
//             {
//                 return txnTaxDetailField;
//             }
//             set
//             {
//                 txnTaxDetailField = value;
//             }
//         }

//         /// <remarks/>
//         public IntuitResponseInvoiceCustomerRef CustomerRef
//         {
//             get
//             {
//                 return customerRefField;
//             }
//             set
//             {
//                 customerRefField = value;
//             }
//         }

//         /// <remarks/>
//         public IntuitResponseInvoiceBillAddr BillAddr
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
//         [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
//         public System.DateTime DueDate
//         {
//             get
//             {
//                 return dueDateField;
//             }
//             set
//             {
//                 dueDateField = value;
//             }
//         }

//         /// <remarks/>
//         public string GlobalTaxCalculation
//         {
//             get
//             {
//                 return globalTaxCalculationField;
//             }
//             set
//             {
//                 globalTaxCalculationField = value;
//             }
//         }

//         /// <remarks/>
//         public decimal TotalAmt
//         {
//             get
//             {
//                 return totalAmtField;
//             }
//             set
//             {
//                 totalAmtField = value;
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
//         public decimal HomeTotalAmt
//         {
//             get
//             {
//                 return homeTotalAmtField;
//             }
//             set
//             {
//                 homeTotalAmtField = value;
//             }
//         }

//         /// <remarks/>
//         public string PrintStatus
//         {
//             get
//             {
//                 return printStatusField;
//             }
//             set
//             {
//                 printStatusField = value;
//             }
//         }

//         /// <remarks/>
//         public string EmailStatus
//         {
//             get
//             {
//                 return emailStatusField;
//             }
//             set
//             {
//                 emailStatusField = value;
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
//         public decimal HomeBalance
//         {
//             get
//             {
//                 return homeBalanceField;
//             }
//             set
//             {
//                 homeBalanceField = value;
//             }
//         }

//         /// <remarks/>
//         public byte Deposit
//         {
//             get
//             {
//                 return depositField;
//             }
//             set
//             {
//                 depositField = value;
//             }
//         }

//         /// <remarks/>
//         public bool AllowIPNPayment
//         {
//             get
//             {
//                 return allowIPNPaymentField;
//             }
//             set
//             {
//                 allowIPNPaymentField = value;
//             }
//         }

//         /// <remarks/>
//         public bool AllowOnlinePayment
//         {
//             get
//             {
//                 return allowOnlinePaymentField;
//             }
//             set
//             {
//                 allowOnlinePaymentField = value;
//             }
//         }

//         /// <remarks/>
//         public bool AllowOnlineCreditCardPayment
//         {
//             get
//             {
//                 return allowOnlineCreditCardPaymentField;
//             }
//             set
//             {
//                 allowOnlineCreditCardPaymentField = value;
//             }
//         }

//         /// <remarks/>
//         public bool AllowOnlineACHPayment
//         {
//             get
//             {
//                 return allowOnlineACHPaymentField;
//             }
//             set
//             {
//                 allowOnlineACHPaymentField = value;
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
//     public partial class IntuitResponseInvoiceMetaData
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
//     public partial class IntuitResponseInvoiceCustomField
//     {

//         private byte definitionIdField;

//         private string typeField;

//         /// <remarks/>
//         public byte DefinitionId
//         {
//             get
//             {
//                 return definitionIdField;
//             }
//             set
//             {
//                 definitionIdField = value;
//             }
//         }

//         /// <remarks/>
//         public string Type
//         {
//             get
//             {
//                 return typeField;
//             }
//             set
//             {
//                 typeField = value;
//             }
//         }
//     }

//     /// <remarks/>
//     [System.SerializableAttribute()]
//     [System.ComponentModel.DesignerCategoryAttribute("code")]
//     [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schema.intuit.com/finance/v3")]
//     public partial class IntuitResponseInvoiceCurrencyRef
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

//     [System.SerializableAttribute()]
//     [System.ComponentModel.DesignerCategoryAttribute("code")]
//     [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schema.intuit.com/finance/v3")]
//     public partial class IntuitResponseInvoiceLinkedTxn
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
//     public partial class IntuitResponseInvoiceLine
//     {

//         private byte idField;

//         private bool idFieldSpecified;

//         private byte lineNumField;

//         private bool lineNumFieldSpecified;

//         private string descriptionField;

//         private decimal amountField;

//         private string detailTypeField;

//         private object subTotalLineDetailField;

//         private IntuitResponseInvoiceLineSalesItemLineDetail salesItemLineDetailField;

//         /// <remarks/>
//         public byte Id
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
//         [System.Xml.Serialization.XmlIgnoreAttribute()]
//         public bool IdSpecified
//         {
//             get
//             {
//                 return idFieldSpecified;
//             }
//             set
//             {
//                 idFieldSpecified = value;
//             }
//         }

//         /// <remarks/>
//         public byte LineNum
//         {
//             get
//             {
//                 return lineNumField;
//             }
//             set
//             {
//                 lineNumField = value;
//             }
//         }

//         /// <remarks/>
//         [System.Xml.Serialization.XmlIgnoreAttribute()]
//         public bool LineNumSpecified
//         {
//             get
//             {
//                 return lineNumFieldSpecified;
//             }
//             set
//             {
//                 lineNumFieldSpecified = value;
//             }
//         }

//         /// <remarks/>
//         public string Description
//         {
//             get
//             {
//                 return descriptionField;
//             }
//             set
//             {
//                 descriptionField = value;
//             }
//         }

//         /// <remarks/>
//         public decimal Amount
//         {
//             get
//             {
//                 return amountField;
//             }
//             set
//             {
//                 amountField = value;
//             }
//         }

//         /// <remarks/>
//         public string DetailType
//         {
//             get
//             {
//                 return detailTypeField;
//             }
//             set
//             {
//                 detailTypeField = value;
//             }
//         }

//         /// <remarks/>
//         public object SubTotalLineDetail
//         {
//             get
//             {
//                 return subTotalLineDetailField;
//             }
//             set
//             {
//                 subTotalLineDetailField = value;
//             }
//         }

//         /// <remarks/>
//         public IntuitResponseInvoiceLineSalesItemLineDetail SalesItemLineDetail
//         {
//             get
//             {
//                 return salesItemLineDetailField;
//             }
//             set
//             {
//                 salesItemLineDetailField = value;
//             }
//         }
//     }

//     /// <remarks/>
//     [System.SerializableAttribute()]
//     [System.ComponentModel.DesignerCategoryAttribute("code")]
//     [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schema.intuit.com/finance/v3")]
//     public partial class IntuitResponseInvoiceLineSalesItemLineDetail
//     {

//         private IntuitResponseInvoiceLineSalesItemLineDetailItemRef itemRefField;

//         private decimal unitPriceField;

//         private byte qtyField;

//         private byte taxCodeRefField;

//         private decimal taxInclusiveAmtField;

//         /// <remarks/>
//         public IntuitResponseInvoiceLineSalesItemLineDetailItemRef ItemRef
//         {
//             get
//             {
//                 return itemRefField;
//             }
//             set
//             {
//                 itemRefField = value;
//             }
//         }

//         /// <remarks/>
//         public decimal UnitPrice
//         {
//             get
//             {
//                 return unitPriceField;
//             }
//             set
//             {
//                 unitPriceField = value;
//             }
//         }

//         /// <remarks/>
//         public byte Qty
//         {
//             get
//             {
//                 return qtyField;
//             }
//             set
//             {
//                 qtyField = value;
//             }
//         }

//         /// <remarks/>
//         public byte TaxCodeRef
//         {
//             get
//             {
//                 return taxCodeRefField;
//             }
//             set
//             {
//                 taxCodeRefField = value;
//             }
//         }

//         /// <remarks/>
//         public decimal TaxInclusiveAmt
//         {
//             get
//             {
//                 return taxInclusiveAmtField;
//             }
//             set
//             {
//                 taxInclusiveAmtField = value;
//             }
//         }
//     }

//     /// <remarks/>
//     [System.SerializableAttribute()]
//     [System.ComponentModel.DesignerCategoryAttribute("code")]
//     [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schema.intuit.com/finance/v3")]
//     public partial class IntuitResponseInvoiceLineSalesItemLineDetailItemRef
//     {

//         private string nameField;

//         private ushort valueField;

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
//         public ushort Value
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

//     /// <remarks/>
//     [System.SerializableAttribute()]
//     [System.ComponentModel.DesignerCategoryAttribute("code")]
//     [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schema.intuit.com/finance/v3")]
//     public partial class IntuitResponseInvoiceTxnTaxDetail
//     {

//         private decimal totalTaxField;

//         private IntuitResponseInvoiceTxnTaxDetailTaxLine taxLineField;

//         /// <remarks/>
//         public decimal TotalTax
//         {
//             get
//             {
//                 return totalTaxField;
//             }
//             set
//             {
//                 totalTaxField = value;
//             }
//         }

//         /// <remarks/>
//         public IntuitResponseInvoiceTxnTaxDetailTaxLine TaxLine
//         {
//             get
//             {
//                 return taxLineField;
//             }
//             set
//             {
//                 taxLineField = value;
//             }
//         }
//     }

//     /// <remarks/>
//     [System.SerializableAttribute()]
//     [System.ComponentModel.DesignerCategoryAttribute("code")]
//     [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schema.intuit.com/finance/v3")]
//     public partial class IntuitResponseInvoiceTxnTaxDetailTaxLine
//     {

//         private decimal amountField;

//         private string detailTypeField;

//         private IntuitResponseInvoiceTxnTaxDetailTaxLineTaxLineDetail taxLineDetailField;

//         /// <remarks/>
//         public decimal Amount
//         {
//             get
//             {
//                 return amountField;
//             }
//             set
//             {
//                 amountField = value;
//             }
//         }

//         /// <remarks/>
//         public string DetailType
//         {
//             get
//             {
//                 return detailTypeField;
//             }
//             set
//             {
//                 detailTypeField = value;
//             }
//         }

//         /// <remarks/>
//         public IntuitResponseInvoiceTxnTaxDetailTaxLineTaxLineDetail TaxLineDetail
//         {
//             get
//             {
//                 return taxLineDetailField;
//             }
//             set
//             {
//                 taxLineDetailField = value;
//             }
//         }
//     }

//     /// <remarks/>
//     [System.SerializableAttribute()]
//     [System.ComponentModel.DesignerCategoryAttribute("code")]
//     [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schema.intuit.com/finance/v3")]
//     public partial class IntuitResponseInvoiceTxnTaxDetailTaxLineTaxLineDetail
//     {

//         private byte taxRateRefField;

//         private bool percentBasedField;

//         private byte taxPercentField;

//         private decimal netAmountTaxableField;

//         /// <remarks/>
//         public byte TaxRateRef
//         {
//             get
//             {
//                 return taxRateRefField;
//             }
//             set
//             {
//                 taxRateRefField = value;
//             }
//         }

//         /// <remarks/>
//         public bool PercentBased
//         {
//             get
//             {
//                 return percentBasedField;
//             }
//             set
//             {
//                 percentBasedField = value;
//             }
//         }

//         /// <remarks/>
//         public byte TaxPercent
//         {
//             get
//             {
//                 return taxPercentField;
//             }
//             set
//             {
//                 taxPercentField = value;
//             }
//         }

//         /// <remarks/>
//         public decimal NetAmountTaxable
//         {
//             get
//             {
//                 return netAmountTaxableField;
//             }
//             set
//             {
//                 netAmountTaxableField = value;
//             }
//         }
//     }

//     /// <remarks/>
//     [System.SerializableAttribute()]
//     [System.ComponentModel.DesignerCategoryAttribute("code")]
//     [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schema.intuit.com/finance/v3")]
//     public partial class IntuitResponseInvoiceCustomerRef
//     {

//         private string nameField;

//         private ushort valueField;

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
//         public ushort Value
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

//     /// <remarks/>
//     [System.SerializableAttribute()]
//     [System.ComponentModel.DesignerCategoryAttribute("code")]
//     [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schema.intuit.com/finance/v3")]
//     public partial class IntuitResponseInvoiceBillAddr
//     {

//         private int idField;

//         private string line1Field;

//         private string line2Field;

//         private string cityField;

//         private string countryField;

//         private string countrySubDivisionCodeField;

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


// }