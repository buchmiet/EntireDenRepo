using denSharedLibrary;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace denViewModels;

public class AddLabelPropertyViewModel : ObservableObject, IAsyncDialogViewModel
{
    private string labelName;
    private int topFont;
    private float largeFont;
    private float lesserFont;
    private float bottomFont;
    private int height;
    private int width;
    private int centralLineSpacing;
    private int bottomMargin;
    private int topMargin;
    private bool landscape;

    public string LabelName
    {
        get => labelName;
        set => SetProperty(ref labelName, value);
    }

    public int TopFont
    {
        get => topFont;
        set => SetProperty(ref topFont, value);
    }

    public float LargeFont
    {
        get => largeFont;
        set => SetProperty(ref largeFont, value);
    }

    public float LesserFont
    {
        get => lesserFont;
        set => SetProperty(ref lesserFont, value);
    }

    public float BottomFont
    {
        get => bottomFont;
        set => SetProperty(ref bottomFont, value);
    }

    public int Height
    {
        get => height;
        set => SetProperty(ref height, value);
    }

    public int Width
    {
        get => width;
        set => SetProperty(ref width, value);
    }

    public int CentralLineSpacing
    {
        get => centralLineSpacing;
        set => SetProperty(ref centralLineSpacing, value);
    }

    public int BottomMargin
    {
        get => bottomMargin;
        set => SetProperty(ref bottomMargin, value);
    }

    public int TopMargin
    {
        get => topMargin;
        set => SetProperty(ref topMargin, value);
    }

    public bool Landscape
    {
        get => landscape;
        set => SetProperty(ref landscape, value);
    }

    public ICommand OkCommand { get; }
    public ICommand CancelCommand { get; }

    IDialogService _dialogService;

    public event AsyncEventHandler RequestClose;


       
    public AddLabelPropertyViewModel(IDialogService dialogService)
    {
        _dialogService = dialogService;
        OkCommand = new AsyncRelayCommand(OkExecute);
        CancelCommand = new RelayCommand(CancelExecute);
        LabelName = "New Label";
        TopFont = 14;
        LargeFont = 35;
        LesserFont = 16;
        BottomFont = 12;
        Height = 32;
        Width = 57;
        CentralLineSpacing = 5;
        BottomMargin = 5;
        TopMargin = 5;
        Landscape = true;
    }

    public LabelProperties Result;

    private async Task OkExecute()
    {
        if (string.IsNullOrEmpty(LabelName))
        {
            await _dialogService.ShowMessage(denLanguageResourses.Resources.Warning, denLanguageResourses.Resources.LabelNameCannotBeEmpty);
            return;
        }

        if (LabelPropertiesManager.GetProperty(LabelName) != null)
        {
            var message = string.Format(denLanguageResourses.Resources.LabelNameInUse, LabelName);
            await _dialogService.ShowMessage(denLanguageResourses.Resources.Warning, message);
            return;
        }

        if (!string.IsNullOrEmpty(LabelName)&&(labelName.ToLower().Equals("return label")|| labelName.ToLower().Equals("address label")))
        {
            await _dialogService.ShowMessage(denLanguageResourses.Resources.Warning, denLanguageResourses.Resources.ReservedNamesInformation);
            return;
        }


        if (TopFont <= 0)
        {
            await _dialogService.ShowMessage(denLanguageResourses.Resources.Warning, denLanguageResourses.Resources.TopFontMustBeGreaterThanZero);
            return;
        }

        if (LargeFont <= 0)
        {
            await _dialogService.ShowMessage(denLanguageResourses.Resources.Warning, denLanguageResourses.Resources.LargeFontMustBeGreaterThanZero);
            return;
        }

        if (LesserFont <= 0)
        {
            await _dialogService.ShowMessage(denLanguageResourses.Resources.Warning, denLanguageResourses.Resources.LesserFontMustBeGreaterThanZero);
            return;
        }

        if (BottomFont <= 0)
        {
            await _dialogService.ShowMessage(denLanguageResourses.Resources.Warning, denLanguageResourses.Resources.BottomFontMustBeGreaterThanZero);
            return;
        }
        if (Height <= 0)
        {
            await _dialogService.ShowMessage(denLanguageResourses.Resources.Warning, denLanguageResourses.Resources.HeightMustBeGreaterThanZero);
            return;
        }

        if (Height > 100 && landscape)
        {
            await _dialogService.ShowMessage(denLanguageResourses.Resources.Warning, denLanguageResourses.Resources.HeightInLandscapeOrientation);
            return;
        }

        if (Height > 160)
        {
            await _dialogService.ShowMessage(denLanguageResourses.Resources.Warning, denLanguageResourses.Resources.HeightMustNotBeGreaterThan);
            return;
        }

        if (Width <= 0)
        {
            await _dialogService.ShowMessage(denLanguageResourses.Resources.Warning, denLanguageResourses.Resources.WidthMustBeGreaterThanZero);
            return;
        }
        if (Width >= 100 && !landscape)
        {
            await _dialogService.ShowMessage(denLanguageResourses.Resources.Warning, denLanguageResourses.Resources.WidthInLandscapeMode);
            return;
        }

        if (Width > 160)
        {
            await _dialogService.ShowMessage(denLanguageResourses.Resources.Warning, denLanguageResourses.Resources.WidthMustNotBeGreaterThan160);
            return;
        }

        if (CentralLineSpacing < 0)
        {
            await _dialogService.ShowMessage(denLanguageResourses.Resources.Warning, denLanguageResourses.Resources.CentralLineSpacingMustNotBeNegative);
            return;
        }

        if (BottomMargin < 0)
        {
            await _dialogService.ShowMessage(denLanguageResourses.Resources.Warning, denLanguageResourses.Resources.BottomMarginMustNotBeNegative);
            return;
        }

        if (TopMargin < 0)
        {
            await _dialogService.ShowMessage(denLanguageResourses.Resources.Warning, denLanguageResourses.Resources.TopMarginMustNotBeNegative);
            return;
        }


        // If validation passes, execute the rest of the code to add the label property, etc.
        // For example:
        LabelProperties labelProperties = new LabelProperties
        {
            LabelName = this.LabelName,
            TopFont = this.TopFont,
            LargeFont = this.LargeFont,
            LesserFont = this.LesserFont,
            BottomFont = this.BottomFont,
            Height = this.Height,
            Width = this.Width,
            CentralLineSpacing = this.CentralLineSpacing,
            BottomMargin = this.BottomMargin,
            TopMargin = this.TopMargin,
            Landscape = this.Landscape
        };
        Result = labelProperties;
        RequestClose?.Invoke(this, EventArgs.Empty);
    }


    

    private void CancelExecute()
    {
        RequestClose?.Invoke(this, EventArgs.Empty);
    }
}