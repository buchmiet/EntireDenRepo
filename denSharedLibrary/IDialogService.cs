namespace denSharedLibrary;

public interface IDialogService
{
    Task<bool?> ShowDialog<T>(T viewModel) where T : IAsyncDialogViewModel;

    Task ShowMessage(string title, string message);
    Task<bool> ShowYesNoMessageBox(string title, string message);
}