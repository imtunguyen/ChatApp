using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Abstracts.Services
{
    public interface IUserStatusService
    {
        Task SetUserOnline(string userId);
        Task SetUserOffline(string userId);
        Task<bool> IsUserOnline(string userId);
        Task<int> GetOnlineUsersCount();
    }
}
