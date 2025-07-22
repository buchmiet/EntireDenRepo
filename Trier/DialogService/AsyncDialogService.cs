using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trier.DialogService
{
    public delegate Task AsyncEventHandler(object sender, EventArgs e);
    public interface IAsyncDialogViewModel
    {
        event AsyncEventHandler RequestClose;
    }

    public interface YesNoMessageDialog
    {
        Task<bool> ShowYesNoDialogAsync(string message, string title);
    }

    public interface YesNoCancelMessageDialog
    {
        Task<bool> ShowYesNoCancelDialogAsync(string message, string title);
    }

    public interface TimeOutMessageDialog
    {
        Task<bool> ShowTimeoutDialogAsync(string message, string title);
    }

    public interface IAsyncDialogService
    {
        Task<bool?> ShowDialog<TDialog>(TDialog dialog) where TDialog : IAsyncDialogViewModel;
        Task ShowMessage(string title, string message);
        Task<bool> ShowYesNoMessagePrompt(string title, string message);
        Task<bool> ShowTimeoutDialogAsync(string message, string title);
        Task<bool> ShowYesNoCancelDialogAsync(string message, string title);
    }

}
