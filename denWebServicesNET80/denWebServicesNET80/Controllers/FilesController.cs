using DataServicesNET80.Extensions;
using DataServicesNET80.Models;
using DataServicesNET80;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace denWebServicesNET80.Controllers;
#if !DEBUG
    [Authorize(AuthenticationSchemes = "Bearer")]
#endif
[Route("api/[controller]")]
[ApiController]
public class FilesController : ControllerBase
{
    [HttpGet("WhatIsNewestVersion", Name = "WhatIsNewestVersion")]
    public IActionResult WhatIsNewestVersion()
    {
        string version;
        if (!System.IO.File.Exists(@"C:\Inetpub\vhosts\time4parts.co.uk\den\version.txt"))
        {
            version = "0";
        }
        else
        {
            var raportFI = System.IO.File.ReadAllLines(@"C:\Inetpub\vhosts\time4parts.co.uk\den\version.txt");

            var versiones = raportFI[^1].Split(':');
            version = versiones[^1];
        }

        return new ContentResult()
        {
            Content = version,
            ContentType = "text/html",
        };
    }

    [HttpGet("AdjustShopPrices", Name = "AdjustShopPrices")]
    public async Task<IActionResult> AdjustShopPrices()
    {
        // Initialize unit of work and services for database operations
        var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
        var itemBodyService = new EntityService<itembody>(unitOfWork);
        var itemHeaderService = new EntityService<itemheader>(unitOfWork);
        var shopItemService = new EntityService<shopitem>(unitOfWork);
        var itemMarketAssService = new EntityService<itmmarketassoc>(unitOfWork);
        // Load all active item bodies for the specified brand
        List<itembody> itemBodies = (await itemBodyService.GetAllAsync(
            p => p.brandID == 1 && p.readyTotrack && !p.name.ToLower().Equals("discontinued"))).ToList();

        // Load all shop items
        var shopItems = (await shopItemService.GetAllAsync()).ToList();

        // Load item headers from Casio UK (supplierID == 1) and Casio PL (supplierID == 2)
        var itemHeadersCasioUk = (await itemHeaderService.GetAllAsync(
            p => p.supplierID == 1 && p.locationID == 1)).ToList();
        var itemHeadersCasioPl = (await itemHeaderService.GetAllAsync(
            p => p.supplierID == 2 && p.locationID == 1)).ToList();

        // Create dictionaries for quick price lookup by item body ID
        var casioUkPricesByItembodyId = itemHeadersCasioUk.ToDictionary(p => p.itembodyID, q => q.pricePaid);
        var casioPlPricesByItembodyId = itemHeadersCasioPl.ToDictionary(p => p.itembodyID, q => q.pricePaid * q.xchgrate);

        // Create a dictionary for quick item body lookup by item body ID
        var itemBodiesById = itemBodies.ToDictionary(p => p.itembodyID);

        var validItems = await itemMarketAssService.GetAllAsync(
            p => p.itembodyID != 6191 && p.marketID == 1 && !p.itemNumber.ToLower().Equals("none"));
        Dictionary<int, int> itemsOnMarkets = new();
        foreach (var item in validItems)
        {
            itemsOnMarkets[item.itembodyID] = item.quantitySold;
        }

        int i = 0;

        foreach (var shopItem in shopItems)
        {
            // Try to get the corresponding item body for the shop item
            if (itemBodiesById.TryGetValue(shopItem.itembodyID, out var itemBody))
            {

                decimal priceUK = -1;
                decimal pricePL = -1;
                decimal finalPrice = -1;

                int ebayQuantity = 1;
                if (itemsOnMarkets.TryGetValue(itemBody.itembodyID, out var quantity))
                {
                    ebayQuantity = quantity;
                }

                // Calculate the selling price based on Casio UK prices if available
                if (casioUkPricesByItembodyId.TryGetValue(itemBody.itembodyID, out var costUK))
                {
                    var calculatedPriceUk = CalculateSellingPrice(costUK);
                    if (ebayQuantity != shopItem.soldQuantity)
                    {
                        //    Console.WriteLine($"Price per 1: {calculatedPriceUk}, price per {ebayQuantity}: {CalculateSellingPrice(costUK* ebayQuantity)} ");
                        calculatedPriceUk = CalculateSellingPrice(costUK * ebayQuantity);
                        shopItem.soldQuantity = ebayQuantity;
                    }
                    priceUK = RoundPrice(calculatedPriceUk);
                }

                // Calculate the selling price based on Casio PL prices if available
                if (casioPlPricesByItembodyId.TryGetValue(itemBody.itembodyID, out var costPL))
                {
                    var calculatedPricePl = (costPL * 100 + 50) / 63;
                    pricePL = RoundPrice(calculatedPricePl);
                }

                // Determine the final price based on the higher of the two prices
                finalPrice = Math.Max(priceUK, pricePL);

                // Update the shop item's price
                shopItem.price = finalPrice;
            }

            
        }

        // Save all changes to the database at once
        await unitOfWork.SaveChangesAsync();

        // Local function to calculate the selling price
        decimal CalculateSellingPrice(decimal cost)
        {
            decimal packagingCost = 1.7m;
            decimal totalCost = cost + packagingCost;
            decimal factor = 0.565m; // (1 - VAT - Payment processing fee - Profit margin)
            return totalCost / factor;
        }

        // Local function to round the price according to business rules
        decimal RoundPrice(decimal price)
        {
            var step = Math.Ceiling(price / 0.5m) * 0.5m;
            return Math.Round(step - 0.01m, 2);
        }


        return null;
    }
}