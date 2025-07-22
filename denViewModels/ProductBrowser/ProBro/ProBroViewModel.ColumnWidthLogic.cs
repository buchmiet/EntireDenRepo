using System.Timers;

namespace denViewModels.ProductBrowser.ProBro;

public partial class ProBroViewModel
{
    private void OnTimerElapsed(object sender, ElapsedEventArgs e)
    {
        SettingsService.UpdateSettings(GetColumnsWidths());
    }

    private void StartOrResetTimer()
    {
        if (_saveTimer == null)
        {
            return;
        }
        if (_saveTimer.Enabled)
        {
            _saveTimer.Stop();
            _saveTimer.Start();
        }
        else
        {
            _saveTimer.Start();
        }
    }

    public Dictionary<string, string> GetColumnsWidths()
    {
        return new Dictionary<string, string>()
        {
            {"column1", Col1Width.ToString()},
            {"column2", Col3Width.ToString()},
            {"column3", Col3Width.ToString()},
            {"column4", Col4Width.ToString()}
        };
    }

    private double _col1Width;

    public double Col1Width
    {
        get => _col1Width;
        set
        {
            if (SetProperty(ref _col1Width, value))
            {
                StartOrResetTimer();
            }
        }
    }

    private double _col2Width;

    public double Col2Width
    {
        get => _col2Width;
        set
        {
            if (SetProperty(ref _col2Width, value))
            {
                StartOrResetTimer();
            }
        }
    }

    private double _col3Width;

    public double Col3Width
    {
        get => _col3Width;
        set
        {
            if (SetProperty(ref _col3Width, value))
            {
                StartOrResetTimer();
            }
        }
    }

    private double _col4Width;

    public double Col4Width
    {
        get => _col4Width;
        set
        {
            if (SetProperty(ref _col4Width, value))
            {
                StartOrResetTimer();
            }
        }
    }

    public void SetColumnsWidths()
    {
        Dictionary<string, string> columnWidths = SettingsService.GetAllSettings();
        if (columnWidths != null)
        {
            if (columnWidths.ContainsKey("column1"))
            {
                Col1Width = Convert.ToDouble(columnWidths["column1"]);
            }
            else
            {
                Col1Width = -1;
            }

            if (columnWidths.ContainsKey("column2"))
            {
                Col2Width = Convert.ToDouble(columnWidths["column2"]);
            }
            else
            {
                Col2Width = -1;
            }

            if (columnWidths.ContainsKey("column3"))
            {
                Col3Width = Convert.ToDouble(columnWidths["column3"]);
            }
            else
            {
                Col3Width = -1;
            }

            if (columnWidths.ContainsKey("column4"))
            {
                Col4Width = Convert.ToDouble(columnWidths["column4"]);
            }
            else
            {
                Col4Width = -1;
            }
        }
    }
}