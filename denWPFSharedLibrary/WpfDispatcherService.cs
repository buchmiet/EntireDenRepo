using denSharedLibrary;
using System.Windows.Threading;

namespace denWPFSharedLibrary;

public class WpfDispatcherService : IDispatcherService
{
    private readonly Dispatcher _dispatcher;

    public WpfDispatcherService(Dispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    public void Invoke(Action action)
    {
        if (_dispatcher.CheckAccess())
            action();
        else
            _dispatcher.Invoke(action);
    }

    public bool CheckAccess()
    {
        return _dispatcher.CheckAccess();
    }

    public TResult Invoke<TResult>(Func<TResult> func)
    {
        if (_dispatcher.CheckAccess())
        {
            return func();
        }
        return _dispatcher.Invoke(func);
    }

    public async Task InvokeTaskAsync(Func<Task> taskFunc)
    {
        if (_dispatcher.CheckAccess())
        {
            await taskFunc();
        }
        else
        {
            await _dispatcher.InvokeAsync(taskFunc);
        }
    }
}