
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CommunityToolkit.Mvvm.ComponentModel;
using denModels;


namespace denViewModels;

public class UniformGridFieldViewModel : ObservableObject, IBaseFieldViewModel
{
    public string FieldIdentifier { get; set; }
    public string FieldName { get; set; }
    public FieldType FieldType { get; set; }

    private ObservableCollection<string> _initialValue;
    public ObservableCollection<string> InitialValue
    {
        get => _initialValue;
        set => SetProperty(ref _initialValue, value);
    }

    private ObservableCollection<string> _values;
    public ObservableCollection<string> SelectedValue
    {
        get { return _values; }
        set
        {
            if (_values != value)
            {
                _values = value;
                OnPropertyChanged(nameof(SelectedValue));
                OnPropertyChanged(nameof(HasChanged));

                // Wywołanie akcji OnValueChanged
                OnValueChanged?.Invoke(_values);
            }
        }
    }

    public UniformGridFieldViewModel()
    {
        SelectedValue = new ObservableCollection<string>();
        SelectedValue.CollectionChanged += Values_CollectionChanged;
    }




    private void Values_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(SelectedValue));
        OnPropertyChanged(nameof(HasChanged));

        // Wywołanie akcji OnValueChanged
        OnValueChanged?.Invoke(SelectedValue);
    }

    public bool HasChanged
    {
        get
        {
            return !InitialValue.SequenceEqual(SelectedValue);
        }
    }

    public Action<ObservableCollection<string>> OnValueChanged { get; set; }

     
}