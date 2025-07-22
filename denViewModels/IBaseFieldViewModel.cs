using System.ComponentModel;

namespace denViewModels;

public interface IBaseFieldViewModel
{
    string FieldName { get; set; }
    denModels.FieldType FieldType { get; set; }
    string  FieldIdentifier { get; set; }
    bool HasChanged { get; }
    event PropertyChangedEventHandler PropertyChanged;
}