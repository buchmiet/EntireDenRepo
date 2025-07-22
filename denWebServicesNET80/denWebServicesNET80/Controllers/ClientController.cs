using denWebServicesNET80.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using denWebServicesNET80.Models;


namespace denWebServicesNET80.Controllers;

[Authorize(AuthenticationSchemes = "Identity.Bearer")]

[ApiController]
[Route("api/[controller]")]
public class ClientController(
    UserManager<IdentityUser> userManager,
    IConfiguration configuration,
    IUserServices userservices,
    ISignalRActions signalRActions)
    : Controller
{
    private readonly IConfiguration _configuration = configuration;

    [HttpGet("AllocateClient", Name = "AllocateClient")]
    public async Task<IActionResult> AllocateClient()
    {
        IdentityUser? user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized("User not found");
        }

        int maxUsers;
        try
        {
            maxUsers = await userservices.GetMaxClients(user.UserName);
        }
        catch (Exception ex)
        {
            return BadRequest("error");
        }


        var loggedInClients = await userservices.GetLoggedInClients(user.UserName);
        Console.WriteLine(user.UserName + " is attempting to make connection");

        if (await RefreshConnectionsIfNeeded())
        {
            var newLoggedInClient = await CreateNewLoggedInClient();
            if (newLoggedInClient != null)
            {
                Console.WriteLine("{0} was granted the connection with handshake {1}",user.UserName,newLoggedInClient.Handshake);
                return Ok(newLoggedInClient.Handshake);
            }
        }

        return BadRequest("Max number of clients logged in");

         
        async Task<bool> RefreshConnectionsIfNeeded()
        {
            if (loggedInClients.Count >= maxUsers)
            {
                await CheckForDeadConnections(user.UserName);
                loggedInClients = await userservices.GetLoggedInClients(user.UserName);
                return loggedInClients.Count < maxUsers;
            }
            return true;
        }

         
        async Task<LoggedInClient?> CreateNewLoggedInClient()
        {
            var allClientNames = await userservices.GetClientNames(user.UserName);
            var loggedInClientNames = new HashSet<string>(loggedInClients.Select(p => p.ClientName));
            var newClient = allClientNames.FirstOrDefault(client => !loggedInClientNames.Contains(client.ClientName));
            if (newClient != null)
            {
                var newLoggedInClient = new LoggedInClient
                {
                    UserClientNamesId = newClient.UserClientNamesId,
                    TimeCreated = DateTime.Now,
                    ConnectionId = "" 
                };
                await userservices.AddLoggedInClient(newLoggedInClient);
                return newLoggedInClient;
            }
            return null;
        }
    }


    public async Task CheckForDeadConnections(string username)
    {
        await userservices.RemoveDeadLoggedInUsers(username);

        var connectionIds = await userservices.GetConnectionIdsForAnUser(username);
        var response = await signalRActions.CheckIfConnectionIsAlive(connectionIds);
        if (response.Values.Any(p => !p))
        {
            await userservices.RemoveLoggedInClientsWithProvidedConnectionId(response.Where(p => !p.Value).Select(p => p.Key)
                .ToList());
        }
    }


    
}