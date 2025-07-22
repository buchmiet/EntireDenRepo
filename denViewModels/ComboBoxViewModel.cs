using CommunityToolkit.Mvvm.ComponentModel;
using denModels;


namespace denViewModels;

public class ComboBoxViewModel : ObservableObject, IBaseFieldViewModel
{
    public string FieldName { get; set; }
    public  FieldType FieldType { get; set; }


    private Idname _initialValue;
    public Idname InitialValue
    {
        get => _initialValue;
        set => SetProperty(ref _initialValue, value);
    }

    private Idname _selectedValue;
    public Idname SelectedValue
    {
        get => _selectedValue;
        set
        {
            if (SetProperty(ref _selectedValue, value))
            {
                OnPropertyChanged(nameof(HasChanged));  // Jeśli posiadasz właściwość "HasChanged"
                OnValueChanged?.Invoke(_selectedValue);
            }
        }
    }

    private List<Idname> _values;
    public List<Idname> Values
    {
        get => _values;
        set => SetProperty(ref _values, value);
    }

    public string FieldIdentifier { get; set; }

    public ComboBoxViewModel()
    {
            
    }

    public bool HasChanged
    {
        get
        {
            if (SelectedValue == null) return false;

            return InitialValue.Id != SelectedValue.Id;
        }
    }

    public Action<Idname> OnValueChanged { get; set; }

      
}