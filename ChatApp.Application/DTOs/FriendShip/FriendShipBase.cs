using ChatApp.Domain.Enums;

namespace ChatApp.Application.DTOs.FriendShip
{
    public class FriendShipBase
    {
        public required string RequesterId { get; set; }
        public required string AddresseeId { get; set; }
        
    }
}
