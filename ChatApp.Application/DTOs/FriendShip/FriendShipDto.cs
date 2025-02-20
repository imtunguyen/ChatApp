using ChatApp.Domain.Enums;

namespace ChatApp.Application.DTOs.FriendShip
{
    public class FriendShipDto : FriendShipBase
    {
        public int Id { get; set; }
        public FriendShipStatus Status { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? AcceptedAt { get; set; }
    }
}
