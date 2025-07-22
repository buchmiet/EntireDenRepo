using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace denWebServicesNET80.Controllers;

[Authorize(AuthenticationSchemes = "Bearer")]
[ApiController]
[Route("api/[controller]")]
public class SearchController : Controller
{
    public IActionResult Index()
    {
        return Ok();
    }
}