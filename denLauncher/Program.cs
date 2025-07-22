using IWshRuntimeLibrary;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using File = System.IO.File;

namespace denLauncher
{
    internal class Program
    {
        public static string AppName = "denMVVM.exe";
        public static string SoftwareDeveloperName = "Buchmiet Ltd";
        public static string ApplicationFullName = "Den";

        //public static void listenToClient()
        //{
        //    IPAddress vmAdd = IPAddress.Parse("127.0.0.1");
        //    TcpListener listener = new TcpListener(vmAdd, 5000);
        //    // Console.WriteLine("Listening...");
        //    listener.Start();

        //    //---incoming client connected---
        //    TcpClient client = listener.AcceptTcpClient();

        //    //---get the incoming data through a network stream---
        //    NetworkStream nwStream = client.GetStream();
        //    byte[] buffer = new byte[client.ReceiveBufferSize];

        //    //---read incoming stream---
        //    int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);
        //}

        //   private static string installDir = @"c:\den";

        private static void WaitForASingnalFromClientAndExit()
        {
            using (NamedPipeServerStream pipeServer = new NamedPipeServerStream("denPipe", PipeDirection.In))
            {
                pipeServer.WaitForConnection();
                ListenToClient(pipeServer);
            }
        }

        private static void ListenToClient(NamedPipeServerStream pipeStream)
        {
            byte[] buffer = new byte[256];
            try
            {
                while (true)
                {
                    int bytesRead = pipeStream.Read(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        if (message == "ReadyToRun")
                        {
                            Console.WriteLine("Starting den instance...");
                            break;
                        }
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("ERROR: {0}", e.Message);
            }
        }

        private static string FormatBytes(long bytes)
        {
            const long scale = 1024;
            string[] orders = new string[] { "GB", "MB", "KB", "B" };
            long max = (long)Math.Pow(scale, orders.Length - 1);

            var formattedBytes = new List<string>();
            foreach (string order in orders)
            {
                if (bytes >= max)
                {
                    long value = bytes / max;
                    if (value > 0) // Dodaj tylko, gdy wartość jest większa niż 0.
                    {
                        formattedBytes.Add(string.Format("{0} {1}", value, order));
                    }

                    bytes %= max; // Przygotuj 'bytes' do następnej iteracji
                }

                max /= scale;
            }

            return string.Join(" ", formattedBytes); // Łączy elementy listy z przecinkami
        }

        private static async Task<bool> DownloadFileAsync(string installDir)
        {
            const int totalBlocks = 20;
            var stopwatch = new System.Diagnostics.Stopwatch();

            try
            {
                using (var client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(@"http://buchmiet.com/api/den/DownloadNewest",
                        HttpCompletionOption.ResponseHeadersRead);

                    if (response.IsSuccessStatusCode)
                    {
                        using (var contentStream = await response.Content.ReadAsStreamAsync())
                        using (var fileStream = File.Create(Path.Combine(installDir, "lo.7z")))
                        {
                            var buffer = new byte[8192]; // bufor o rozmiarze 8KB
                            var totalBytesToRead =
                                response.Content.Headers.ContentLength; // Rozmiar zawartości może być null
                            var totalBytesRead = 0L;

                            stopwatch.Start();
                            int originalCursorTop = Console.CursorTop;
                            int bytesRead;

                            Console.WriteLine("Download size: {0}", FormatBytes(totalBytesToRead.Value));

                            while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                            {
                                await fileStream.WriteAsync(buffer, 0, bytesRead);
                                totalBytesRead += bytesRead;

                                // Oblicz prędkość pobierania (w megabitach na sekundę)
                                double speed = (totalBytesRead * 8) /
                                               (stopwatch.Elapsed.TotalSeconds * 1024 * 1024); // Megabity na sekundę

                                if (totalBytesToRead.HasValue)
                                {
                                    var progress = (double)totalBytesRead / totalBytesToRead.Value;
                                    int blocksFilled = (int)(progress * totalBlocks);

                                    // Wyczyść linie z informacją o postępie i prędkości
                                    Console.SetCursorPosition(0, originalCursorTop);
                                    Console.Write(new string(' ', Console.WindowWidth));
                                    Console.SetCursorPosition(0, originalCursorTop + 1);
                                    Console.Write(new string(' ', Console.WindowWidth));

                                    // Aktualizacja postępu
                                    Console.SetCursorPosition(0, originalCursorTop);
                                    Console.Write("Downloading [");
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.Write(new string('■', blocksFilled));
                                    Console.ResetColor();
                                    Console.Write(new string(' ', totalBlocks - blocksFilled) +
                                                  $"] {progress * 100:0.00}%");

                                    // Aktualizacja ilości pobranych danych i prędkości
                                    Console.SetCursorPosition(0, originalCursorTop + 1);
                                    Console.Write($"{FormatBytes(totalBytesRead)} at {speed:0.00} Mbps");
                                }
                            }

                            // Zatrzymaj zegar
                            stopwatch.Stop();

                            Console.WriteLine("\nDownloading successful");
                            //     loglauncher("Downloading successful"); // Zakładam, że ta metoda istnieje gdzieś w kodzie.
                            return true;
                        }
                    }
                    else
                    {
                        var contentStream = await response.Content.ReadAsStringAsync();
                        Console.WriteLine("Error: Unable to download file.");
                        //  loglauncher("Downloading failed"); // Zakładam, że ta metoda istnieje gdzieś w kodzie.
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An exception occurred while downloading the file: " + ex.Message);
                //loglauncher("An exception occurred: " + ex.Message); // Zakładam, że ta metoda istnieje gdzieś w kodzie.
                return false;
            }
        }

        public enum ProcessRunType
        {
            WaitForExit,
            FireAndForget
        }

        public static async Task StartProcess(string installDir, string executableFileName, string arguments,
            ProcessRunType runType)
        {
            try
            {
                Process pProcess = new Process();
                pProcess.StartInfo.FileName = Path.Combine(installDir, executableFileName);
                pProcess.StartInfo.Arguments = arguments;
                pProcess.StartInfo.UseShellExecute = false;
                pProcess.StartInfo.WorkingDirectory = installDir;
                pProcess.StartInfo.RedirectStandardOutput = true;
                if (runType == ProcessRunType.WaitForExit)
                {
                    pProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                }
                else
                {
                    pProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
                }

                pProcess.StartInfo.CreateNoWindow = true;
                pProcess.Start();
                if (runType == ProcessRunType.WaitForExit)
                {
                    string _ = await pProcess.StandardOutput.ReadToEndAsync();
                    pProcess.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                // Handle the exception
                // For example, log the exception, or rethrow it
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private static string GetInstallationPath(string companyName, string appName)
        {
            string registryPath = $@"SOFTWARE\{companyName}\{appName}";
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registryPath))
            {
                if (key != null)
                {
                    object installPathValue = key.GetValue("InstallPath");
                    if (installPathValue != null)
                    {
                        return installPathValue.ToString();
                    }
                }
            }

            return null;
        }

        private static void SetInstallationPath(string companyName, string appName, string installPath)
        {
            string registryPath = $@"SOFTWARE\{companyName}\{appName}";
            using (RegistryKey key = Registry.LocalMachine.CreateSubKey(registryPath))
            {
                if (key != null)
                {
                    key.SetValue("InstallPath", installPath, RegistryValueKind.String);
                    Console.WriteLine("Ścieżka instalacyjna została zapisana w rejestrze.");
                }
                else
                {
                    Console.WriteLine("Nie udało się utworzyć klucza w rejestrze.");
                }
            }
        }

        public static async Task<string> GetInstallationFolder()
        {
            return @"C:\Program Files\den";
        }

        public static async Task DisplayPreInstallationInformation(string installDir)
        {
            Console.WriteLine("No previous version detected, running in the installer mode - installing in " +
                              installDir);
            Console.WriteLine("Downloading current version of the app");
        }

        public static async Task DisplayPostInstallationInformation(string installDir)
        {
            Console.WriteLine($"The newest version has been downloaded successfully. Attempting to install now.");
        }

        public static void CreateInstallFolder(string installDir)
        {
            if (!Directory.Exists(installDir))
            {
                Directory.CreateDirectory(installDir);
            }
        }

        public static void DownloadErrorAndExitInfo()
        {
            Console.WriteLine("Program failed to download. Please try again");
        }

        public static async Task Run7Zip(string installationFolder)
        {
            File.Copy(Path.Combine(Environment.CurrentDirectory, "7zdec.exe"), Path.Combine(installationFolder, "7zdec.exe"));
            await StartProcess(installationFolder, "7zdec.exe", "x lo.7z",
                ProcessRunType.WaitForExit);
        }

        public static async Task DisplayPostUnpackingInformation()
        {
            Console.WriteLine("Unpacking complete, creating link on desktop");
        }

        public static async Task<bool> ConfirmWhetherUserWantsIconAdded2Desktop()
        {
            return true;
        }

        public static async Task AddIconToDesktop(string installationFolder)
        {
            object shDesktop = (object)"Desktop";
            WshShell shell = new WshShell();
            string shortcutAddress = (string)shell.SpecialFolders.Item(ref shDesktop) + @"\Den.lnk";
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutAddress);
            shortcut.Description = "den";
            shortcut.IconLocation = installationFolder + @"\Data\denDig.ico";
            shortcut.TargetPath = installationFolder + @"\denlauncher.exe";
            shortcut.Save();
        }

        public static async Task RemoveInstallationFile(string installationFolder)
        {
            if (File.Exists(Path.Combine(installationFolder, "lo.7z")))
            {
                File.Delete(Path.Combine(installationFolder, "lo.7z"));
            }
        }

        public static async Task StartTheAppFirstTime(string installationFolder, string parameters)
        {
            await StartProcess(installationFolder, "dlauncher.exe", parameters, ProcessRunType.FireAndForget);
        }

        public static async Task StartFreshInstallation()
        {
            var installationFolder = await GetInstallationFolder();
            await DisplayPreInstallationInformation(installationFolder);
            CreateInstallFolder(installationFolder);
            if (!await DownloadFileAsync(installationFolder))
            {
                Directory.Delete(installationFolder);
                DownloadErrorAndExitInfo();
                return;
            }

            await DisplayPostInstallationInformation(installationFolder);
            await Run7Zip(installationFolder);
            await DisplayPostUnpackingInformation();
            if (await ConfirmWhetherUserWantsIconAdded2Desktop())
            {
                await AddIconToDesktop(installationFolder);
            }

            await RemoveInstallationFile(installationFolder);
            SetInstallationPath(SoftwareDeveloperName, ApplicationFullName, installationFolder);
            await StartTheAppFirstTime(installationFolder, "refresh");
        }

        public static bool CheckIfArgumentsSaysItsTheFirstStart(string[] args)
        {
            if (args.Length == 1 && args[0].Equals("refresh"))
            {
                return true;
            }

            return false;
        }

        public static async Task IfOldLauncherExistsRemoveIt(string installationFolder)
        {
            if (File.Exists(Path.Combine(installationFolder, "denlauncher.exe")))
            {
                try
                {
                    File.Delete(Path.Combine(installationFolder, "denlauncher.exe"));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.InnerException.Message);
                }
            }
        }

        public static async Task PrepareNewLauncher(string installationFolder)
        {
            File.Copy(Path.Combine(installationFolder, "dlauncher.exe"), Path.Combine(installationFolder, "denlauncher.exe"));
        }

        public static async Task<string> GetCurrentVersion(string installationFolder)
        {
            string version = File.ReadAllText(Path.Combine(installationFolder, "version.txt"));
            return version;
        }

        public static async Task<string> GetTheNewestVersion()
        {
            var client = new HttpClient();
            HttpResponseMessage response = await
                client.GetAsync(@"http://buchmiet.com/api/den/whatisnewest");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            HttpContent content = response.Content;
            return await content.ReadAsStringAsync();
        }

        public static void DisplayNormaStartInfo()
        {
            Console.WriteLine("Starting the app...");
        }

        public static void DisplayDownloadingNewVersionInfo(string version)
        {
            Console.WriteLine("newer version found, will attempt to update first");
            Console.WriteLine("downloading version " + version);
        }

        public static async Task<string[]> GetFilesToDelete(string installationFolder)
        {
            return File.ReadAllLines(Path.Combine(installationFolder, "files"));
        }

        public static async Task DeleteOldInstallFiles(string installationFolder, string[] doUsu)
        {
            foreach (var VARIABLE in doUsu)
            {
                if (File.Exists(Path.Combine(installationFolder, VARIABLE)))
                {
                    File.Delete(Path.Combine(installationFolder, VARIABLE));
                }
            }
        }

        public static async Task InstallationExists(string[] args, string installationFolder)
        {
            if (CheckIfArgumentsSaysItsTheFirstStart(args))
            {
                await IfOldLauncherExistsRemoveIt(installationFolder);
                await RemoveInstallationFile(installationFolder);
                await PrepareNewLauncher(installationFolder);

                await StartProcess(installationFolder, AppName, "", ProcessRunType.FireAndForget);
                WaitForASingnalFromClientAndExit();
            }
            else
            {
                var currentVersion = await GetCurrentVersion(installationFolder);
                var version = await GetTheNewestVersion();
                if (version == null)
                {
                    DownloadErrorAndExitInfo();
                    return;
                }

                if (currentVersion.Equals(version))
                {
                    DisplayNormaStartInfo();
                    await StartProcess(installationFolder, AppName, "", ProcessRunType.FireAndForget);
                    WaitForASingnalFromClientAndExit();
                    return;
                }

                DisplayDownloadingNewVersionInfo(version);
                if (!await DownloadFileAsync(installationFolder))
                {
                    DownloadErrorAndExitInfo();
                    return;
                }

                await DeleteOldInstallFiles(installationFolder, await GetFilesToDelete(installationFolder));
                await StartProcess(installationFolder, "7zdec.exe", "x lo.7z",
                    ProcessRunType.WaitForExit);
                await StartTheAppFirstTime(installationFolder, "refresh");
            }
        }

        private static async Task Main(string[] args)
        {
            string installPath = GetInstallationPath(SoftwareDeveloperName, ApplicationFullName);
            if (installPath == null)
            {
                await StartFreshInstallation();
            }
            else
            {
                await InstallationExists(args, installPath);
            }
        }

        private enum AppState
        {
            // Stany wspólne dla instalacji i aktualizacji
            GetInstallationFolder,

            DownloadFile,
            StartApp,

            // Stany specyficzne dla instalacji
            DisplayPreInstallationInfo,

            CreateInstallationFolder,
            DisplayPostInstallationInfo,
            RunUnpacking,
            DisplayPostUnpackingInfo,
            ConfirmIconAddition,
            RemoveInstallationFile,
            SetInstallationPath,

            // Stany specyficzne dla aktualizacji
            CheckFirstStart,

            RemoveOldLauncher,
            PrepareNewLauncher,
            CheckCurrentVersion,
            DisplayNormalStartInfo,
            DisplayDownloadingNewVersionInfo,
            DeleteOldInstallFiles,
            UnpackNewVersion
        }

        private AppState currentState;
        private bool isInstallation;

        private void OnBackClicked()
        {
            if (isInstallation)
            {
                // Logika dla "Back" w procesie instalacji
                switch (currentState)
                {
                    case AppState.DisplayPreInstallationInfo:
                        currentState = AppState.GetInstallationFolder;
                        // Logika cofania do wyboru folderu instalacyjnego
                        break;

                    case AppState.CreateInstallationFolder:
                        currentState = AppState.DisplayPreInstallationInfo;
                        // Logika cofania do wyświetlania informacji przed instalacją
                        break;

                    case AppState.DownloadFile:
                        currentState = AppState.CreateInstallationFolder;
                        // Logika cofania do tworzenia folderu instalacyjnego
                        break;

                    case AppState.DisplayPostInstallationInfo:
                        currentState = AppState.DownloadFile;
                        // Logika cofania do pobierania pliku
                        break;

                    case AppState.RunUnpacking:
                        currentState = AppState.DisplayPostInstallationInfo;
                        // Logika cofania do wyświetlania informacji po instalacji
                        break;

                    case AppState.DisplayPostUnpackingInfo:
                        currentState = AppState.RunUnpacking;
                        // Logika cofania do procesu rozpakowywania
                        break;

                    case AppState.ConfirmIconAddition:
                        currentState = AppState.DisplayPostUnpackingInfo;
                        // Logika cofania do wyświetlania informacji po rozpakowaniu
                        break;

                    case AppState.RemoveInstallationFile:
                        currentState = AppState.ConfirmIconAddition;
                        // Logika cofania do potwierdzenia dodania ikony na pulpit
                        break;

                    case AppState.SetInstallationPath:
                        currentState = AppState.RemoveInstallationFile;
                        // Logika cofania do usuwania pliku instalacyjnego
                        break;

                    case AppState.StartApp:
                        currentState = AppState.SetInstallationPath;
                        // Logika cofania do ustawienia ścieżki instalacji
                        break;

                    default:
                        // W przypadku nieznanego stanu, nie wykonujemy żadnej akcji
                        break;
                }
            }
            else
            {
                switch (currentState)
                {
                    case AppState.DisplayDownloadingNewVersionInfo:
                        currentState = AppState.CheckCurrentVersion;
                        // Logika cofania do sprawdzenia bieżącej wersji
                        break;

                    case AppState.DeleteOldInstallFiles:
                        currentState = AppState.DownloadFile;
                        // Logika cofania do pobierania pliku
                        break;

                    case AppState.UnpackNewVersion:
                        currentState = AppState.DeleteOldInstallFiles;
                        // Logika cofania do usuwania starych plików instalacyjnych
                        break;

                    case AppState.CheckCurrentVersion:
                        currentState = AppState.CheckFirstStart;
                        // Logika cofania do sprawdzenia, czy to pierwsze uruchomienie
                        break;

                    case AppState.PrepareNewLauncher:
                        currentState = AppState.RemoveOldLauncher;
                        // Logika cofania do usuwania starego launchera
                        break;

                    case AppState.RemoveOldLauncher:
                        // W tym przypadku nie ma wcześniejszego stanu do cofnięcia
                        break;

                    case AppState.CheckFirstStart:
                        // W tym przypadku nie ma wcześniejszego stanu do cofnięcia
                        break;

                    default:
                        // W przypadku nieznanego stanu, nie wykonujemy żadnej akcji
                        break;
                }
            }

            UpdateUI(); // Aktualizacja interfejsu użytkownika zgodnie z bieżącym stanem
        }

        private void UpdateUI()
        {
            // Tutaj umieszczasz logikę do aktualizacji interfejsu użytkownika
        }

        private void OnContinueClicked()
        {
            if (isInstallation)
            {
                // Logika dla "Back" w procesie instalacji
                switch (currentState)
                {
                    case AppState.GetInstallationFolder:
                        currentState = AppState.DisplayPreInstallationInfo;
                        // Logika przejścia do wyświetlania informacji przed instalacją
                        break;

                    case AppState.DisplayPreInstallationInfo:
                        currentState = AppState.CreateInstallationFolder;
                        // Logika przejścia do tworzenia folderu instalacyjnego
                        break;

                    case AppState.CreateInstallationFolder:
                        currentState = AppState.DownloadFile;
                        // Logika przejścia do pobierania pliku
                        break;

                    case AppState.DownloadFile:
                        currentState = AppState.DisplayPostInstallationInfo;
                        // Logika przejścia do wyświetlania informacji po instalacji
                        break;

                    case AppState.DisplayPostInstallationInfo:
                        currentState = AppState.RunUnpacking;
                        // Logika przejścia do procesu rozpakowywania
                        break;

                    case AppState.RunUnpacking:
                        currentState = AppState.DisplayPostUnpackingInfo;
                        // Logika przejścia do wyświetlania informacji po rozpakowaniu
                        break;

                    case AppState.DisplayPostUnpackingInfo:
                        currentState = AppState.ConfirmIconAddition;
                        // Logika przejścia do potwierdzenia dodania ikony na pulpit
                        break;

                    case AppState.ConfirmIconAddition:
                        currentState = AppState.RemoveInstallationFile;
                        // Logika przejścia do usuwania pliku instalacyjnego
                        break;

                    case AppState.RemoveInstallationFile:
                        currentState = AppState.SetInstallationPath;
                        // Logika przejścia do ustawienia ścieżki instalacji
                        break;

                    case AppState.SetInstallationPath:
                        currentState = AppState.StartApp;
                        // Logika przejścia do uruchamiania aplikacji
                        break;

                    case AppState.StartApp:
                        // W tym przypadku nie ma kolejnego stanu do przejścia
                        break;

                    default:
                        // W przypadku nieznanego stanu, nie wykonujemy żadnej akcji
                        break;
                }
            }
            else
            {
                switch (currentState)
                {
                    case AppState.CheckFirstStart:
                        currentState = AppState.RemoveOldLauncher;
                        // Logika przejścia do usuwania starego launchera
                        break;

                    case AppState.RemoveOldLauncher:
                        currentState = AppState.PrepareNewLauncher;
                        // Logika przejścia do przygotowania nowego launchera
                        break;

                    case AppState.PrepareNewLauncher:
                        currentState = AppState.CheckCurrentVersion;
                        // Logika przejścia do sprawdzenia bieżącej wersji
                        break;

                    case AppState.CheckCurrentVersion:
                        currentState = AppState.DisplayDownloadingNewVersionInfo;
                        // Logika przejścia do wyświetlenia informacji o pobieraniu nowej wersji
                        break;

                    case AppState.DisplayDownloadingNewVersionInfo:
                        currentState = AppState.DownloadFile;
                        // Logika przejścia do pobierania pliku
                        break;

                    case AppState.DownloadFile:
                        currentState = AppState.DeleteOldInstallFiles;
                        // Logika przejścia do usuwania starych plików instalacyjnych
                        break;

                    case AppState.DeleteOldInstallFiles:
                        currentState = AppState.UnpackNewVersion;
                        // Logika przejścia do rozpakowywania nowej wersji
                        break;

                    case AppState.UnpackNewVersion:
                        currentState = AppState.StartApp;
                        // Logika przejścia do uruchamiania aplikacji
                        break;

                    case AppState.StartApp:
                        // W tym przypadku nie ma kolejnego stanu do przejścia
                        break;

                    default:
                        // W przypadku nieznanego stanu, nie wykonujemy żadnej akcji
                        break;
                }
            }

            UpdateUI(); // Aktualizacja interfejsu użytkownika zgodnie z bieżącym stanem
        }
    }
}