using denSharedLibrary;
using System.Windows.Threading;

namespace denWPFSharedLibrary;

public class WpfDispatcherTimer : IDispatcherTimer
{
    private readonly DispatcherTimer _timer = new DispatcherTimer();

    public event EventHandler Tick
    {
        add => _timer.Tick += value;
        remove => _timer.Tick -= value;
    }

    public TimeSpan Interval { get => _timer.Interval; set => _timer.Interval = value; }

    public bool IsEnabled { get => _timer.IsEnabled; set => _timer.IsEnabled = value; }

    public void Start() => _timer.Start();

    public void Stop() => _timer.Stop();
}