using Answers;

namespace SettingsKeptInFile;

public interface ISettingsService
{
    int LocationId { get; }
    SettingsMessenger SettingsMessenger { get; }
    Answer GetSetting(string settingName);
    Dictionary<string, string> GetAllSettings();
    void UpdateSetting(string settingName, string value);
    void UpdateSettings(Dictionary<string, string> mainSettings);
}

public class SettingsService : ISettingsService
{
    private const int _locationID = 1;
    public int LocationId => _locationID;
    public SettingsMessenger SettingsMessenger { get; }

    private Dictionary<string, string> _settings = new();
    private readonly string _settingsFile;
    private readonly string _mainAssemblyPath;

    public SettingsService(string settingsFilePath)
    {
        SettingsMessenger = new SettingsMessenger(this);
        _settingsFile = settingsFilePath;
        _mainAssemblyPath = AppContext.BaseDirectory;
    }

    public Answer GetSetting(string settingName)
    {
        var response = Answer.Prepare($"Getting setting '{settingName}' from settings");
        if (_settings.Count == 0)
        {
            ReadSettings();
        }
        if (_settings.TryGetValue(settingName, out var value))
        {
            return response.WithValue(value);
        }
        return response.Error($"Setting '{settingName}' not found");
    }

    private void ReadSettings()
    {
        string providedPath = Path.Combine(_mainAssemblyPath, _settingsFile);
        if (!File.Exists(providedPath)) return;
        var settingsAsString = File.ReadAllText(providedPath);
        _settings = new Dictionary<string, string>();
        using StringReader reader = new StringReader(settingsAsString);
        string line;
        while ((line = reader.ReadLine()) is not null)
        {
            var columns = line.Split('=');
            _settings.Add(columns[0], columns[1]);
        }
    }

    public Dictionary<string, string> GetAllSettings()
    {
        if (_settings.Count == 0)
        {
            ReadSettings();
        }
        return _settings;
    }


    public void UpdateSetting(string settingName, string value)
    {
        _settings[settingName] = value;
        UpdateSettings(_settings);
        SettingsMessenger.Publish(settingName);
    }

    public void UpdateSettings(Dictionary<string, string> MainSettings)
    {
      
        string filePath = Path.Combine(_mainAssemblyPath, _settingsFile);

       //TODO - check if CFG folder exists

        // Sprawdzamy, czy plik istnieje. Jeśli nie, to go tworzymy.
        if (!File.Exists(filePath))
        {
            File.Create(filePath).Close();
        }
        var settings = GetAllSettings();
        foreach (var entry in MainSettings)
        {
            if (string.IsNullOrEmpty(entry.Value)) continue;
            // Jeśli klucz istnieje i wartość się różni, aktualizujemy wartość.
            // Jeśli klucz nie istnieje, dodajemy nową parę klucz-wartość.
            if (!settings.ContainsKey(entry.Key) || settings[entry.Key] != entry.Value)
            {
                settings[entry.Key] = entry.Value;
            }
        }

        // Teraz zapiszemy ustawienia do pliku.
        using StreamWriter file = new(filePath);
        foreach (KeyValuePair<string, string> entry in settings)
        {
            if (!string.IsNullOrEmpty(entry.Value))
            {
                file.WriteLine($"{entry.Key}={entry.Value}");
            }
        }
    }

}