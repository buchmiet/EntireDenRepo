using DataServicesNET80;
using DataServicesNET80.Extensions;
using Microsoft.AspNetCore.Mvc;
using DataServicesNET80.Models;

namespace denWebservicesNET80;

[Route("api/[controller]")]
[ApiController]
public class StockShotController() : ControllerBase
{
    public async Task StockCount()
    {
        using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
        var itemHeaderService = new EntityService<itemheader>(unitOfWork);
        var itemBodyService = new EntityService<itembody>(unitOfWork);
        var stockShotService = new EntityService<stockshot>(unitOfWork);

        var bodies= await itemBodyService.GetAllAsync(p=>p.readyTotrack);
        var hedz = (await itemHeaderService.GetAllAsync(p => p.locationID == 1 && p.quantity > 0)).ToList();

        var stany = new List<stockshot>();
        var fecha = DateTime.Now;
        foreach (var itembody in bodies)
        {
            var hedy = hedz.Where(p => p.itembodyID == itembody.itembodyID).Select(p => p.quantity).Sum();
            if (hedy > 0)
            {
                stany.Add(new stockshot
                {
                    bodyid = itembody.itembodyID,
                    quantity = hedy,
                    date = fecha,
                    locationID = 1

                });
            }
        }
        await stockShotService.AddRangeAsync(stany);

    }

    [HttpGet("DoIt/{*name}", Name = "DoIt")]
    public async Task<IActionResult> DoIt(string name)
    {
        string odpo = "OK";
        if (name == null)
        {
            odpo = "null";

        }
        else
        {
            if (!name.Equals("T09BXcw6ZiZBQIjhjgOtbWCgKhqJdcIQjz2A49Lo"))
            {
                odpo = "ok";
            }
            else
            {
                await StockCount();
            }
        }
        return Ok(odpo);
    }
}