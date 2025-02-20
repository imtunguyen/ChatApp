using ChatApp.Domain.Enums;

namespace ChatApp.Domain.Entities
{
    public class FriendShip : BaseEntity
    {
        public required string RequesterId { get; set; }
        public required string AddresseeId { get; set; }
        public FriendShipStatus Status { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? AcceptedAt { get; set; }

        public AppUser? Requester { get; set; }
        public AppUser? Addressee { get; set; }
    }
}
