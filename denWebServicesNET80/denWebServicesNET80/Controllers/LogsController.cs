using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace denWebServicesNET80.Controllers;

[Authorize(AuthenticationSchemes = "Identity.Application")]
[ApiController]
[Route("[controller]")]
public class LogsController(ILogger<LogsController> logger) : ControllerBase
{
    [HttpGet]
    public IActionResult GetLogs()
    {
        var logDirectoryPath = "logs";
        if (!Directory.Exists(logDirectoryPath))
        {
            return NotFound("Katalog logów nie został znaleziony.");
        }

        var logFiles = Directory.GetFiles(logDirectoryPath, "*.log")
            .OrderByDescending(f => f)
            .ToList();

        if (!logFiles.Any())
        {
            return NotFound("Pliki logów nie zostały znalezione.");
        }

        var sb = new StringBuilder();
        sb.Append("<html><body style='background-color:black; color:white; font-family:monospace;'>");

        foreach (var logFilePath in logFiles)
        {
            try
            {
                using var fileStream = new FileStream(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var reader = new StreamReader(fileStream);
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line.Contains("[ERR]"))
                    {
                        sb.Append($"<div style='color:red;'>{line}</div>");
                    }
                    else if (line.Contains("[INF]"))
                    {
                        sb.Append($"<div style='color:gray;'>{line}</div>");
                    }
                    else
                    {
                        sb.Append($"<div>{line}</div>");
                    }
                }
            }
            catch (IOException ex)
            {
                logger.LogError(ex, $"Błąd podczas odczytu pliku logów: {logFilePath}");
            }
        }

        sb.Append("</body></html>");

        return Content(sb.ToString(), "text/html", Encoding.UTF8);
    }
}