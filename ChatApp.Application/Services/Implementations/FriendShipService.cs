using ChatApp.Application.DTOs.FriendShip;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Mappers;
using ChatApp.Application.Services.Abstracts;
using ChatApp.Domain.Enums;
using ChatApp.Domain.Exceptions;

namespace ChatApp.Application.Services.Implementations
{
    public class FriendShipService : IFriendShipService
    {
        private readonly IUnitOfWork _unitOfWork;
        public FriendShipService(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<FriendShipDto> AddFriendShip(FriendShipAddDto friendShipAddDto)
        {
            var friendShip = FriendShipMapper.FriendShipAddDtoToEntity(friendShipAddDto);

            // Kiểm tra xem mối quan hệ đã tồn tại chưa
            var existingFriendShip = await _unitOfWork.FriendShipRepository
                .GetFriendShip(friendShip.RequesterId, friendShip.AddresseeId);

            if (existingFriendShip != null)
            {
                if (existingFriendShip.Status == FriendShipStatus.None) // Status = 0
                {
                    existingFriendShip.Status = FriendShipStatus.Pending; // Status = 1
                    _unitOfWork.FriendShipRepository.Update(existingFriendShip);

                    return await _unitOfWork.CompleteAsync()
                        ? FriendShipMapper.FriendShipToDto(existingFriendShip)
                        : throw new BadRequestException("Lỗi khi cập nhật trạng thái friendship");
                }

                throw new BadRequestException("Đã gửi lời mời kết bạn");
            }

            // Nếu chưa có, tạo mới
            await _unitOfWork.FriendShipRepository.AddAsync(friendShip);

            return await _unitOfWork.CompleteAsync()
                ? FriendShipMapper.FriendShipToDto(friendShip)
                : throw new BadRequestException("Lỗi khi thêm friendship");
        }


        public async Task<FriendShipDto> GetFriendShip(string requesterId, string addresseeId)
        {
            var friendShip = await _unitOfWork.FriendShipRepository.GetFriendShip(requesterId, addresseeId);
            return FriendShipMapper.FriendShipToDto(friendShip);
        }

        public async Task<IEnumerable<FriendShipDto>> GetFriendShips(string userId, FriendShipStatus? status = null)
        {
            var friendShips = await _unitOfWork.FriendShipRepository.GetFriendShips(userId, status);
            return friendShips.Select(FriendShipMapper.FriendShipToDto);
        }

        public async Task<FriendShipDto> UpdateFriendShip(FriendShipUpdateDto friendShipUpdateDto)
        {
            var friendShip = FriendShipMapper.FriendShipUpdateDtoToEntity(friendShipUpdateDto);
            friendShip.Id = await _unitOfWork.FriendShipRepository.GetFriendShipId(friendShip.RequesterId, friendShip.AddresseeId);
            _unitOfWork.FriendShipRepository.UpdateFriendShip(friendShip);
            return await _unitOfWork.CompleteAsync()
                ? FriendShipMapper.FriendShipToDto(friendShip)
                : throw new BadRequestException("Lỗi khi cập nhật Friendship");
        }
    }
}
