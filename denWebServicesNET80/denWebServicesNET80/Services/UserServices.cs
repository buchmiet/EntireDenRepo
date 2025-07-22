using denWebServicesNET80.Models;
using Microsoft.EntityFrameworkCore;

namespace denWebServicesNET80.Services;

public interface IUserServices
{
    Task<int> GetMaxClients(string username);
    Task<List<UserClientNames>> GetClientNames(string username);

    Task<List<UserClientNames>> GetLoggedInClients(string username);
    Task AddLoggedInClient(LoggedInClient loggedInClient);
    Task<LoggedInClient> ConnectClient(string handshake, string connectionId);
    Task RemoveDeadLoggedInUsers(string username);
    Task<List<string>> GetConnectionIdsForAnUser(string username);
    Task RemoveLoggedInClientsWithProvidedConnectionId(List<string> connectionIds);
    Task<bool> DisconnectClient(string connectionId);
    Task<string> GetClientName(int userclientnameid);

    Task<SensitiveInformation> GetSensitiveInformationForConnectionIDAndHandshake(string connectionID,
        string handshake);
}

public class UserServices : IUserServices
{
    private UsersAndClientsDbContext _context;
    public UserServices(UsersAndClientsDbContext context)
    {
        _context=context;
    }

    public async Task<int> GetMaxClients(string username)
    {

        return await _context.UserMaxClientsAssociations
            .Where(u => u.UserName.Equals(username))
            .Select(u => u.MaxClients)
            .FirstOrDefaultAsync();
    }

    public async Task<List<UserClientNames>> GetClientNames(string username)
    {
        var uid =await _context.UserMaxClientsAssociations.FirstOrDefaultAsync(p => p.UserName .Equals(username));

        return await _context.UserClientNamess
            .Where(u => u.UserId==uid.UserId).ToListAsync();
    }

    public async Task<List<UserClientNames>> GetLoggedInClients(string username)
    {
        var userid = _context.UserMaxClientsAssociations.First(p => p.UserName.Equals(username)).UserId;
        var allclients = _context.UserClientNamess
            .Where(p => p.UserId.Equals(userid)).ToList();
        var allClientIds = allclients.Select(ac => ac.UserClientNamesId).ToList();
        var loggedInClientsForAllClients = _context.LoggedInClients
            .Where(lic => allClientIds.Contains(lic.UserClientNamesId)&&lic.IsConnected).ToList();
        return allclients
            .Where(ac => loggedInClientsForAllClients.Any(lic => lic.UserClientNamesId == ac.UserClientNamesId))
            .ToList();
    }

    public async Task RemoveDeadLoggedInUsers(string username)
    {
        var userid = _context.UserMaxClientsAssociations.First(p => p.UserName.Equals(username)).UserId;
        var allclients = _context.UserClientNamess
            .Where(p => p.UserId.Equals(userid)).Select(p=>p.UserClientNamesId).ToList();
        var deadline = DateTime.Now.AddMinutes(2);
        var deadClients = _context.LoggedInClients.Where(p =>
            allclients.Contains(p.UserClientNamesId) && !p.IsConnected && p.TimeCreated < deadline).ToList();
        _context.LoggedInClients.RemoveRange(deadClients);
        await _context.SaveChangesAsync();
    }

    public async Task<SensitiveInformation> GetSensitiveInformationForConnectionIDAndHandshake(string connectionID, string handshake)
    {
        var user =await _context.LoggedInClients.FirstOrDefaultAsync(p =>
            p.IsConnected && p.Handshake.Equals(handshake) && p.ConnectionId.Equals(connectionID));
        if (user == null)
        {
            return new SensitiveInformation{UserId = -1};
        }

        var userId =
            await _context.UserClientNamess.FirstOrDefaultAsync(p => p.UserClientNamesId == user.UserClientNamesId);
        if (userId == null)
        {
            return new SensitiveInformation { UserId = -1 };
        }

        return await _context.SensitiveInformations.FirstOrDefaultAsync(p => p.UserId == userId.UserId);
    }

    public async Task RemoveLoggedInClientsWithProvidedConnectionId(List<string> connectionIds)
    {
        var deadClients = _context.LoggedInClients.Where(p => connectionIds.Contains(p.ConnectionId));
        _context.LoggedInClients.RemoveRange(deadClients);
        await _context.SaveChangesAsync();
    }


    public async Task<List<string>> GetConnectionIdsForAnUser(string username)
    {
        var allClientNames =(await GetClientNames(username)).Select(p=>p.UserClientNamesId);
        return await _context.LoggedInClients
            .Where(p => allClientNames.Contains(p.UserClientNamesId) && p.IsConnected).Select(p => p.ConnectionId).ToListAsync();
    }

    public async Task<bool> DisconnectClient(string connectionId)
    {
        var lc =await _context.LoggedInClients.FirstOrDefaultAsync(p => p.ConnectionId.Equals(connectionId));
            
        if (lc == null)
        {
            return false;
        }
        _context.LoggedInClients.Remove(lc);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<LoggedInClient> ConnectClient(string handshake, string connectionId)
    {
        var lc = await _context.LoggedInClients.FirstOrDefaultAsync(p => p.Handshake.Equals(handshake) && !p.IsConnected);
        if (lc == null)
        {
            return null;
        }
        lc.ConnectionId = connectionId;
        lc.IsConnected = true;
        await _context.SaveChangesAsync();
        return lc;
    }

    public async Task AddLoggedInClient(LoggedInClient loggedInClient)
    {
        string handshake;
        bool isUnique;
            
        do
        {
                
            handshake = Guid.NewGuid().ToString();
            isUnique = !await _context.LoggedInClients.AnyAsync(l => l.Handshake == handshake);
        } while (!isUnique);

        loggedInClient.Handshake = handshake;
        loggedInClient.IsConnected = false;
        _context.LoggedInClients.Add(loggedInClient);
        await _context.SaveChangesAsync();
    }

    public async Task<string> GetClientName(int userclientnameid)
    {
        return (await _context.UserClientNamess.FirstOrDefaultAsync(p => p.UserClientNamesId == userclientnameid))
            .ClientName;
    }
}