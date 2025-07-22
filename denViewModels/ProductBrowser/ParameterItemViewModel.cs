using CommunityToolkit.Mvvm.ComponentModel;
using DataServicesNET80.Models;

namespace denViewModels;

public class ParameterItemViewModel : ObservableObject//INotifyPropertyChanged
{
    // Nazwa cechy, która zostanie wyświetlona jako Label
    public string CechaName { get; set; }
    public static parametervalue NotSetCechaValue = new parametervalue { parameterValueID = -1, parameterID = -1, name = "not set", pos = 0 };
    public parametervalue InitialCechaValue { get; set; }

    // Lista dostępnych wartości cech do wyboru w ComboBox
    private List<parametervalue> _dostepneCechyValues;

    public List<parametervalue> DostepneCechyValues
    {
        get { return _dostepneCechyValues; }
        set
        {
            if (value != null && value.Count > 0)
            {
                value = new List<parametervalue>(value); // Tworzenie nowej kopii listy

                if (value.First().parameterValueID != -1)
                {
                    value.Insert(0, NotSetCechaValue);
                }
            }

            SetProperty(ref _dostepneCechyValues, value);
        }
    }

    public bool HasChanged
    {
        get
        {
            return SelectedCechaValue != InitialCechaValue && SelectedCechaValue != NotSetCechaValue;
        }
    }



    // Wybrana wartość cechy
    private parametervalue _selectedCechaValue;

    public parametervalue SelectedCechaValue
    {
        get { return _selectedCechaValue; }
        set
        {
            if (SetProperty(ref _selectedCechaValue, value))
            {
                OnPropertyChanged(nameof(HasChanged));
            }
        }
    }


}