using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SettingsKeptInFile;

namespace denMVVM;

public partial class MainWindow : Window
{
    public static string tempDir = "";
    public static string version;
    private IServiceProvider _serviceProvider;

    public MainWindow(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        InitializeComponent();
        if (!ifNoConnectionThenExit())
        {
            return;
        }

         

        this.Loaded += MainWindow_OnLoaded;
        this.Closing += Window_Closing;
        version = File.ReadAllText(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"version.txt"));
        this.Title += " " + version;
        this.DataContext = _serviceProvider.GetRequiredService<MainWindowViewModel>();
    }
     

    private bool ifNoConnectionThenExit()
    {
        return true;
    }
    private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        var mainset = _serviceProvider.GetRequiredService<ISettingsService>() .GetAllSettings();
        if (mainset != null && mainset.ContainsKey("WindowLeft") && mainset.ContainsKey("WindowTop") && mainset.ContainsKey("WindowWidth") && mainset.ContainsKey("WindowHeight"))
        {
            this.Left = Convert.ToDouble(mainset["WindowLeft"]);
            this.Top = Convert.ToDouble(mainset["WindowTop"]);
            this.Width = Convert.ToDouble(mainset["WindowWidth"]);
            this.Height = Convert.ToDouble(mainset["WindowHeight"]);
        }
        await Task.Run(async () =>
        {
            //try
            //{
            //    TcpClient client = new TcpClient();
            //    await client.ConnectAsync("127.0.0.1", 5000);
            //    NetworkStream nwStream = client.GetStream();
            //    byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes("I am in");
            //    await nwStream.WriteAsync(bytesToSend, 0, bytesToSend.Length);
            //}
            //catch (Exception ex)
            //{
            //}

            using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "denPipe", PipeDirection.Out))
            {
                pipeClient.Connect();
                SendMessage(pipeClient, "ReadyToRun");
            }

        });

        static void SendMessage(NamedPipeClientStream pipeClient, string message)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            pipeClient.Write(buffer, 0, buffer.Length);
        }

    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        if (!string.IsNullOrEmpty(tempDir))
        {
            string[] filePaths = Directory.GetFiles(tempDir);
            foreach (string filePath in filePaths)
                File.Delete(filePath);
            Directory.Delete(tempDir);
        }
        var mainset = new Dictionary<string, string>();
        mainset["WindowLeft"] = this.Left.ToString();
        mainset["WindowTop"] = this.Top.ToString();
        mainset["WindowWidth"] = this.Width.ToString();
        mainset["WindowHeight"] = this.Height.ToString();

        //if (((MainWindowViewModel)DataContext)._userControls.ContainsKey("ProBro"))
        //{
        //    var dict = (((MainWindowViewModel)DataContext)._userControls["ProBro"]).GetColumnsWidths();
        //    foreach (var column in dict)
        //    {
        //        mainset.Add(column.Key, column.Value.ToString());
        //    }
        //}
        _serviceProvider.GetRequiredService<ISettingsService>().UpdateSettings(mainset);
    }
}