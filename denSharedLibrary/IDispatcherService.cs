namespace denSharedLibrary;

public interface IDispatcherService
{
    void Invoke(Action action);
    bool CheckAccess();
    TResult Invoke<TResult>(Func<TResult> func);
    Task InvokeTaskAsync(Func<Task> taskFunc);
}