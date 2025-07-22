using denSharedLibrary;
using System.Windows;

namespace denWPFSharedLibrary;

public class WpfUiThreadInvoker : IUiThreadInvoker
{
    public async Task InvokeOnUiThreadAsync(Func<Task> action)
    {
        await Application.Current.Dispatcher.InvokeAsync(action);
    }
}