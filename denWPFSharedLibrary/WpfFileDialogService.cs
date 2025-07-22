using denSharedLibrary;
using Microsoft.Win32;
using System.IO;

namespace denWPFSharedLibrary;

public class WpfFileDialogService : IFileDialogService
{
    public async Task<Stream?> SaveFileAsync(string title, string filter)
    {
        var saveFileDialog = new SaveFileDialog
        {
            Title = title,
            Filter = filter + "|All Files|*.*"
        };
        if (saveFileDialog.ShowDialog() == true)
        {
            return new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write);
        }
        return null;
    }


    public async Task<string?> OpenFileAsync(string title, string filter)
    {
        var openFileDialog = new OpenFileDialog
        {
            Title = title,
            Filter = filter + "|All Files|*.*"
        };
        if (openFileDialog.ShowDialog() == true)
        {
            return openFileDialog.FileName;
        }
        return null;
    }

}