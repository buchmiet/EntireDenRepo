namespace SettingsKeptInFile;

public class SettingsMessenger
{
    private SettingsService _service;
    public SettingsMessenger(SettingsService service)
    {
        _service = service;
    }
    private readonly Dictionary<string, List<Action<string>>> SubscriberActions = new();

    public void Subscribe(string settingName, Action<string> action)
    {

        if (!SubscriberActions.ContainsKey(settingName))
        {
            SubscriberActions[settingName] = [];
        }
        SubscriberActions[settingName].Add(action);
    }

    public void Unsubscribe(string settingName, Action<string> action)
    {

        if (SubscriberActions.ContainsKey(settingName))
        {
            SubscriberActions[settingName].Remove(x => action(x));
            if (!SubscriberActions[settingName].Any())
            {
                SubscriberActions.Remove(settingName);
            }
        }
    }

    public void Publish(string settingName)
    {

        if (SubscriberActions.TryGetValue(settingName, out var subscriberAction))
        {
            var setting = _service.GetSetting(settingName);
            if (setting.IsSuccess)
            {
                var settingValue= setting.GetValue<string>();
                foreach (var action in subscriberAction)
                {
                    action(settingValue);
                }
            }
                
        }
    }
}