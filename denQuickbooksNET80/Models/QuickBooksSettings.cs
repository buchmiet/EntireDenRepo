namespace denQuickbooksNET80.Models
{
   
    public class QuickBooksSettings
    {
        public string ApiBaseUrl { get; set; }

        public string OauthUrl { get; set; }

        public string CompanyId { get; set; }

        public string MinorVersion { get; set; }

        public int DefaultLocationId { get; set; }

        public string DefaultCurrency { get; set; }

        public string InvalidEmailPlaceholder { get; set; }

        public QuickBooksCredentialsSettings Credentials { get; set; }

        public AccountRefsSettings AccountRefs { get; set; }

        public ItemRefsSettings ItemRefs { get; set; }

        public TaxRefsSettings TaxRefs { get; set; }
    }

    /// <summary>
    /// Ustawienia uwierzytelniania dla QuickBooks.
    /// </summary>
    public class QuickBooksCredentialsSettings
    {
        
        public string EncodedClientSecret { get; set; }
    }

    
    public class AccountRefsSettings
    {
        public int UndepositedFunds { get; set; }
    }

    /// <summary>
    /// Ustawienia referencji do produktów/usług.
    /// </summary>
    public class ItemRefsSettings
    {
        
        public int WatchStrap { get; set; }

        
        public int PostageFee { get; set; }

        
        public int Discount { get; set; }
    }

    /// <summary>
    /// Ustawienia referencji do podatków.
    /// </summary>
    public class TaxRefsSettings
    {
        public int StandardTaxCode { get; set; }

        public int StandardTaxRate { get; set; }

        public int ZeroTaxCode { get; set; }

        public int ZeroTaxRate { get; set; }


        public string NonVatTaxCode { get; set; }
    }
}