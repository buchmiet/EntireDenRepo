using CommunityToolkit.Mvvm.ComponentModel;
using denModels;


namespace denViewModels;

public class TextFieldViewModel : ObservableObject, IBaseFieldViewModel
{
    public string _fieldName;

    public string FieldName
    {
        get=>_fieldName;
        set => SetProperty(ref _fieldName, value);
    }
    public FieldType FieldType { get; set; }

    private string _initialValue;
    public string InitialValue
    {
        get => _initialValue; 
        set =>SetProperty(ref _initialValue, value);
               
            
    }

    private string _selectedValue;
    public string SelectedValue
    {
        get => _selectedValue; 
        set 
        {
            if (SetProperty(ref _selectedValue, value))
            {
                OnPropertyChanged(nameof(SelectedValue));
                OnPropertyChanged(nameof(HasChanged));
                OnValueChanged?.Invoke(_selectedValue);
            }
        }
    }

    public bool HasChanged
    {
        get
        {
            if (_selectedValue == null) { return false; }
            return SelectedValue != InitialValue;
        }
    }

    public string FieldIdentifier { get; set; }


       
    public TextFieldViewModel()
    {

    }

    public Action<string> OnValueChanged { get; set; }

       
}