namespace DataServicesNET80;

//public interface IMessageDialog
//{
//    Task<DialogResult> ShowAsync(string message, string title, DialogButtons buttons);
//}
public interface IMessageDialog
{
    Task<bool> ShowYesNoDialogAsync(string message, string title);
}