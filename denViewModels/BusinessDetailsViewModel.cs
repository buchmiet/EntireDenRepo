using denMethods;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using SettingsKeptInFile;

namespace denViewModels;

public class BusinessDetailsViewModel : ObservableObject
{
    private string _businessName;
    private string _businessAddress;
    private bool _isVATRegistered;
    private string _vatNumber;
    private Timer _debounceTimer; // Timer do opóźnionego zapisu
    private ISettingsService SettingsService;

    public BusinessDetailsViewModel(ISettingsService settingsService)
    {
        SettingsService = settingsService;
        _debounceTimer = new Timer(DebounceTimerCallback, null, Timeout.Infinite, Timeout.Infinite);

        var businessNameResponse = SettingsService.GetSetting("businessname");
        if (businessNameResponse.IsSuccess)
        {
            BusinessName = businessNameResponse.GetValue<string>();
        }
        //BusinessName = SettingsService.GetSetting("businessname").GetValue<string>();
        var storedAddressResponse = SettingsService.GetSetting("businessaddress");
        if (storedAddressResponse.IsSuccess)
        {
            var ba = storedAddressResponse.GetValue<string>().Trim();
            BusinessAddress = Base64Converter.DecodeBase64ToString(ba);// Encoding.UTF8.GetString(Convert.FromBase64String(ba)); 
        }
        //var storedAddress = SettingsService.GetSetting("businessaddress").GetValue<string>();
        //if (storedAddress != null)
        //{
        //    var ba = storedAddress.Trim();
        //    BusinessAddress = Base64Converter.DecodeBase64ToString(ba);// Encoding.UTF8.GetString(Convert.FromBase64String(ba)); 
        //}

        var isVatRegisteredResponse = SettingsService.GetSetting("isvatregistered");
        if (isVatRegisteredResponse.IsSuccess)
        {
            var isVATRegistered = isVatRegisteredResponse.GetValue<string>();
            if (bool.TryParse(isVATRegistered, out var isVAT))
            {
                IsVATRegistered = isVAT;
            }
        }

        //var isVATRegistered = SettingsService.GetSetting("isvatregistered").GetValue<string>();
        //if (bool.TryParse(isVATRegistered, out var isVAT))
        //{
        //    IsVATRegistered = isVAT;
        //}

        var vatNumberResponse = SettingsService.GetSetting("vatnumber");
        if (vatNumberResponse.IsSuccess)
        {
            VATNumber = vatNumberResponse.GetValue<string>();
        }

        //VATNumber = SettingsService.GetSetting("vatnumber").GetValue<string>();
        SettingsService = settingsService;
    }

    private void DebounceTimerCallback(object state)
    {
        SaveSettings();
    }

    public string BusinessName
    {
        get => _businessName;
        set
        {
            if (SetProperty(ref _businessName, value))
            {
                DebounceSave();
            }
        }
    }
    public string BusinessAddress
    {
        get => _businessAddress;
        set
        {
            if (SetProperty(ref _businessAddress, value))
            {
                DebounceSave();
            }
        }
    }

    public bool IsVATRegistered
    {
        get => _isVATRegistered;
        set
        {
            if (SetProperty(ref _isVATRegistered, value))
            {
                DebounceSave();
            }
        }
    }

    public string VATNumber
    {
        get => _vatNumber;
        set
        {
            if (SetProperty(ref _vatNumber, value))
            {
                DebounceSave();
            }
        }
    }

    private void DebounceSave()
    {
        _debounceTimer.Change(1000, Timeout.Infinite); // Opóźnienie 2 sekundy przed zapisem
    }

    private void SaveSettings()
    {
        if (BusinessName != null)
        {
            SettingsService.UpdateSetting("businessname", BusinessName);
        }

        if (BusinessAddress != null)
        {
            SettingsService.UpdateSetting("businessaddress", Convert.ToBase64String(Encoding.UTF8.GetBytes(BusinessAddress)));
        }

        SettingsService.UpdateSetting("isvatregistered", IsVATRegistered.ToString());

        if (VATNumber != null)
        {
            SettingsService.UpdateSetting("vatnumber", VATNumber);
        }
    }
}