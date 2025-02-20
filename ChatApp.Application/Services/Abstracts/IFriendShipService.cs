using ChatApp.Application.DTOs.FriendShip;
using ChatApp.Domain.Enums;

namespace ChatApp.Application.Services.Abstracts
{
    public interface IFriendShipService
    {
        Task<FriendShipDto> AddFriendShip(FriendShipAddDto friendShipAddDto);
        Task<FriendShipDto> UpdateFriendShip(FriendShipUpdateDto friendShipUpdateDto);
        Task<FriendShipDto> GetFriendShip(string requesterId, string addresseeId);
        Task<IEnumerable<FriendShipDto>> GetFriendShips(string userId, FriendShipStatus? status = null);
    }
}
