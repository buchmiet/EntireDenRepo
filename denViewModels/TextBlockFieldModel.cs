using CommunityToolkit.Mvvm.ComponentModel;
using denModels;


namespace denViewModels;

public class TextBlockFieldModel :ObservableObject, IBaseFieldViewModel 
{
    public string FieldName { get; set; }
    public string FieldIdentifier { get; set; }
    public FieldType FieldType { get; set; }

    private string _initialValue;
    public string InitialValue
    {
        get =>  _initialValue;
        set => SetProperty(ref _initialValue, value);

    }

    private string _selectedValue;
    public string SelectedValue
    {
        get { return _selectedValue; }
        set
        {
            _selectedValue = value;
            OnPropertyChanged(nameof(SelectedValue));
            //    OnPropertyChanged(nameof(HasChanged));
            OnValueChanged?.Invoke(SelectedValue);
        }
    }

    public bool HasChanged
    {
        get
        {
            //   if (_selectedValue == null) { return false; }
            //    return SelectedValue != InitialValue;
            return false;
        }
    }
    public TextBlockFieldModel(string fieldName, FieldType fieldType, string initialValue)
    {
        FieldName = fieldName;
        FieldType = fieldType;
        InitialValue = initialValue;
        SelectedValue = initialValue;
    }
        

    public Action<string> OnValueChanged { get; set; }

      
}