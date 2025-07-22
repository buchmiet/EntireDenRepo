using denSharedLibrary;
using System.Windows;

namespace denWPFSharedLibrary;

public class DialogService : IDialogService
{
    public async Task<bool> ShowYesNoMessageBox(string title, string message)
    {
        var result = MessageBox.Show(message, title, MessageBoxButton.YesNo);
        return result == MessageBoxResult.Yes;
    }
    public async Task ShowMessage(string title, string message)
    {
        MessageBox.Show(message, title);
    }
    //private System.Type GetWindowType(System.Type viewModelType)
    //{
    //    var windowTypeName = viewModelType.FullName.Replace("ViewModel", "Window");
    //    System.Type windowType = null;
    //    foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
    //    {
    //        windowType = assembly.GetType(windowTypeName);
    //        if (windowType != null)
    //        {
    //            break;
    //        }
    //    }
    //    return windowType;
    //}
    private System.Type GetWindowType(System.Type viewModelType)
    {

        string shortName = viewModelType.FullName;

        int lastDotIndex = shortName.LastIndexOf('.');

        if (lastDotIndex != -1)
        {
            shortName = shortName.Substring(lastDotIndex + 1);
        }

          
          

        var windowShortName = shortName.Replace("ViewModel", "Window");

        // Iteruj przez wszystkie zgromadzenia
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            // Dla każdego zgromadzenia, przeszukaj wszystkie typy
            foreach (var type in assembly.GetTypes())
            {
                if (type.Name == windowShortName)
                {
                    return type;
                }
            }
        }

        return null;
    }



    //public bool? ShowDialog<T>(T viewModel) where T : IAsyncDialogViewModel
    //{
    //    System.Type windowType = GetWindowType(typeof(T));
    //    if (windowType == null)
    //    {
    //        throw new InvalidOperationException($"Nie można znaleźć okna odpowiadającego ViewModelu: {typeof(T)}");
    //    }

    //    var window = (Window)Activator.CreateInstance(windowType);
    //    window.DataContext = viewModel;

    //    viewModel.RequestClose += async (sender, e) =>
    //    {
    //        await Application.Current.Dispatcher.InvokeAsync(() =>
    //        {
    //            window.Close();
    //        });
    //    };
    //    return window.ShowDialog();
    //}

    public async Task<bool?> ShowDialog<T>(T viewModel) where T : IAsyncDialogViewModel
    {
        System.Type windowType = GetWindowType(typeof(T));
        if (windowType == null)
        {
            throw new InvalidOperationException($"Nie można znaleźć okna odpowiadającego ViewModelu: {typeof(T)}");
        }

        var window = (System.Windows.Window)Activator.CreateInstance(windowType);
        window.DataContext = viewModel;

        viewModel.RequestClose += async (sender, e) =>
        {
            await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
            {
                window.Close();
            });
        };

        // Asynchroniczne oczekiwanie, ale w rzeczywistości tylko opakowuje synchroniczne wywołanie.
        bool? result = null;
        await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
        {
            result = window.ShowDialog();
        });
        return result;
    }

    
}