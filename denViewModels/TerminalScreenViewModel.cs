using denSharedLibrary;

using System.Collections.ObjectModel;

using System.Collections.Concurrent;
using CommunityToolkit.Mvvm.ComponentModel;
using Color = denSharedLibrary.Color;

namespace denViewModels;

public class TerminalScreenViewModel : ObservableObject, ITerminalScreenViewModel
{
    public ObservableCollection<ColourTextBlockViewModel> ColourTextBlockViewModels
    {
        get => _colourTextBlockViewModels;
        set => SetProperty(ref _colourTextBlockViewModels, value);
    }
    private ObservableCollection<ColourTextBlockViewModel> _colourTextBlockViewModels;
    private bool _scrollToEnd;


    public bool ScrollToEnd
    {
        get => _scrollToEnd;
        set => SetProperty(ref _scrollToEnd, value);
    }

    public List<Theme> Themes;
    private readonly ConcurrentQueue<Tuple<string, int>> _textQueue = new ConcurrentQueue<Tuple<string, int>>();

    private Theme CurrenTheme;

    public Theme GetTheme() => CurrenTheme;


    public TerminalScreenViewModel()
    {
        ColourTextBlockViewModels = new ObservableCollection<ColourTextBlockViewModel>();
        Themes = ThemesActions.MYThemes.ToList();       
        CurrenTheme = Themes.First();
        InitializeTextBlocks(CurrenTheme);
        Task.Run(() => ProcessTextQueue());
    }


    

    public void SetTheme(string theme)
    {
        CurrenTheme = Themes.First(p => p.Name.Equals(theme));
        InitializeTextBlocks(CurrenTheme);
    }

    public void NextTheme()
    {
        var currentIndex = Themes.IndexOf(CurrenTheme);

        if (currentIndex >= 0 && currentIndex < Themes.Count - 1)
        {
            // If CurrentTheme is found in _themes and is not the last one, set the next one
            CurrenTheme = Themes[currentIndex + 1];
        }
        else
        {
            // If CurrentTheme is not found or it's the last one, set the first one
            CurrenTheme = ThemesActions.MYThemes[0];
        }
        InitializeTextBlocks(CurrenTheme);
    }

    public Color GetBorder()
    {
        return CurrenTheme.Border;
    }


    private async Task ProcessTextQueue()
    {
        bool cursorOn = false;
        while (true)
        {
            if (_textQueue.TryDequeue(out var item))
            {

                cursorTextBlock.RemoveCursor(CurrenTheme.CursorOff, CurrenTheme.CursorOn);


                // Dodawanie tekstu z kolejki
                string text = item.Item1;
                int color = item.Item2;

                char[] characters = text.ToCharArray();
                foreach (char character in characters)
                {

                    if (character == '\r' || character == '\n')
                    {
                        for (int i = 0; i < ColourTextBlockViewModels.Count; i++)
                        {
                            ColourTextBlockViewModels[i].AddNewLineChar(character);
                        }



                        ScrollToEnd = true;
                    }
                    else
                    {
                        for (int i = 0; i < ColourTextBlockViewModels.Count; i++)
                        {
                            if (i == color)
                            {
                                ColourTextBlockViewModels[i].AddChar(character);
                            }
                            else
                            {
                                ColourTextBlockViewModels[i].AddChar(' ');
                            }
                        }
                    }



                    await Task.Delay(TimeSpan.FromMilliseconds(5));
                }
            }
            else
            {

                if (!cursorOn)
                {
                    cursorTextBlock.FlipCursor(CurrenTheme.CursorOn, CurrenTheme.CursorOff);

                    cursorOn = true;
                }
                else
                {

                    cursorTextBlock.FlipCursor(CurrenTheme.CursorOff, CurrenTheme.CursorOn);
                    cursorOn = false;
                }

                await Task.Delay(TimeSpan.FromMilliseconds(100)); // Opóźnienie dla migania kursora
            }

        }
    }






    public void AddString(string text, int color)
    {
        _textQueue.Enqueue(Tuple.Create(text, color));
    }



    private ColourTextBlockViewModel cursorTextBlock;
    private void InitializeTextBlocks(Theme theme)
    {


        ColourTextBlockViewModels = new();

        for (int i = 0; i < theme.ForegroundColours.Colors.Count; i++)
        {
            var tb = new ColourTextBlockViewModel
            {
                BackgroundColour = i == 0 ? theme.Background : new denSharedLibrary.Color { R = "00", G = "00", B = "00", A = "00" },
                ForegroundColour = theme.ForegroundColours.Colors[i],
                FontFamily = theme.FontFile,
                FontSize = theme.FontSize
            };
            if (i == 0)
            {
                cursorTextBlock = tb;
            }
            ColourTextBlockViewModels.Add(tb);
        }

    }


}