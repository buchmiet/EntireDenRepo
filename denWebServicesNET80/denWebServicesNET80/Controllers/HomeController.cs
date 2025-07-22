using System.Diagnostics;
using denWebservicesNET80;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace denWebServicesNET80.Controllers;

public class HomeController(UserManager<IdentityUser> userManager, IWebHostEnvironment environment)
    : Controller
{
    private readonly ILogger<HomeController> _logger;


    public static string GetFrameworkDescription()
    {
        return System
            .Runtime
            .InteropServices
            .RuntimeInformation
            .FrameworkDescription;
    }


    public async Task<IActionResult> Index()
    {
        
        var parts = GetFrameworkDescription();
        ViewBag.Message=parts;
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}