namespace denSharedLibrary;

public interface IUiThreadInvoker
{
    Task InvokeOnUiThreadAsync(Func<Task> action);
}