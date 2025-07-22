using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace denWebServicesNET80.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DenController : ControllerBase
{

    // Ścieżki i nazwy plików
    private const string BaseDirectoryPath = @"C:\Inetpub\vhosts\time4parts.co.uk\den";
    private const string FilesContentFileName = "files";
    private const string ArchiveFileName = "lo.7z";
    private const string FullVersionFilePath = @"C:\Inetpub\vhosts\time4parts.co.uk\den\version.txt";
    private const string FullErrorLogFilePath = @"C:\Inetpub\vhosts\time4parts.co.uk\den\error.txt";

    // Klucze w danych formularza (IFormCollection)
    private const string VersionFormField = "version";
    private const string FilesFormField = "files";

    // Trasy (Routes) i nazwy akcji
    private const string Upload2Action = "Upload2";
    private const string Upload3Action = "Upload3";
    private const string UploadAction = "Upload";
    private const string DownloadNewestAction = "DownloadNewest";
    private const string WhatIsNewestAction = "WhatIsNewest";

    // Typy zawartości (Content Types) i nagłówki HTTP
    private const string OctetStreamContentType = "application/octet-stream";
    private const string HtmlContentType = "text/html";
    private const string ContentDispositionAttachment = "attachment";

    // Inne stałe
    private const char VersionFileDelimiter = ':';
    private const string DefaultVersion = "0";


    [Authorize]
    [HttpPost(Upload2Action, Name = Upload2Action)]
    public async Task<IActionResult> Upload2(IFormCollection formData)
    {
        var files = HttpContext.Request.Form.Files;

        var raportFI = System.IO.File.AppendText(FullVersionFilePath);
        await raportFI.WriteLineAsync(DateTime.Now.ToString() + VersionFileDelimiter + formData[VersionFormField]);
        raportFI.Close();

        string filesContent = formData[FilesFormField];

        string directoryPath = Path.Combine(BaseDirectoryPath, formData[VersionFormField]);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        string filePath = Path.Combine(directoryPath, FilesContentFileName);
        await System.IO.File.WriteAllTextAsync(filePath, filesContent);

        var file1 = files[0];
        await using (var fileStream = new FileStream(Path.Combine(directoryPath, file1.FileName), FileMode.Create))
        {
            await file1.CopyToAsync(fileStream);
        }
        return Ok();
    }

    [HttpPost(Upload3Action, Name = Upload3Action)]
    public async Task<IActionResult> Upload3(IFormCollection formData)
    {
        var files = HttpContext.Request.Form.Files;
        var raportFI = System.IO.File.AppendText(FullVersionFilePath);
        await raportFI.WriteLineAsync(DateTime.Now.ToString() + VersionFileDelimiter + formData[VersionFormField]);
        raportFI.Close();
        string filesContent = formData[FilesFormField];
        string directoryPath = Path.Combine(BaseDirectoryPath, formData[VersionFormField]);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        string filePath = Path.Combine(directoryPath, FilesContentFileName);
        await System.IO.File.WriteAllTextAsync(filePath, filesContent);
        var file1 = files[0];
        using (var fileStream = new FileStream(Path.Combine(directoryPath, file1.FileName), FileMode.Create))
        {
            await file1.CopyToAsync(fileStream);
        }
        return Ok();
    }

    [HttpPost(UploadAction, Name = UploadAction)]
    public async Task<IActionResult> Upload(IFormCollection formData)
    {
        var files = HttpContext.Request.Form.Files;

        var raportFI = System.IO.File.AppendText(FullVersionFilePath);
        await raportFI.WriteLineAsync(DateTime.Now.ToString() + VersionFileDelimiter + formData[VersionFormField]);
        raportFI.Close();

        _ = Directory.CreateDirectory(BaseDirectoryPath + @"\" + formData[VersionFormField]);

        await System.IO.File.WriteAllTextAsync(BaseDirectoryPath + @"\" + formData[VersionFormField] + @"\" + FilesContentFileName, formData[FilesFormField]);

        try
        {
            var file1 = files[0];
            await using var fileStream = new FileStream(BaseDirectoryPath + @"\" + formData[VersionFormField] + @"\" + ArchiveFileName, FileMode.Create);
            await file1.CopyToAsync(fileStream);
        }
        catch (Exception ex)
        {
            await System.IO.File.WriteAllTextAsync(FullErrorLogFilePath, ex.InnerException.Message.ToString());
        }
        return Ok();
    }

    [HttpGet(DownloadNewestAction, Name = DownloadNewestAction)]
    public IActionResult DownloadNewest()
    {
        var raportFI = System.IO.File.ReadAllLines(FullVersionFilePath);
        var versiones = raportFI[^1].Split(VersionFileDelimiter);
        var version = versiones[^1];
        var filePath = BaseDirectoryPath + @"\" + version + @"\" + ArchiveFileName;

        try
        {
            var fileStream = new FileStream(filePath, FileMode.Open);
            var fileName = Path.GetFileName(filePath);

            var result = new FileStreamResult(fileStream, OctetStreamContentType)
            {
                FileDownloadName = fileName
            };

            Response.Headers[HeaderNames.ContentDisposition] = new ContentDispositionHeaderValue(ContentDispositionAttachment)
            {
                FileName = fileName
            }.ToString();


            return result;

        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }


    [HttpGet(WhatIsNewestAction, Name = WhatIsNewestAction)]
    public IActionResult WhatIsNewest()
    {
        string version;
        if (!System.IO.File.Exists(FullVersionFilePath))
        {
            version = DefaultVersion;
        }
        else
        {
            var raportFI = System.IO.File.ReadAllLines(FullVersionFilePath);

            var versiones = raportFI[^1].Split(VersionFileDelimiter);
            version = versiones[^1];
        }
        return new ContentResult()
        {
            Content = version,
            ContentType = HtmlContentType,
        };
    }
}