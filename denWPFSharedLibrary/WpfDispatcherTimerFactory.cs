using denSharedLibrary;

namespace denWPFSharedLibrary;

public class WpfDispatcherTimerFactory : IDispatcherTimerFactory
{
    public IDispatcherTimer Create()
    {
        return new WpfDispatcherTimer();
    }
}