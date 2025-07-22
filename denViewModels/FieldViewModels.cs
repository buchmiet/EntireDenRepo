using denSharedLibrary;
using denModels;

namespace denViewModels;

public static class FieldViewModels
{

    public static IBaseFieldViewModel CreateFieldViewModel(string fieldName, FieldType fieldType, string fieldIdentifier, object initialValue, object values = null)
    {
        IBaseFieldViewModel fieldViewModel = null;

        switch (fieldType)
        {
            case FieldType.TextBox:
                fieldViewModel = new TextFieldViewModel
                {
                    FieldName = fieldName,
                    FieldIdentifier = fieldIdentifier,
                    FieldType = fieldType,
                    InitialValue = initialValue.ToString(),
                    SelectedValue = initialValue.ToString()
                };
                break;

            case FieldType.DatePicker:
                fieldViewModel = new DateFieldViewModel(fieldName, fieldType, (DateTime)initialValue);
                break;

            case FieldType.TextBlock:
                fieldViewModel = new TextBlockFieldModel(fieldName, fieldType, initialValue.ToString());
                break;
            case FieldType.ComboBox:
                if (values == null || !(values is List<StringString> stringStringValues))
                {
                    throw new ArgumentException("Invalid values for ComboBox");
                }
                StringString initialValueStringString = stringStringValues.First(p => p.Key.Equals((initialValue as StringString).Key));
                fieldViewModel = new ComboBoxStringStringViewModel(fieldName, fieldType, stringStringValues, initialValueStringString);
                break;
        }

        return fieldViewModel;
    }
}