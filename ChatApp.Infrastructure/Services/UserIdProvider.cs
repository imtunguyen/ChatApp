using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

public class UserIdProvider : IUserIdProvider
{
    public string GetUserId(HubConnectionContext connection)
    {
        var userId = connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        Console.WriteLine($"[UserIdProvider] UserId: {userId}");
        return userId;
    }


}
