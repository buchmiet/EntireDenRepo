using DataServicesNET80;
using denSharedLibrary;
using System.Windows;

namespace denWPFSharedLibrary;

public class WpfMessageDialog : IMessageDialog
{
    private readonly IDialogService _dialogService;

    public WpfMessageDialog(IDialogService dialogService)
    {
        _dialogService = dialogService;
    }

    public async Task<bool> ShowYesNoDialogAsync(string message, string title)
    {
        bool result = false;

        await Application.Current.Dispatcher.InvokeAsync(async() =>
        {
            result =await _dialogService.ShowYesNoMessageBox(title, message);
        });

        return result;
    }
}