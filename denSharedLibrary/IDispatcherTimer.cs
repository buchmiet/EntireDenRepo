namespace denSharedLibrary;

public interface IDispatcherTimer
{
    event EventHandler Tick;
    TimeSpan Interval { get; set; }
    bool IsEnabled { get; set; }
    void Start();
    void Stop();
}