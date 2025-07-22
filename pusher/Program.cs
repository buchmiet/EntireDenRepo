using System.Security.Cryptography;
using ZstdNet;

internal class Program
{
    public static string dodajPliki(string katalogBiezacy)
    {
        string ret = "";
        var tymczasowepliki = Directory.GetFiles(katalogBiezacy, "*.*", SearchOption.AllDirectories);
        foreach (var tym in tymczasowepliki)
        {
            FileAttributes fileAttributes = File.GetAttributes(tym);
            if ((fileAttributes & FileAttributes.Directory) == 0)
            {
                ret += tym.Substring(katalogBiezacy.Length + 1, tym.Length - katalogBiezacy.Length - 1) + Environment.NewLine;
            }
        }
        return ret;
    }

    protected static string GetMD5HashFromFile(string fileName)
    {
        using (var md5 = MD5.Create())
        {
            using (var stream = File.OpenRead(fileName))
            {
                return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", string.Empty);
            }
        }
    }

    private static string raport = "";
    private static readonly HttpClient client = new HttpClient();

    public static void UploadFileAsync(string path, string files, string version)
    {
        // we need to send a request with multipart/form-data
        var multiForm = new MultipartFormDataContent();

        // add file and directly upload it
        FileStream fs = File.OpenRead(path);
        multiForm.Add(new StreamContent(fs), "file", Path.GetFileName(path));
        multiForm.Add(new StringContent(version), "version");
        multiForm.Add(new StringContent(files), "files");
        // send request to API
        var url = "http://buchmiet.com/api/den/Upload";
        using (HttpClient client = new HttpClient())
        {
            var response = client.PostAsync(url, multiForm).Result;
            var g = response.Content.ReadAsStringAsync().Result;
        }
    }

    private static void CompressFile(string inputFile, string outputFile)
    {
        const int bufferSize = 4096 * 32; // Można dostosować w zależności od potrzeb
        CompressionOptions compressionOptions = new CompressionOptions(22);// CompressionOptions.MaxCompressionLevel;
        using (var inputStream = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
        using (var bufferedInput = new BufferedStream(inputStream, bufferSize))
        using (var outputStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
        using (var compressionStream = new CompressionStream(outputStream, compressionOptions))
        {
            bufferedInput.CopyTo(compressionStream);
        }
    }

    private static async Task Main(string[] args)
    {
        //string directory = Directory
        //    .GetParent(
        //                Directory.GetParent(
        //                                    Directory.GetParent(
        //                                            Directory.GetParent(

        //                                                                             Environment.CurrentDirectory
        //                                                                             ).ToString()

        //                                                        )
        //                                    .ToString()
        //                                    )

        //                .ToString()
        //            )

        //    .ToString();
        // projDir = directory + sciezka;
        string projDir = "c:\\den-mariadb\\denMVVM\\bin\\Release\\net8.0-windows10.0.17763.0\\publish";
        string projDl = "c:\\den-mariadb\\denLauncher\\bin\\Release";

        string version = File.ReadAllText(projDir + @"\version.txt");

        var client = new HttpClient();

        HttpResponseMessage response = await client.GetAsync(@"http://buchmiet.com/api/den/whatisnewest");

        if (response.IsSuccessStatusCode)
        {
            HttpContent content = response.Content;
            var contentStream = content.ReadAsStringAsync().Result;

            if (version.Equals(contentStream))
            {
                Console.WriteLine("this version is already uploaded");
                return;
            }
        }
        else
        {
            return;
        }

        //if (File.Exists(Path.Combine(Environment.CurrentDirectory, "lo.tar")))
        //{
        //    File.Delete(Path.Combine(Environment.CurrentDirectory, "lo.tar"));
        //    Console.WriteLine("lo.tar deleted.");
        //}

        if (File.Exists(Path.Combine(Environment.CurrentDirectory, "lo.7z")))
        {
            File.Delete(Path.Combine(Environment.CurrentDirectory, "lo.7z"));
            Console.WriteLine("lo.7z deleted.");
        }
        else Console.WriteLine("File not found");

        if (File.Exists(projDir + @"\dlauncher.exe"))
        {
            File.Delete(projDir + @"\dlauncher.exe");
        }
        File.Copy(Path.Combine(projDl, "denLauncher.exe"), projDir + @"\dlauncher.exe");
        File.Delete(projDir + @"\denlauncher.exe");

        raport += dodajPliki(projDir);
        raport += "files";
        File.WriteAllText(projDir + @"\files", raport);
        var argi = "a -t7z -m0=lzma -mx=9 -mfb=64 -md=32m -ms=on lo.7z " + projDir + @"\*";
        var o = Environment.CurrentDirectory + @"\7zr.exe";

        System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
        pProcess.StartInfo.FileName = o;
        pProcess.StartInfo.Arguments = argi;
        pProcess.StartInfo.UseShellExecute = false;
        pProcess.StartInfo.RedirectStandardOutput = true;
        pProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
        pProcess.StartInfo.CreateNoWindow = true;
        pProcess.Start();
        string output = pProcess.StandardOutput.ReadToEnd();
        pProcess.WaitForExit();
        File.Delete(projDir + @"\files");

        //using (var tarStream = File.Create("lo.tar"))
        //{
        //    TarFile.CreateFromDirectory(projDir,tarStream,false);
        //}
        //Console.WriteLine("done tar file, now compressing to zstd");
        //      CompressFile("lo.tar", "lo.tar.zstd");
        Console.WriteLine("done compressing, now uploading");
        UploadFileAsync(Path.Combine(Environment.CurrentDirectory, "lo.7z"), raport, version);
    }
}