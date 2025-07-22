using DataServicesNET80;
using denSharedLibrary;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SettingsKeptInFile;
using SkiaSharp;
using static denSharedLibrary.Colours;

namespace denViewModels.SalesSummary;

public class StatsViewModel : ObservableObject, IAsyncDialogViewModel
{
    private bool _isShowNettoChecked = false;
    private bool _isShowRunningAverageChecked = false;
    private bool _isPieChartSelected = false;
    private bool _isLinesChartSelected = true;
    private byte[] _image;

    public bool IsShowNettoChecked
    {
        get => _isShowNettoChecked;
        set
        {
            SetProperty(ref _isShowNettoChecked, value);
            ShowImage();
            if (IsShowNettoChecked)
            {
                SettingsService.UpdateSetting("chartsNettoPrices", "yes");
            }
            else
            {
                SettingsService.UpdateSetting("chartsNettoPrices", "no");
            }
        }
    }

    public bool IsShowRunningAverageChecked
    {
        get => _isShowRunningAverageChecked;
        set
        {
            SetProperty(ref _isShowRunningAverageChecked, value);
            ShowImage();
            if (IsShowRunningAverageChecked)
            {
                SettingsService.UpdateSetting("chartsRunningAverage", "yes");
            }
            else
            {
                SettingsService.UpdateSetting("chartsRunningAverage", "no");
            }
        }
    }

    public bool IsPieChartSelected
    {
        get => _isPieChartSelected;
        set
        {
            SetProperty(ref _isPieChartSelected, value);
            ShowImage();
        }
    }

    public bool IsLinesChartSelected
    {
        get => _isLinesChartSelected;
        set
        {
            SetProperty(ref _isLinesChartSelected, value);
            ShowImage();
            if (IsLinesChartSelected)
            {
                SettingsService.UpdateSetting("chartsChartType", "lines");
            }
            else
            {
                SettingsService.UpdateSetting("chartsChartType", "pie");
            }
        }
    }

    public byte[] Image
    {
        get => _image;
        set => SetProperty(ref _image, value);
    }

    private int _totalColumnsWidth = 6;

    public int TotalColumnsInGrid
    {
        get => _totalColumnsWidth;
        set => SetProperty(ref _totalColumnsWidth, value);
    }

    private int _imageWitdh = 5;

    public int ImageWitdh
    {
        get => _imageWitdh;
        set => SetProperty(ref _imageWitdh, value);
    }

    private int _descriptionWidth = 1;

    public int DescriptionWidth
    {
        get => TotalColumnsInGrid - ImageWitdh;
    }

    public AsyncRelayCommand ShowWindowCommand { get; }
    public AsyncRelayCommand CloseWindowCommand { get; }

    public enum ComparedData
    {
        Countries,
        MarketPlaces
    }

    public class ChartsPack
    {
        public ComparedData WhatAreWeComparing;
        public Dictionary<List<string>, string> countriesGroups;
        public Dictionary<List<int>, string> marketGroups;
        public Dictionary<DateTime, List<orderData>> GroupedOrders;
        public char YSymbol;
        public Dictionary<List<int>, string> GroupNames;
    }

    private ChartsPack MyChartsPack;
    private MySizeInfo MySizeInfo;

    public struct SingleChartValue
    {
        public double YValue;
        public int XValue;
        public int Identifier;
    }

    public interface IChartsValues
    {
        List<SingleChartValue>[] ChartValues { get; set; }
        List<SingleChartValue>[] TransformedChartValues { get; set; }
        List<List<int>> Groups { get; set; }
        Dictionary<int, string> XValuesLabels { get; set; }
    }

    public class ChartsValues<T> : IChartsValues
    {
        public List<SingleChartValue>[] ChartValues { get; set; }
        public List<SingleChartValue>[] TransformedChartValues { get; set; }
        public Dictionary<T, int> Translation { get; set; }
        public List<List<int>> Groups { get; set; }
        public Dictionary<int, string> XValuesLabels { get; set; }
    }

    private IChartsValues MyChartValues;
    private RGB[] kolory;

    public Dictionary<int, string> GetXValuesLabelsForOrdersPack(ChartsPack chartsPack)
    {
        var zwrotka = new Dictionary<int, string>();
        int i = 0;
        foreach (var dzien in chartsPack.GroupedOrders.Keys)
        {
            zwrotka.Add(i, dzien.ToShortDateString());
            i++;
        }
        return zwrotka;
    }

    public async Task DoInitialMath()
    {
        if (MyChartsPack.WhatAreWeComparing == ComparedData.MarketPlaces)
        {
            MyChartValues = new ChartsValues<int>();
            MyChartValues.XValuesLabels = GetXValuesLabelsForOrdersPack(MyChartsPack);
            HashSet<int> markety = new();
            foreach (var mark in MyChartsPack.marketGroups)
            {
                foreach (var mrk in mark.Key)
                {
                    markety.Add(mrk);
                }
            }
            Dictionary<DateTime, List<orderData>> GroupedOrders = new Dictionary<DateTime, List<orderData>>();
            foreach (var orders in MyChartsPack.GroupedOrders)
            {
                var ValidOrders = orders.Value.Where(p => markety.Contains(p.MarketId)).ToList();
                GroupedOrders.Add(orders.Key, ValidOrders);
            }
            MyChartsPack.GroupedOrders = GroupedOrders.OrderBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            DateTime minDate = MyChartsPack.GroupedOrders.Keys.First();
            DateTime maxDate = MyChartsPack.GroupedOrders.Keys.Last();
            int daysDifference = (maxDate - minDate).Days + 1; // +1, aby uwzględnić obie daty krańcowe
            MyChartValues.ChartValues = new List<SingleChartValue>[daysDifference];
            MyChartValues.TransformedChartValues = new List<SingleChartValue>[daysDifference];

            int[] AllMarkets = markety.OrderBy(p => p).ToArray();
            if (MyChartValues is ChartsValues<int> IntChartsValues)
            {
                IntChartsValues.Translation = new Dictionary<int, int>();
                for (int i = 0; i < AllMarkets.Length; i++)
                {
                    IntChartsValues.Translation.Add(AllMarkets[i], i);
                }
                IntChartsValues.Groups = new List<List<int>>();
                foreach (var lm in MyChartsPack.marketGroups)
                {
                    var listaT = new List<int>();
                    foreach (var mrk in lm.Key)
                    {
                        listaT.Add(IntChartsValues.Translation[mrk]);
                    }
                    IntChartsValues.Groups.Add(listaT);
                }

                var currdate = minDate;
                for (int i = 0; i < daysDifference; i++)
                {
                    MyChartValues.ChartValues[i] = new List<SingleChartValue>();
                    MyChartValues.TransformedChartValues[i] = new List<SingleChartValue>();
                    if (MyChartsPack.GroupedOrders.TryGetValue(currdate, out List<orderData> orders))
                    {
                        foreach (var ord in orders)
                        {
                            MyChartValues.ChartValues[i].Add(new SingleChartValue
                            {
                                Identifier = IntChartsValues.Translation[ord.MarketId],
                                XValue = i,
                                YValue = Convert.ToDouble(ord.Total)
                            });
                            MyChartValues.TransformedChartValues[i].Add(new SingleChartValue
                            {
                                Identifier = IntChartsValues.Translation[ord.MarketId],
                                XValue = i,
                                YValue = Convert.ToDouble(ord.NetTotal)
                            });
                        }
                    }

                    //filling gaps
                    foreach (var group in MyChartValues.Groups)
                    {
                        //jezeli w danym dniu nie ma zamowienia dla danej grupy
                        if (!MyChartValues.ChartValues[i].Any(p => group.Any(q => q == p.Identifier)))
                        {
                            //to dodaje jedno zamowienie dla danej grupy z wartoscia zero
                            var spraw = new SingleChartValue
                            {
                                Identifier = group.First(),
                                XValue = i,
                                YValue = 0
                            };
                            MyChartValues.ChartValues[i].Add(spraw);
                            MyChartValues.TransformedChartValues[i].Add(new SingleChartValue
                            {
                                Identifier = group.First(),
                                XValue = i,
                                YValue = 0
                            });
                        }
                    }
                    currdate = currdate.AddDays(1);
                }

                kolory = Colours.GetBaseColors(MyChartsPack.marketGroups.Count).ToArray();
                int j = 0;
                foreach (var element in IntChartsValues.Groups)
                {
                    Legenda.Add(new TextRGB
                    {
                        Text = MyChartsPack.marketGroups.ElementAt(j).Value,
                        Colour = kolory[j],
                        Group = element
                    });
                    j++;
                }
            }
        }

        if (MyChartsPack.WhatAreWeComparing == ComparedData.Countries)
        {
            MyChartValues = new ChartsValues<string>
            {
                XValuesLabels = GetXValuesLabelsForOrdersPack(MyChartsPack)
            };
            HashSet<string> countries = new();
            foreach (var mark in MyChartsPack.countriesGroups)
            {
                foreach (var mrk in mark.Key)
                {
                    countries.Add(mrk);
                }
            }
            Dictionary<DateTime, List<orderData>> GroupedOrders = new Dictionary<DateTime, List<orderData>>();
            foreach (var orders in MyChartsPack.GroupedOrders)
            {
                var ValidOrders = orders.Value.Where(p => countries.Contains(p.CountryCode)).ToList();
                GroupedOrders.Add(orders.Key, ValidOrders);
            }
            MyChartsPack.GroupedOrders = GroupedOrders.OrderBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            DateTime minDate = MyChartsPack.GroupedOrders.Keys.First();
            DateTime maxDate = MyChartsPack.GroupedOrders.Keys.Last();
            int daysDifference = (maxDate - minDate).Days + 1; // +1, aby uwzględnić obie daty krańcowe
            MyChartValues.ChartValues = new List<SingleChartValue>[daysDifference];
            MyChartValues.TransformedChartValues = new List<SingleChartValue>[daysDifference];

            string[] AllCountries = countries.OrderBy(p => p).ToArray();
            if (MyChartValues is ChartsValues<string> StringChartsValues)
            {
                StringChartsValues.Translation = new Dictionary<string, int>();
                for (int i = 0; i < AllCountries.Length; i++)
                {
                    StringChartsValues.Translation.Add(AllCountries[i], i);
                }
                StringChartsValues.Groups = new List<List<int>>();
                foreach (var lm in MyChartsPack.countriesGroups)
                {
                    var listaT = new List<int>();
                    foreach (var mrk in lm.Key)
                    {
                        listaT.Add(StringChartsValues.Translation[mrk]);
                    }
                    StringChartsValues.Groups.Add(listaT);
                }

                var currdate = minDate;
                for (int i = 0; i < daysDifference; i++)
                {
                    MyChartValues.ChartValues[i] = new List<SingleChartValue>();
                    MyChartValues.TransformedChartValues[i] = new List<SingleChartValue>();
                    if (MyChartsPack.GroupedOrders.TryGetValue(currdate, out List<orderData> orders))
                    {
                        foreach (var ord in orders)
                        {
                            MyChartValues.ChartValues[i].Add(new SingleChartValue
                            {
                                Identifier = StringChartsValues.Translation[ord.CountryCode],
                                XValue = i,
                                YValue = Convert.ToDouble(ord.Total)
                            });
                            MyChartValues.TransformedChartValues[i].Add(new SingleChartValue
                            {
                                Identifier = StringChartsValues.Translation[ord.CountryCode],
                                XValue = i,
                                YValue = Convert.ToDouble(ord.NetTotal)
                            });
                        }
                    }

                    //filling gaps
                    foreach (var group in MyChartValues.Groups)
                    {
                        if (!MyChartValues.ChartValues[i].Any(p => group.Any(q => q == p.Identifier)))
                        {
                            //to dodaje jedno zamowienie dla danej grupy z wartoscia zero
                            MyChartValues.ChartValues[i].Add(new SingleChartValue
                            {
                                Identifier = group.First(),
                                XValue = i,
                                YValue = 0
                            });
                            MyChartValues.TransformedChartValues[i].Add(new SingleChartValue
                            {
                                Identifier = group.First(),
                                XValue = i,
                                YValue = 0
                            });
                        }
                    }
                    currdate = currdate.AddDays(1);
                }
                kolory = Colours.GetBaseColors(MyChartsPack.countriesGroups.Count).ToArray();
                int j = 0;
                foreach (var element in StringChartsValues.Groups)
                {
                    Legenda.Add(new TextRGB
                    {
                        Text = MyChartsPack.countriesGroups.ElementAt(j).Value,
                        Colour = kolory[j],
                        Group = element
                    });
                    j++;
                }
            }
        }
    }

    public void PrepareWindow()
    {
        var set = SettingsService.GetSetting("chartsChartType").GetValue<string>();
        if (set != null && (set.ToLower().Equals("lines") || set.ToLower().Equals("pie")))
        {
            if (set.ToLower().Equals("lines"))
            {
                IsLinesChartSelected = true;
            }
            else
            {
                IsPieChartSelected = true;
            }
        }
        set = SettingsService.GetSetting("chartsNettoPrices").GetValue<string>();
        if (set != null && (set.ToLower().Equals("yes") || set.ToLower().Equals("no")))
        {
            if (set.ToLower().Equals("yes"))
            {
                IsShowNettoChecked = true;
            }
            else
            {
                IsShowNettoChecked = false;
            }
        }
        set = SettingsService.GetSetting("chartsRunningAverage").GetValue<string>();
        if (set != null && (set.ToLower().Equals("yes") || set.ToLower().Equals("no")))
        {
            if (set.ToLower().Equals("yes"))
            {
                IsShowRunningAverageChecked = true;
            }
            else
            {
                IsShowRunningAverageChecked = false;
            }
        }
    }

    private IDispatcherTimer _dispatcherTimer;
    private ISettingsService SettingsService;

    public StatsViewModel(ChartsPack chartsPack, IDispatcherTimerFactory dispatcherTimerFactory, ISettingsService settingsService)
    {
        MyChartsPack = chartsPack;
        MySizeInfo = new MySizeInfo();

        _dispatcherTimer = dispatcherTimerFactory.Create();
        _dispatcherTimer.Interval = TimeSpan.FromMilliseconds(100);
        _dispatcherTimer.Tick += OnDebounceTimerTick;
        ShowWindowCommand = new AsyncRelayCommand(ShowWindowExecute);
        CloseWindowCommand = new AsyncRelayCommand(CloseWindow);
        PrepareWindow();
        SettingsService = settingsService;
    }

    public MySizeInfo Response;

    public async Task CloseWindow()
    {
        Response = MySizeInfo;
        RequestClose?.Invoke(this, EventArgs.Empty);
    }

    private double _windowWidth = 800;

    public double WindowWidth
    {
        get { return _windowWidth; }
        set
        {
            if (SetProperty(ref _windowWidth, value))
            {
                MySizeInfo.Width = value;
                _dispatcherTimer.Stop();
                _dispatcherTimer.Start();
            }
        }
    }

    private double _windowHeight = 600;

    public double WindowHeight
    {
        get { return _windowHeight; }
        set
        {
            if (SetProperty(ref _windowHeight, value))
            {
                MySizeInfo.Height = value;
                _dispatcherTimer.Stop();
                _dispatcherTimer.Start();
            }
        }
    }

    private double _windowTop = 100;

    public double WindowTop
    {
        get { return _windowTop; }
        set
        {
            if (SetProperty(ref _windowTop, value))
            {
                MySizeInfo.X = value;
            }
        }
    }

    private double _windowLeft = 100;

    public event AsyncEventHandler RequestClose;

    public double WindowLeft
    {
        get { return _windowLeft; }
        set
        {
            if (SetProperty(ref _windowLeft, value))
            {
                MySizeInfo.Y = value;
            }
        }
    }

    public async Task ShowWindowExecute()
    {
        var prex = SettingsService.GetSetting("chartsWindowTopLeftX").GetValue<string>();
        var prey = SettingsService.GetSetting("chartsWindowTopLeftY").GetValue<string>();
        if (double.TryParse(prex, out double prex2) && double.TryParse(prey, out double prey2))
        {
            WindowTop = prex2;
            WindowLeft = prey2;
        }
        prex = SettingsService.GetSetting("chartsWindowWidth").GetValue<string>();
        prey = SettingsService.GetSetting("chartsWindowHeight").GetValue<string>();
        if (double.TryParse(prex, out prex2) && double.TryParse(prey, out prey2))
        {
            WindowWidth = prex2;
            WindowHeight = prey2;
        }
        MySizeInfo.Width = WindowWidth; MySizeInfo.Height = WindowHeight;
        await DoInitialMath();
        ShowImage();
    }

    private void ShowImage()
    {
        if (MySizeInfo.Width == 0 || MySizeInfo.Height == 0) { return; }

        double width = (MySizeInfo.Width * ImageWitdh) / TotalColumnsInGrid;
        ChartsReturn ret;
        if (IsLinesChartSelected)
        {
            ret = DrawLinearChart(MyChartValues, IsShowRunningAverageChecked, (float)width, (float)MySizeInfo.Height, !IsShowNettoChecked, MyChartsPack.YSymbol);
        }
        else
        {
            ret = DrawPieChart(MyChartValues, !IsShowNettoChecked, (float)width, (float)MySizeInfo.Height);
        }
        foreach (var grupa in MyChartValues.Groups)
        {
            Legenda.First(p => p.Group.Equals(grupa)).TotalValue = MyChartsPack.YSymbol + Math.Round(ret.TotalValues[grupa], 2).ToString();
        }
        Image = ret.ImageArray;
    }

    private void OnDebounceTimerTick(object sender, EventArgs e)
    {
        _dispatcherTimer.Stop();
        ShowImage();
    }

    public class ChartsReturn
    {
        public Dictionary<List<int>, double> TotalValues;
        public RGB[] ColorsUsed;
        public byte[] ImageArray;
    }

    public class TextRGB : ObservableObject
    {
        private RGB _colour;
        private string _text;
        private string _totalValue;
        private List<int> _group;

        public RGB Colour
        {
            get => _colour;
            set => SetProperty(ref _colour, value);
        }

        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }

        public string TotalValue
        {
            get => _totalValue;
            set => SetProperty(ref _totalValue, value);
        }

        public List<int> Group
        {
            get => _group;
            set => SetProperty(ref _group, value);
        }
    }

    public ObservableCollection<TextRGB> Legenda { get; set; } = new ObservableCollection<TextRGB>();

    private ChartsReturn DrawLinearChart(IChartsValues chartsValues, bool ShowTrends, float width, float height, bool transformed, char YSymbol)
    {
        if (width <= 0 || height <= 0) return default;
        int XSpan = chartsValues.ChartValues.Length;

        float yOffset = 50;
        float margin = 50;
        float graphWidth = width - margin * 2;
        float graphHeight = height - margin - yOffset;

        // w dict mam slownik, taki, ze
        //dla kazdej grupy mam odpowiadajacy jej slownik (wartosc na osi X, lista zamowien)
        //czyli, dla pierwszej wartosci dla punktu na osi x
        //mam przypisane przypisany slownik, taki, ze
        //dla kazdej porownywanej grupy mam wszystkie wartosci w danym punkcie
        // w tym momencie, jezeli nie ma zadnych wartosci dla danej grupy dla danego punktu
        //to jest juz dodana jedna wartosc = 0

        Dictionary<List<int>, Dictionary<int, List<SingleChartValue>>> Groups2ValuesOverXAxis = new Dictionary<List<int>, Dictionary<int, List<SingleChartValue>>>();
        foreach (var group in chartsValues.Groups)
        {
            Groups2ValuesOverXAxis.Add(group, new Dictionary<int, List<SingleChartValue>>());
        }

        double maxTotal = 0;
        Dictionary<int, double> orderedTotals = new();
        var MyValues = transformed ? chartsValues.TransformedChartValues : chartsValues.ChartValues;

        Dictionary<List<int>, double> TotalValues = new();

        PreDrawingCalculations();

        //w movingAverages mam slownik taki, ze dla kazdej grupy mam
        // zagniezdzony slownik taki, ze dla kazdej wartosci ma osi X, mam wyliczonona usredniona wartosc na osi Y
        Dictionary<List<int>, Dictionary<int, double>> movingAverages = new();
        byte alpha = 255;
        if (ShowTrends)
        {
            CalculateAvergaes();
            alpha = 64;
        }

        double xScale = graphWidth / MyValues.Length;
        double yScale = graphHeight / maxTotal;

        var bitmap = new SKBitmap(Convert.ToInt32(width), Convert.ToInt32(height), SKColorType.Bgra8888, SKAlphaType.Premul);
        using (var surface = SKSurface.Create(bitmap.Info, bitmap.GetPixels(out _)))
        {
            SKCanvas canvas = surface.Canvas;
            DrawXYAxis(canvas);

            double x1 = 0;
            double y1 = 0;
            var step = graphWidth / orderedTotals.Count;

            Dictionary<int, double> x1s = new();
            Dictionary<int, double> y1s = new();
            Dictionary<int, double> previousx1s = new();
            Dictionary<int, double> previousy1s = new();
            int j = 0;
            ClearDicts(x1s, y1s, previousx1s, previousy1s);

            double suma = 0;
            SKPaint[] PaintLines = new SKPaint[chartsValues.Groups.Count];
            SKPaint[] PaintAverages = new SKPaint[chartsValues.Groups.Count];
            for (int l = 0; l < chartsValues.Groups.Count; l++)
            {
                PaintLines[l] = new SKPaint { Color = new SKColor(kolory[l].R, kolory[l].G, kolory[l].B, alpha), StrokeWidth = 2 };
                PaintAverages[l] = new SKPaint { Color = new SKColor(kolory[l].R, kolory[l].G, kolory[l].B, 255), StrokeWidth = 2 };
            }

            for (int l = 0; l < orderedTotals.Count; l++)
            {
                j = 0;
                foreach (var grupa in chartsValues.Groups)
                {
                    suma = Groups2ValuesOverXAxis[grupa][l].Select(p => p.YValue).Sum();
                    x1s[j] += step;
                    y1s[j] = suma * yScale;
                    x1 = x1s[j];
                    y1 = y1s[j];
                    if (l == 0)
                    {
                        previousy1s[j] = y1;
                    }
                    canvas.DrawLine((float)previousx1s[j] + margin, graphHeight - (float)previousy1s[j] + yOffset, (float)x1s[j] + margin, graphHeight - (float)y1s[j] + yOffset, PaintLines[j]);
                    previousx1s[j] = x1;
                    previousy1s[j] = y1;
                    j++;
                }
            }

            if (ShowTrends)
            {
                ClearDicts(x1s, y1s, previousx1s, previousy1s);
                for (int l = 0; l < orderedTotals.Count; l++)
                {
                    j = 0;
                    foreach (var grupa in chartsValues.Groups)
                    {
                        suma = movingAverages[grupa][l];
                        x1s[j] += step;
                        y1s[j] = suma * yScale;
                        x1 = x1s[j];
                        y1 = y1s[j];
                        if (l == 0)
                        {
                            previousy1s[j] = y1;
                        }
                        canvas.DrawLine((float)previousx1s[j] + margin, graphHeight - (float)previousy1s[j] + yOffset, (float)x1s[j] + margin, graphHeight - (float)y1s[j] + yOffset, PaintAverages[j]);
                        previousx1s[j] = x1;
                        previousy1s[j] = y1;
                        j++;
                    }
                }
            }
        }

        var zwrotka = new ChartsReturn
        {
            ColorsUsed = kolory,
            TotalValues = TotalValues
        };
        using (var image = SKImage.FromBitmap(bitmap))
        using (var encoded = image.Encode(SKEncodedImageFormat.Png, 100))
        {
            using (var stream = new MemoryStream())
            {
                encoded.SaveTo(stream);
                zwrotka.ImageArray = stream.ToArray();
            }
        }
        return zwrotka;

        void ClearDicts(Dictionary<int, double> x1s, Dictionary<int, double> y1s, Dictionary<int, double> previousx1s, Dictionary<int, double> previousy1s)
        {
            int j = 0;
            x1s.Clear();
            y1s.Clear();
            previousx1s.Clear();
            previousy1s.Clear();
            foreach (var mark in chartsValues.Groups)
            {
                x1s.Add(j, 0);
                y1s.Add(j, 0);
                previousx1s.Add(j, 0);
                previousy1s.Add(j, 0);
                j++;
            }
        }

        void DrawXYAxis(SKCanvas canvas)
        {
            int indicatorsonYaxis = 10;
            double intervalY = maxTotal / indicatorsonYaxis;

            int intervalX;
            float pixelSpacing = 80; // Przewidywana odległość między indykatorami w pikselach
            int maxIndicators = Convert.ToInt32(graphWidth / pixelSpacing); // Maksymalna liczba indykatorów na osi X

            int expectedInterval = XSpan / maxIndicators;

            if (expectedInterval <= 1)
            {
                intervalX = 1; // Minimum to 1 dzień
            }
            else if (expectedInterval <= 7)
            {
                intervalX = 7; // Tygodniowy interwał
            }
            else if (expectedInterval <= 30)
            {
                intervalX = 30; // Miesięczny interwał
            }
            else
            {
                // Maksymalny interwał w zależności od szerokości grafu
                intervalX = Math.Min(365, expectedInterval);
            }

            SKPaint dashedLinePaint = new SKPaint
            {
                Color = SKColors.Black,
                StrokeWidth = 1f,
                PathEffect = SKPathEffect.CreateDash(new float[] { 10, 10 }, 0)
            };

            canvas.Clear(SKColors.White);
            float axisStrokeWidth = 3;
            SKPaint axisPaint = new() { Color = SKColors.Black, StrokeWidth = axisStrokeWidth };
            canvas.DrawLine(margin, graphHeight + yOffset, margin, yOffset, axisPaint);
            canvas.DrawLine(margin + graphWidth, graphHeight + yOffset, margin + graphWidth, yOffset, axisPaint);
            var labelPaint = new SKPaint { Color = SKColors.Black, TextSize = 12 };

            double indicatorY = 0;
            for (int k = 0; k <= indicatorsonYaxis; k++)
            {
                float y = graphHeight - (float)(indicatorY / maxTotal * (graphHeight));
                canvas.DrawLine(margin - 10, y + yOffset, margin, y + yOffset, axisPaint);
                canvas.DrawLine(margin + graphWidth, y + yOffset, margin + graphWidth + 10, y + yOffset, axisPaint);
                canvas.DrawText(YSymbol.ToString() + Math.Round(indicatorY, 2).ToString(), margin - 40, y + yOffset, labelPaint);
                canvas.DrawText(YSymbol.ToString() + Math.Round(indicatorY, 2).ToString(), graphWidth + margin + 5, y + yOffset, labelPaint);
                canvas.DrawLine(margin, y + yOffset, margin + graphWidth, y + yOffset, dashedLinePaint);
                indicatorY += intervalY;
            }

            canvas.DrawLine(margin, graphHeight + yOffset, graphWidth + 50, graphHeight + yOffset, axisPaint);
            int integerIndicator = 0;

            foreach (var localTotal in orderedTotals)
            {
                if (integerIndicator % intervalX == 0) // Rysuj indykatory tylko dla odpowiednich indeksów
                {
                    canvas.DrawLine(margin + (float)(integerIndicator * xScale), graphHeight + yOffset, margin + (float)(integerIndicator * xScale), graphHeight + 10 + yOffset, axisPaint);
                    canvas.DrawText(chartsValues.XValuesLabels[localTotal.Key], margin + (float)(integerIndicator * xScale) - 30, graphHeight + 20 + yOffset, labelPaint);
                    canvas.DrawLine(margin + (float)(integerIndicator * xScale), graphHeight + yOffset, margin + (float)(integerIndicator * xScale), +yOffset, dashedLinePaint);
                }
                integerIndicator++;
            }
        }

        void CalculateAvergaes()
        {
            double XValuesPerPixel = (double)MyValues.Length / graphWidth;
            int movingAveragePeriod = Math.Max(1, (int)Math.Round(30 * (double)XValuesPerPixel));

            foreach (var mrk in chartsValues.Groups)
            {
                movingAverages.Add(mrk, new Dictionary<int, double>());
                foreach (var group in Groups2ValuesOverXAxis[mrk].Keys)
                {
                    // Wyszukaj zakres wartisci X dla obliczenia średniej
                    int startingPoint = Convert.ToInt32(group - Math.Ceiling(movingAveragePeriod / 2.0));
                    if (startingPoint < 0) { startingPoint = 0; }
                    int endingPoint = group + Convert.ToInt32(Math.Floor(movingAveragePeriod / 2.0));
                    if (endingPoint > MyValues.Length - 1) { startingPoint = MyValues.Length - 1; }
                    // Filtruj dane dla wybranego zakresu
                    var filteredOrders = Groups2ValuesOverXAxis[mrk].Where(g => g.Key >= startingPoint && g.Key <= endingPoint);

                    // Oblicz sumę wartości Total dla wszystkich zamówień w wybranym zakresie
                    double totalValueForAllOrders = 0;
                    int totalDays = 0;
                    foreach (var orderGroup in filteredOrders)
                    {
                        totalValueForAllOrders += orderGroup.Value.Sum(o => o.YValue);
                        totalDays++;
                    }

                    // Oblicz średnią wartość wszystkich zamówień w ciągu jednego dnia
                    double average = totalValueForAllOrders / totalDays;
                    movingAverages[mrk][group] = average;
                }
            }
        }

        void PreDrawingCalculations()
        {
            foreach (var group in chartsValues.Groups)
            {
                TotalValues.Add(group, 0);
            }

            for (int i = 0; i < MyValues.Length; i++)
            {
                List<SingleChartValue> OrdersThisDay;

                OrdersThisDay = MyValues[i];
                double totalek = 0;
                double localtotal = 0;

                foreach (var group in chartsValues.Groups)
                {
                    var lista = new List<SingleChartValue>();
                    foreach (var order in OrdersThisDay)
                    {
                        if (group.Any(p => order.Identifier == p))
                        {
                            lista.Add(order);
                        }
                    }
                    Groups2ValuesOverXAxis[group].Add(i, lista);

                    var orders = MyValues[i].Where(m => group.Contains(m.Identifier)).ToList();
                    totalek = orders.Select(m => m.YValue).Sum();
                    localtotal += totalek;
                    if (totalek > maxTotal)
                    {
                        maxTotal = totalek;
                    }
                    TotalValues[group] += totalek;
                }
                orderedTotals.Add(i, localtotal);
            }
        }
    }

    private ChartsReturn DrawPieChart(IChartsValues chartsValues, bool transformed, float width, float height)
    {
        if (width <= 0 || height <= 0) return default;
        var MyValues = transformed ? chartsValues.TransformedChartValues : chartsValues.ChartValues;
        Dictionary<List<int>, double> TotalValues = new();
        foreach (var group in chartsValues.Groups)
        {
            TotalValues.Add(group, 0);
        }
        double total = 0;
        for (int i = 0; i < MyValues.Length; i++)
        {
            double totalek = 0;
            foreach (var group in chartsValues.Groups)
            {
                var orders = MyValues[i].Where(m => group.Contains(m.Identifier)).ToList();
                totalek = orders.Select(m => m.YValue).Sum();
                total += totalek;
                TotalValues[group] += totalek;
            }
        }
        var bitmap = new SKBitmap(Convert.ToInt32(width), Convert.ToInt32(height), SKColorType.Bgra8888, SKAlphaType.Premul);
        using (var surface = SKSurface.Create(bitmap.Info, bitmap.GetPixels(out _)))
        {
            SKCanvas canvas = surface.Canvas;
            canvas.Clear(SKColors.White);

            var startAngle = 0f;

            var margin = 50;
            var centerX = width / 2;
            var centerY = height / 2;
            var squareSide = Math.Min(width, height) - 2 * margin;
            var left = centerX - squareSide / 2;
            var top = centerY - squareSide / 2;

            var rect = new SKRect(left, top, left + squareSide, top + squareSide);

            int i = 0;
            foreach (var kvp in TotalValues)
            {
                var sweepAngle = (float)(360 * kvp.Value / total);
                var paint = new SKPaint
                {
                    Color = new SKColor(kolory[i].R, kolory[i].G, kolory[i].B, 255),
                    IsAntialias = true,
                    Style = SKPaintStyle.Fill
                };

                canvas.DrawArc(rect, startAngle, sweepAngle, true, paint);

                startAngle += sweepAngle;
                i++;
            }
        }

        var zwrotka = new ChartsReturn
        {
            ColorsUsed = kolory,
            TotalValues = TotalValues
        };
        using (var image = SKImage.FromBitmap(bitmap))
        using (var encoded = image.Encode(SKEncodedImageFormat.Png, 100)) // 100 to jakość obrazu, dla PNG to jest ignorowane
        {
            using (var stream = new MemoryStream())
            {
                encoded.SaveTo(stream);
                zwrotka.ImageArray = stream.ToArray();
            }
        }
        return zwrotka;
    }
}