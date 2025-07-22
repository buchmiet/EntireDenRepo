namespace denSharedLibrary;

public interface IFileDialogService
{
    Task<Stream?> SaveFileAsync(string title, string filter);
    Task<string?> OpenFileAsync(string title, string filter);
}