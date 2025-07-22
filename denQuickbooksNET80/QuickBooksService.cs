using AmazonSPAPIClient;
using DataServicesNET80.DatabaseAccessLayer;
using DataServicesNET80.Models;
using denModels.OauthApi;
using denQuickbooksNET80;
using denQuickbooksNET80.Models;
using denQuickBooksNET80.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OauthApi;
using Serilog;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.RegularExpressions;
using static denQuickBooksNET80.Models.QBCustomer;
using static denQuickBooksNET80.Models.QBInvoice;

namespace denQuickbooksNET80
{
    public interface IQuickBooksService
    {
        HttpRequestMessage GetRefreshHeader(OauthTokenObject weToken);

        KeyValuePair<string, string> String2RefreshTokenAndAccessToken(string inputString, Action<string> pisz);

        Task<Invoice> UpdateShippingFee(int id, decimal newShippingFee, Action<string> pisz);

        Task<Invoice> GetAlternativeInvoice(int id, Action<string> pisz);

        Task<T> GetQuickBooksObject<T>(string objectId, string resourceType, Action<string> pisz);

        Task<bool> DeleteInvoice(int id, Action<string> pisz);

        Task DeletePayment(QuickBooksService.PreDelRet delret, Action<string> pisz);

        Task<QuickBooksService.QBCustomerWithID> AddCustomerToQuickbooks(customer custo, billaddr addr, Action<string> pisz);

        Task<Invoice> ConvertMyInvoiceToQuickbooksInvoice(Complete completeOrder, orderitemtype itemType, Action<string> write);

        Task<Invoice> AddInvoice(Invoice inv, Action<string> pisz, decimal ExchangeRate);
   
    };
    }

    public class QuickBooksService : IQuickBooksService
    {
        private int locationID;
        private readonly ILogger<QuickBooksService> _logger;
        public static string ServiceName= "quickbooks";
        private readonly QuickBooksSettings _quickbooksSettings;

    public QuickBooksService(IHttpClientFactory httpClientFactory, IOauthService refreshTokenService, IDatabaseAccessLayer databaseAccessLayer, ILogger<QuickBooksService> logger, IOptions<QuickBooksSettings> quickbooksSettings)
        {
            _quickbooksSettings= quickbooksSettings.Value;
             _logger = logger;
            var httpClient = httpClientFactory.CreateClient(ServiceName);
            _refreshTokenService = refreshTokenService;
            var tf = new OauthTokenHandler
            {
                RequestUrl = "https://quickbooks.api.intuit.com/",
                TokenRetriever = databaseAccessLayer.GetQuickBooksToken,
                TokenSetter = databaseAccessLayer.UpdateQuickbooksToken,
                Client = httpClient,
                GetRefreshHeader = GetRefreshHeader,
                ConvertResponseStringToToken = String2RefreshTokenAndAccessToken,
                GetRequestHeaders = () => new Dictionary<string, string>()
            };
            _refreshTokenService.AddOauthService(tf, ServiceName);
            
            locationID = 1;
        }

        public HttpRequestMessage GetRefreshHeader(OauthTokenObject weToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _quickbooksSettings.ApiBaseUrl)
            {
                Content = new FormUrlEncodedContent([
                    new KeyValuePair<string, string>("grant_type", "refresh_token"),
                    new KeyValuePair<string, string>("refresh_token", weToken.RefreshToken)
                ])
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic",_quickbooksSettings.Credentials.EncodedClientSecret);
            return request;
        }

        public KeyValuePair<string, string> String2RefreshTokenAndAccessToken(string inputString, Action<string> pisz)
        {
            _ = new JsonSerializerOptions
            {
                IncludeFields = true,
            };
            var tok = JsonConvert.DeserializeObject<AouthRefreshTokenResponse>(inputString);
            return new KeyValuePair<string, string>(tok.refresh_token, tok.access_token);
        }

        private readonly IOauthService _refreshTokenService;

        public async Task<Invoice> UpdateShippingFee(int id, decimal newShippingFee, Action<string> pisz)
        {
            //this returns null sometimes
            Invoice invoice = (await GetAlternativeInvoice(id, pisz));
            int LinkePaymentId = Convert.ToInt32(invoice.LinkedTxn[0].TxnId);
            int shippingLineIndex = -1;
          
            decimal taxRate = 0;

            var staracalosc = Convert.ToDecimal(invoice.TotalAmt);
            decimal staryPostage = 0;
            var staraCaloscBezWysylki = staracalosc - staryPostage;
            var nowaCalosc = Math.Round(staraCaloscBezWysylki + newShippingFee, 2);
            var nowaCalosPreTax = Math.Round(nowaCalosc / (1 + taxRate / 100), 2);

            if (invoice.TxnTaxDetail.TotalTax == 0)
            {
                taxRate = 0;
            }
            else
            {
                taxRate = invoice.TxnTaxDetail.TaxLine[0].TaxLineDetail.TaxPercent;
                invoice.TxnTaxDetail.TotalTax = (float)(nowaCalosc - nowaCalosPreTax);
                invoice.TxnTaxDetail.TaxLine[0].TaxLineDetail.NetAmountTaxable = (float)nowaCalosPreTax;
                invoice.TxnTaxDetail.TaxLine[0].Amount = (float)(nowaCalosc - nowaCalosPreTax);
            }

            var lines = invoice.Line.Where(line => line.SalesItemLineDetail != null && line.SalesItemLineDetail.ItemRef.name.ToLower() != "discount").ToArray();
            invoice.Line = lines;
            if (newShippingFee == 0)
            {
                lines = invoice.Line.Where(line => line.Description.ToLower() != "postage fee").ToArray();
                invoice.Line = lines;
            }
            else
            {
                for (int i = 0; i < invoice.Line.Length; i++)
                {
                    if (invoice.Line[i].DetailType == "SalesItemLineDetail" &&
                        invoice.Line[i].SalesItemLineDetail.ItemRef.value ==_quickbooksSettings.ItemRefs.PostageFee.ToString())
                    {
                        shippingLineIndex = i;
                    }
                }
                if (shippingLineIndex == -1)
                {
                    Line shippingLine = new();
                    if (taxRate == 0)
                    {
                        shippingLine = AddPostage(newShippingFee.ToString(), false);
                    }
                    else
                    {
                        shippingLine = AddPostage(newShippingFee.ToString(), true);
                    }
                    var newLines = new List<Line>(invoice.Line)
                    {
                        shippingLine
                    };
                    invoice.Line = newLines.ToArray();
                }
                else
                {
                    var np = Math.Round((newShippingFee / (1 + taxRate / 100)), 2);
                    invoice.Line[shippingLineIndex].SalesItemLineDetail.UnitPrice = (float)np;
                    invoice.Line[shippingLineIndex].Amount = (float)np;
                }
            }
            invoice.TotalAmt = (float)nowaCalosc;

            string JsonData = System.Text.Json.JsonSerializer.Serialize(invoice);
            var zwro = await _refreshTokenService.GetResponseFromApiCallWithWithInput(ServiceName,
                OauthRequestType.POST, "v3/company/" +_quickbooksSettings.CompanyId+ "/invoice?minorversion=" + _quickbooksSettings.MinorVersion, JsonData, pisz, locationID, false);
           
            var newInvoice = JsonConvert.DeserializeObject<QBInvoice.Rootobject>(zwro.ResponseString);
            var gaaw = (await _refreshTokenService.GetResponseFromApiCall(ServiceName, "v3/company/" + _quickbooksSettings.CompanyId + "/payment/" + LinkePaymentId, "minorversion=69", pisz, locationID, false)).ResponseString;
            QBPayment.Payment txn = System.Text.Json.JsonSerializer.Deserialize<QBPayment.Rootobject>(gaaw).Payment;
            txn.TotalAmt = (float)nowaCalosc;
            txn.Line[0].Amount = (float)nowaCalosc;
            JsonData = System.Text.Json.JsonSerializer.Serialize(txn);
            await _refreshTokenService.GetResponseFromApiCallWithWithInput(ServiceName,
                OauthRequestType.POST, "/v3/company/" + _quickbooksSettings.CompanyId + "/payment?minorversion=" + _quickbooksSettings.MinorVersion, JsonData, pisz, locationID, false);
            return newInvoice.Invoice;
        }

        

        public class PreDelRet
        {
            public string SyncToken { get; set; }
            public string Id { get; set; }
        }

        public class InvoiceWrapper
        {
            public Invoice Invoice { get; set; }
            public string Time { get; set; }
        }

        public async Task<Invoice> GetAlternativeInvoice(int id, Action<string> pisz)
        {
            var zwro = await _refreshTokenService.GetResponseFromApiCall(ServiceName, "v3/company/" + _quickbooksSettings.CompanyId + "/invoice/" + id.ToString(), "minorversion=69", pisz, locationID, false);

            InvoiceWrapper invoiceWrapper = JsonConvert.DeserializeObject<InvoiceWrapper>(zwro.ResponseString);
            return invoiceWrapper.Invoice;
        }

        public async Task<T> GetQuickBooksObject<T>(string objectId, string resourceType, Action<string> pisz)
        {
            var response = await _refreshTokenService.GetResponseFromApiCall(
                ServiceName,
                $"v3/company/{_quickbooksSettings.CompanyId}/{resourceType}/{objectId}",
                "minorversion=" + _quickbooksSettings.MinorVersion,
                pisz,
                locationID, false);

            T returnResponse;
            try
            {
                returnResponse = JsonConvert.DeserializeObject<T>(response.ResponseString);
            }
            catch (Exception ex)
            {
                Log.Error($"error while deserializing object :{ex.Message} and {ex.InnerException} and string : {response.ResponseString}");
                returnResponse = default;
            }
            return returnResponse;
        }

        public async Task<bool> DeleteInvoice(int id, Action<string> pisz)
        {
            QBInvoice.Rootobject inv = await GetQuickBooksObject<QBInvoice.Rootobject>(id.ToString(), "invoice", pisz);
           if (inv.Invoice == null)
            {
                return false;
            }

            var pid = inv.Invoice.LinkedTxn.First().TxnId;
            var gu = await GetQuickBooksObject<QBPayment.Rootobject>(pid, "payment", pisz);

            var paymentId = new PreDelRet
            {
                Id = gu.Payment.Id,
                SyncToken = gu.Payment.SyncToken
            };
            await DeletePayment(paymentId, pisz);
            var koko2 = new PreDelRet
            {
                Id = inv.Invoice.Id,
                SyncToken = inv.Invoice.SyncToken
            };
            var inputJSON = JsonConvert.SerializeObject(koko2);
            _ = await _refreshTokenService.GetResponseFromApiCallWithWithInput(ServiceName,
                OauthRequestType.POST, "v3/company/" + _quickbooksSettings.CompanyId + "/invoice?operation=delete&minorversion=" + _quickbooksSettings.MinorVersion, inputJSON, pisz, locationID, false);
            return true;
        }

        public async Task DeletePayment(PreDelRet delret, Action<string> pisz)
        {
            var inputJSON = JsonConvert.SerializeObject(delret);
            var gu = await _refreshTokenService.GetResponseFromApiCallWithWithInput(ServiceName,
                OauthRequestType.POST, "v3/company/" + _quickbooksSettings.CompanyId + "/payment?operation=delete&minorversion="+_quickbooksSettings.MinorVersion, inputJSON, pisz, locationID, false);
        }

        

        public class QBCustomerWithID
        {
            public int ID;
            public Customer customer;
        }

        public async Task<QBCustomerWithID> AddCustomerToQuickbooks(customer custo, billaddr addr, Action<string> pisz)
        {
            var zwrotka = new QBCustomerWithID();
            var cust = new Customer
            {
                CompanyName = Translit(custo.CompanyName),
                FamilyName = Translit(custo.FamilyName),
                GivenName = Translit(custo.GivenName),
                BillAddr = new QBCustomer.Billaddr
                {
                    Line1 = Translit(addr.Line1),
                    Line2 = Translit(addr.Line2),
                    City = Translit(addr.City),
                    CountrySubDivisionCode = Translit(addr.CountrySubDivisionCode),
                    Country = addr.CountryCode,
                    PostalCode = addr.PostalCode
                },
                PrimaryEmailAddr = new Primaryemailaddr
                {
                    Address = custo.Email
                },
                PrimaryPhone = new Primaryphone
                {
                    FreeFormNumber = custo.Phone
                },
                CurrencyRef = new QBCustomer.Currencyref
                {
                    value = custo.currency
                }
            };
            
            string query = $"query=select * from customer where PrimaryEmailAddr='{custo.Email}'";
            var zwro = await _refreshTokenService.GetResponseFromApiCall(ServiceName, "v3/company/" + _quickbooksSettings.CompanyId + "/query", query, pisz, locationID, false).ConfigureAwait(false);
            var queryResponse = System.Text.Json.JsonSerializer.Deserialize<QBCustomerQuery.Rootobject>(zwro.ResponseString);
            if (queryResponse.QueryResponse.Customer != null)
            {
                zwrotka.ID = Convert.ToInt32(queryResponse.QueryResponse.Customer[0].Id);
                zwrotka.customer = (await GetQuickBooksObject<QBCustomer.Rootobject>(queryResponse.QueryResponse.Customer[0].Id, "customer", pisz).ConfigureAwait(false)).Customer;
                return zwrotka;
            }
            //OR IS NOT SUPPORTED IN QUICKBOOKS QUERY LANGUAGE!
            query = $"query=select * from customer where DisplayName = '{GetDisplayName(cust)}'";
            zwro = await _refreshTokenService.GetResponseFromApiCall(ServiceName, "v3/company/" + _quickbooksSettings.CompanyId + "/query", query, pisz, locationID, false).ConfigureAwait(false);
            queryResponse = System.Text.Json.JsonSerializer.Deserialize<QBCustomerQuery.Rootobject>(zwro.ResponseString);
            if (queryResponse.QueryResponse.Customer != null)
            {
                zwrotka.ID = Convert.ToInt32(queryResponse.QueryResponse.Customer[0].Id);
                zwrotka.customer = (await GetQuickBooksObject<QBCustomer.Rootobject>(queryResponse.QueryResponse.Customer[0].Id, "customer", pisz).ConfigureAwait(false)).Customer;
                return zwrotka;
            }

            Regex _regex = new Regex("^((([a-z]|\\d|[!#\\$%&'\\*\\+\\-\\/=\\?\\^_`{\\|}~]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])" +
                "+(\\.([a-z]|\\d|[!#\\$%&'\\*\\+\\-\\/=\\?\\^_`{\\|}~]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])+)*)|((\\x22)" +
                "((((\\x20|\\x09)*(\\x0d\\x0a))?(\\x20|\\x09)+)?(([\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x7f]|\\x21|[\\x23-\\x5b]|[\\x5d-\\x7e]|" +
                "[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])|(\\\\([\\x01-\\x09\\x0b\\x0c\\x0d-\\x7f]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\u" +
                "FDF0-\\uFFEF]))))*(((\\x20|\\x09)*(\\x0d\\x0a))?(\\x20|\\x09)+)?(\\x22)))@((([a-z]|\\d|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])|" +
                "(([a-z]|\\d|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])([a-z]|\\d|-|\\.|_|~|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])*([a-z]|\\d|" +
                "[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])))\\.)+(([a-z]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])|(([a-z]|[\\u00A0-\\uD7FF\\uF900" +
                "-\\uFDCF\\uFDF0-\\uFFEF])([a-z]|\\d|-|\\.|_|~|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])*([a-z]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFF" +
                "EF])))\\.?$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled);
            var dobry = _regex.IsMatch(cust.PrimaryEmailAddr.Address);
            if (!dobry)
            {
                cust.PrimaryEmailAddr.Address = "no@valiedemailadres.com";
            }
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            string JsonData = JsonConvert.SerializeObject(cust, settings);
            var newCustomer = await _refreshTokenService.GetResponseFromApiCallWithWithInput(ServiceName,
                OauthRequestType.POST, "v3/company/" + _quickbooksSettings.CompanyId + "/customer?minorversion=" + _quickbooksSettings.MinorVersion, JsonData, pisz, locationID, false).ConfigureAwait(false);
            QBCustomer.Rootobject trrxx = JsonConvert.DeserializeObject<QBCustomer.Rootobject>(newCustomer.ResponseString);
            zwrotka.ID = Convert.ToInt32(trrxx.Customer.Id);
            zwrotka.customer = trrxx.Customer;
            return zwrotka;
        }

        private Line AddPostage(string c, bool vatowac)
        {
            c ??= "0";
            decimal vat = 1.2m;
            var linia = new Line
            {
                SalesItemLineDetail = new Salesitemlinedetail()
            };
            if (!vatowac)
            {
                vat = 1;
                linia.SalesItemLineDetail.TaxCodeRef = new Taxcoderef { value = _quickbooksSettings.TaxRefs.NonVatTaxCode };
            }
            else
            {
                linia.SalesItemLineDetail.TaxInclusiveAmt = c;
                linia.SalesItemLineDetail.TaxCodeRef = new Taxcoderef { value = _quickbooksSettings.TaxRefs.StandardTaxCode.ToString() };
            }
            decimal cena = Math.Round(Convert.ToDecimal(c) / vat, 2);
            linia.Description = "Postage fee";
            linia.Amount = (float)cena;
            linia.DetailType = "SalesItemLineDetail";

            linia.SalesItemLineDetail.ItemRef = new Itemref
            {
                name = "Postage fee",
                value =_quickbooksSettings.ItemRefs.PostageFee.ToString(),
            };
            linia.SalesItemLineDetail.Qty = 1;
            linia.SalesItemLineDetail.UnitPrice = (float)cena;
            return linia;
        }

        public async Task<Invoice> ConvertMyInvoiceToQuickbooksInvoice(Complete completeOrder, orderitemtype itemType, Action<string> write)
        {
            if (completeOrder?.Order == null || completeOrder.OrderItems == null)
            {
                throw new ArgumentNullException(nameof(completeOrder));
            }
            var invoice = new Invoice();
            var customer = await AddCustomerToQuickbooks(completeOrder.Customer, completeOrder.BillAddr, write);
            invoice.CustomerRef = new Customerref { value = customer.ID.ToString(), name = customer.customer.DisplayName };
            invoice.TotalAmt = (float)completeOrder.Order.saletotal;
            invoice.HomeTotalAmt = completeOrder.Order.saletotal.ToString();
            invoice.TxnDate = completeOrder.Order.paidOn.ToString();
            invoice.CurrencyRef = new QBInvoice.Currencyref() { value = completeOrder.Order.salecurrency };
            decimal vat = completeOrder.Order.VAT ? 1.2m : 1;
            invoice.TxnTaxDetail = new Txntaxdetail { TotalTax = completeOrder.Order.VAT ? 1 : 0 };
            List<Line> lines = [];
            foreach (var orderItem in completeOrder.OrderItems)
            {
                if (orderItem.OrderItemTypeId == itemType.OrderItemTypeId)
                {
                    lines.Add(CreateInvoiceLine(orderItem, vat, completeOrder.Order.VAT));
                }
            }
            AddPostageToLines(lines, completeOrder.Order.postagePrice, completeOrder.Order.VAT);
            invoice.Line = lines.ToArray();
            invoice.ExchangeRate = completeOrder.Order.xchgrate;
            return invoice;

            Line CreateInvoiceLine(orderitem orderItem, decimal vat, bool isVatApplicable)
            {
                var line = new Line();
                decimal price = Math.Round(orderItem.price / vat, 2);
                decimal total = price * orderItem.quantity;
                line.Description = orderItem.itemName;
                line.Amount = (float)total;
                line.DetailType = "SalesItemLineDetail";
                line.SalesItemLineDetail = new Salesitemlinedetail();
                if (isVatApplicable)
                {
                    line.SalesItemLineDetail.TaxInclusiveAmt = (orderItem.price * orderItem.quantity).ToString();
                    line.SalesItemLineDetail.TaxCodeRef = new Taxcoderef { value = _quickbooksSettings.TaxRefs.StandardTaxCode.ToString() };
                }
                else
                {
                    line.SalesItemLineDetail.TaxCodeRef = new Taxcoderef() { value =_quickbooksSettings.TaxRefs.NonVatTaxCode };
                }
                line.SalesItemLineDetail.UnitPrice = (float)price;
                line.SalesItemLineDetail.ItemRef = new Itemref
                {
                    name = orderItem.itemName,
                    value =_quickbooksSettings.ItemRefs.WatchStrap.ToString(),
                };
                line.SalesItemLineDetail.Qty = orderItem.quantity;
                return line;
            }

            void AddPostageToLines(List<Line> lines, decimal? postagePrice, bool isVatApplicable)
            {
                if (postagePrice.HasValue && postagePrice.Value != 0)
                {
                    lines.Add(AddPostage(postagePrice.Value.ToString(), isVatApplicable));
                }
                else
                {
                    lines.Add(AddPostage("0", false));
                }
            }
        }

        public async Task<Invoice> AddInvoice(Invoice inv, Action<string> pisz, decimal ExchangeRate)
        {
            
            inv.TxnDate = DateTime.Parse(inv.TxnDate, CultureInfo.CurrentCulture).ToString("yyyy-MM-dd");

            if (inv.TxnTaxDetail.TotalTax != 0)
            {
                inv.TxnTaxDetail = new Txntaxdetail
                {
                    TxnTaxCodeRef = new Txntaxcoderef(),
                    TotalTax = 0
                };
                var tl = new Taxline[1];
                tl[0] = new Taxline
                {
                    DetailType = "TaxLineDetail",
                    Amount = 0,
                    TaxLineDetail = new Taxlinedetail
                    {
                        TaxRateRef = new Taxrateref { value = _quickbooksSettings.TaxRefs.StandardTaxRate.ToString() }
                    }
                };
                inv.TxnTaxDetail.TaxLine = tl;
                FixNumbers();
            }
            if (!inv.CurrencyRef.value.Equals(_quickbooksSettings.DefaultCurrency))
            {
                inv.ExchangeRate = ExchangeRate;
            }

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            string JsonData = JsonConvert.SerializeObject(inv, settings);
            var jsonString = await _refreshTokenService.GetResponseFromApiCallWithWithInput(ServiceName,
                OauthRequestType.POST,
                "/v3/company/" + _quickbooksSettings.CompanyId + "/invoice?minorversion=" + _quickbooksSettings.MinorVersion, JsonData, pisz, locationID, false);

            if (jsonString.ResponseString.ToLower().Contains("error"))
            {
                _logger.LogError($"An error occurred while adding invoice: {jsonString.ResponseString}");
                return null;
            }

            Invoice ga;
            try
            {
                _logger.LogInformation("Attempting to Serialize invoice.");
                ga = JsonConvert.DeserializeObject<QBInvoice.Rootobject>(jsonString.ResponseString).Invoice;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the invoice.");
                return null;
            }
            _= await CreateAndAddPayment(inv, ga.Id, pisz);
            return ga;

            void FixNumbers()
            {
                decimal cena = 0;
                decimal beztaxu = 0;
                foreach (var linijka in inv.Line)
                {
                    cena += Convert.ToDecimal(linijka.SalesItemLineDetail.TaxInclusiveAmt);
                    decimal tta = Math.Round(Convert.ToDecimal(linijka.SalesItemLineDetail.UnitPrice), 2, MidpointRounding.AwayFromZero);
                    beztaxu += tta * Convert.ToDecimal(linijka.SalesItemLineDetail.Qty);
                }

                var roznica = Math.Round(cena - Convert.ToDecimal(inv.TotalAmt), 2);

                if (roznica > 0)
                {
                    List<Line> li =
                    [
                        ..inv.Line,
                        AddDiscount(inv, roznica)
                    ];
                    inv.Line = li.ToArray();
                }
                var tt = Math.Round(cena - beztaxu, 2);

                inv.TxnTaxDetail.TotalTax = (float)tt;
                inv.TxnTaxDetail.TaxLine[0].Amount = (float)tt;
                inv.TxnTaxDetail.TaxLine[0].TaxLineDetail.NetAmountTaxable = (float)Math.Round(beztaxu, 2);
            }
            Line AddDiscount(Invoice inv, decimal discount)
            {
                var dk = (discount * -1);
                var linia = new Line
                {
                    Description = "Discount",
                    Amount = (float)dk,
                    DetailType = "SalesItemLineDetail",
                    SalesItemLineDetail = new Salesitemlinedetail
                    {
                        TaxCodeRef = new Taxcoderef()
                        {
                            value =_quickbooksSettings.TaxRefs.ZeroTaxCode.ToString()
                        },
                        TaxInclusiveAmt = dk.ToString(),
                        UnitPrice = (float)dk,
                        ItemRef = new Itemref()
                        {
                            name = "Discount",
                            value =_quickbooksSettings.ItemRefs.Discount.ToString()
                        },
                        Qty = 1
                    }
                };
                var tl = new List<Taxline>(inv.TxnTaxDetail.TaxLine)
                {
                    new()
                    {
                        Amount = 0,
                        TaxLineDetail = new Taxlinedetail
                        {
                            TaxRateRef = new Taxrateref
                            {
                                value =_quickbooksSettings.TaxRefs.ZeroTaxRate.ToString()
                            },
                            NetAmountTaxable =(float) dk
                        }
                    }
                };
                inv.TxnTaxDetail.TaxLine = tl.ToArray();
                return linia;
            }
            async Task<int> CreateAndAddPayment(Invoice inv, string id, Action<string> pisz)
            {
                var tx = new QBPayment.Payment
                {
                    CustomerRef = new QBPayment.Customerref
                    {
                        name = inv.CustomerRef.name,
                        value = inv.CustomerRef.value
                    },
                    TotalAmt = inv.TotalAmt,
                    TxnDate = inv.TxnDate,
                    DepositToAccountRef = new QBPayment.Deposittoaccountref
                    {
                        value =_quickbooksSettings.AccountRefs.UndepositedFunds.ToString()
                    }
                };
                tx.Line = new QBPayment.Line[1];
                tx.Line[0] = new QBPayment.Line() { Amount = inv.TotalAmt, LinkedTxn = new QBPayment.Linkedtxn[1] };
                tx.Line[0].LinkedTxn[0] = new QBPayment.Linkedtxn
                {
                    TxnId = id,
                    TxnType = "Invoice"
                };
                string JsonData = JsonConvert.SerializeObject(tx);

                return Convert.ToInt32(
                    System.Text.Json.JsonSerializer.Deserialize<QBPayment.Payment>(
                        (await _refreshTokenService.GetResponseFromApiCallWithWithInput(ServiceName, OauthRequestType.POST,
                                "v3/company/" + _quickbooksSettings.CompanyId + "/payment?minorversion=" + _quickbooksSettings.MinorVersion, JsonData, pisz, locationID, false)
                            .ConfigureAwait(false)).ResponseString
                    ).Id);
            }
        }
        public string GetDisplayName(Customer customer)
        {
            var nameParts = new List<string>();

            if (!string.IsNullOrWhiteSpace(customer.GivenName))
            {
                nameParts.Add(customer.GivenName);
            }

            if (!string.IsNullOrWhiteSpace(customer.MiddleName))
            {
                nameParts.Add(customer.MiddleName);
            }

            if (!string.IsNullOrWhiteSpace(customer.FamilyName))
            {
                nameParts.Add(customer.FamilyName);
            }

            return string.Join(" ", nameParts);
        }
    public string Translit(string str)
        {
            if (str == null) { return ""; }
            string[] lat_up = ["A", "B", "V", "G", "D", "E", "Yo", "Zh", "Z", "I", "Y", "K", "L", "M", "N", "O", "P", "R", "S", "T", "U", "F", "Kh", "Ts", "Ch", "Sh", "Shch", "\"", "Y", "'", "E", "Yu", "Ya"
            ];
            string[] lat_low = ["a", "b", "v", "g", "d", "e", "yo", "zh", "z", "i", "y", "k", "l", "m", "n", "o", "p", "r", "s", "t", "u", "f", "kh", "ts", "ch", "sh", "shch", "\"", "y", "'", "e", "yu", "ya"
            ];
            string[] rus_up = ["А", "Б", "В", "Г", "Д", "Е", "Ё", "Ж", "З", "И", "Й", "К", "Л", "М", "Н", "О", "П", "Р", "С", "Т", "У", "Ф", "Х", "Ц", "Ч", "Ш", "Щ", "Ъ", "Ы", "Ь", "Э", "Ю", "Я"
            ];
            string[] rus_low = ["а", "б", "в", "г", "д", "е", "ё", "ж", "з", "и", "й", "к", "л", "м", "н", "о", "п", "р", "с", "т", "у", "ф", "х", "ц", "ч", "ш", "щ", "ъ", "ы", "ь", "э", "ю", "я"
            ];
            for (int i = 0; i <= 32; i++)
            {
                str = str.Replace(rus_up[i], lat_up[i]);
                str = str.Replace(rus_low[i], lat_low[i]);
            }
            return str;
        }

}
