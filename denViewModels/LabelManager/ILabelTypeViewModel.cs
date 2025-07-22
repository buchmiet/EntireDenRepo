using System.Windows.Input;

namespace denViewModels;

public interface ILabelTypeViewModel
{
    ICommand DeleteCommand { get; set; }

    void SetPrinterName(string printerName);
}