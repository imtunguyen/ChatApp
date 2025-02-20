using ChatApp.Domain.Enums;

namespace ChatApp.Application.DTOs.FriendShip
{
    public class FriendShipUpdateDto : FriendShipBase
    {
        public FriendShipStatus Status { get; set; }
    }
}
