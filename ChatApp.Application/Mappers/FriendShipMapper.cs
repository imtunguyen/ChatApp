using ChatApp.Application.DTOs.FriendShip;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Enums;

namespace ChatApp.Application.Mappers
{
    public class FriendShipMapper
    {
        public static FriendShip FriendShipAddDtoToEntity(FriendShipAddDto friendShipAddDto)
        {
            return new FriendShip
            {
                RequesterId = friendShipAddDto.RequesterId,
                AddresseeId = friendShipAddDto.AddresseeId,
                Status = FriendShipStatus.Pending,
                CreatedAt = DateTimeOffset.UtcNow
            };
        }
        public static FriendShip FriendShipUpdateDtoToEntity(FriendShipUpdateDto friendShipUpdateDto)
        {
            return new FriendShip
            {
                RequesterId = friendShipUpdateDto.RequesterId,
                AddresseeId = friendShipUpdateDto.AddresseeId,
                Status = friendShipUpdateDto.Status,
                AcceptedAt = friendShipUpdateDto.Status == FriendShipStatus.Accepted ? DateTimeOffset.UtcNow : null
            };
        }
        public static FriendShipDto FriendShipToDto(FriendShip friendShip)
        {
            return new FriendShipDto
            {
                Id = friendShip.Id,
                RequesterId = friendShip.RequesterId,
                AddresseeId = friendShip.AddresseeId,
                Status = friendShip.Status,
                CreatedAt = friendShip.CreatedAt,
                AcceptedAt = friendShip.AcceptedAt
            };
        }
    }
}
