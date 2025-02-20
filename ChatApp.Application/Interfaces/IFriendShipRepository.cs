using ChatApp.Domain.Entities;
using ChatApp.Domain.Enums;

namespace ChatApp.Application.Interfaces
{
    public interface IFriendShipRepository : IRepository<FriendShip>
    {
        void UpdateFriendShip(FriendShip friendShip);
        Task<FriendShip> GetFriendShip(string requesterId, string addresseeId);
        Task<IEnumerable<FriendShip>> GetFriendShips(string userId, FriendShipStatus? status = null);
        Task<int> GetFriendShipId(string requesterId, string addresseeId);
    }
}
