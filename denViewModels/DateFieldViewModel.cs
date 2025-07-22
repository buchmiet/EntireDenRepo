using CommunityToolkit.Mvvm.ComponentModel;
using denModels;


namespace denViewModels;

public class DateFieldViewModel :ObservableObject,  IBaseFieldViewModel 
{
    public string FieldIdentifier { get; set; }
    public string FieldName { get; set; }
    public FieldType FieldType { get; set; }

    private DateTime? _initialValue;
    public DateTime? InitialValue
    {
        get => _initialValue;
        set => SetProperty(ref _initialValue, value);

    }

    private DateTime? _selectedValue;
    public DateTime? SelectedValue
    {
        get { return _selectedValue; }
        set
        {
            _selectedValue = value;
            OnPropertyChanged(nameof(SelectedValue));
            OnPropertyChanged(nameof(HasChanged));
            OnValueChanged?.Invoke(_selectedValue);
        }
    }

    public bool HasChanged
    {
        get
        {
            if (_selectedValue == null) { return false; }
            return SelectedValue?.Date != InitialValue?.Date;
        }
    }

    public DateFieldViewModel(string fieldName, FieldType fieldType, DateTime initialValue)
    {
        FieldName = fieldName;
        FieldType = fieldType;
        InitialValue = initialValue;
        SelectedValue = initialValue;
    }

    public Action<DateTime?> OnValueChanged { get; set; }

      
}