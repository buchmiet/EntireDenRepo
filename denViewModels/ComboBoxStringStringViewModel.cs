using denSharedLibrary;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using denModels;

namespace denViewModels;

public class ComboBoxStringStringViewModel : IBaseFieldViewModel, INotifyPropertyChanged
{
    public string FieldIdentifier { get; set; }
    public string FieldName { get; set; }
    public FieldType FieldType { get; set; }

    private StringString _initialValue;
    public StringString InitialValue
    {
        get { return _initialValue; }
        set
        {
            _initialValue = value;
            OnPropertyChanged(nameof(InitialValue));
        }
    }

    private StringString _selectedValue;
    public StringString SelectedValue
    {
        get { return _selectedValue; }
        set
        {
            if (_selectedValue != value)
            {
                _selectedValue = value;
                OnPropertyChanged(nameof(SelectedValue));
                OnPropertyChanged(nameof(HasChanged));

                OnValueChanged?.Invoke(_selectedValue);
            }
        }
    }

    private List<StringString> _values;

    public List<StringString> Values
    {
        get { return _values; }
        set
        {
            if (_values != value)
            {
                _values = value;
                OnPropertyChanged(nameof(Values));
            }
        }
    }

    public ComboBoxStringStringViewModel()
    {

    }

    public ComboBoxStringStringViewModel(string fieldName, FieldType fieldType, List<StringString> values, StringString initialValue)
    {
        FieldName = fieldName;
        FieldType = fieldType;
        Values = values;
        InitialValue = initialValue;
        SelectedValue = initialValue;
    }


    public bool HasChanged
    {
        get
        {
            if (SelectedValue == null) return false;
            return InitialValue.Key != SelectedValue.Key;
        }
    }

    public Action<StringString> OnValueChanged { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}