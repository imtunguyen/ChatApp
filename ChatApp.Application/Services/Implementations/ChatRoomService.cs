using ChatApp.Application.DTOs.ChatRoom;
using ChatApp.Application.DTOs.UserChatRoom;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Mappers;
using ChatApp.Application.Parameters;
using ChatApp.Application.Services.Abstracts;
using ChatApp.Application.Utilities;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Exceptions;
using System.Linq.Expressions;

namespace ChatApp.Application.Services.Implementations
{
    public class ChatRoomService : IChatRoomService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ChatRoomService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ChatRoomDto> CreateChatRoomAsync(ChatRoomAddDto chatRoom)
        {
            var chatRoomEntity = ChatRoomMapper.ChatRoomAddDtoToEntity(chatRoom);
            await _unitOfWork.ChatRoomRepository.AddAsync(chatRoomEntity);
            return await _unitOfWork.CompleteAsync()
                ? ChatRoomMapper.EntityToChatRoomDto(chatRoomEntity)
                : throw new BadRequestException("Lỗi khi tạo nhóm chat");
        }

        public async Task<ChatRoomDto> UpdateChatRoomAsync(ChatRoomUpdateDto chatRoom)
        {
            var chatRoomEntity = ChatRoomMapper.ChatRoomUpdateDtoToEntity(chatRoom);
            chatRoomEntity = await _unitOfWork.ChatRoomRepository.GetAsync(c => c.Id == chatRoom.Id);
            if (chatRoomEntity == null)
            {
                throw new NotFoundException("Không tìm thấy nhóm chat");
            }
            _unitOfWork.ChatRoomRepository.Update(chatRoomEntity);
            return await _unitOfWork.CompleteAsync()
                ? ChatRoomMapper.EntityToChatRoomDto(chatRoomEntity)
                : throw new BadRequestException("Lỗi khi cập nhật nhóm chat");
        }
        public async Task<bool> DeleteChatRoomAsync(Expression<Func<ChatRoom, bool>> expression)
        {
            var chatRoom = await _unitOfWork.ChatRoomRepository.GetAsync(expression);
            if (chatRoom == null)
            {
                throw new NotFoundException("Không tìm thấy nhóm chat");
            }
            chatRoom.IsDeleted = true;
            await _unitOfWork.ChatRoomRepository.UpdateChatRoomAsync(chatRoom);
            return await _unitOfWork.CompleteAsync();
        }
        public async Task<UserChatRoomDto> AddUserToChatRoom(UserChatRoomAddDto userChatRoomAddDto)
        {
            var userChatRoom = UserChatRoomMapper.AddDtoToEntity(userChatRoomAddDto);
            await _unitOfWork.UserChatRoomRepository.AddAsync(userChatRoom);
            return await _unitOfWork.CompleteAsync()
                ? UserChatRoomMapper.EntityToDto(userChatRoom)
                : throw new BadRequestException("Lỗi khi thêm người dùng vào nhóm chat");
        }
        public async Task<UserChatRoomDto> UpdateUserChatRoom(UserChatRoomUpdateDto userChatRoomUpdateDto)
        {
            var userChatRoom = UserChatRoomMapper.UpdateDtoToEntity(userChatRoomUpdateDto);
            userChatRoom = await _unitOfWork.UserChatRoomRepository.GetAsync(u => u.ChatRoomId == userChatRoomUpdateDto.ChatRoomId && u.UserId == userChatRoomUpdateDto.UserId);
            if (userChatRoom == null)
            {
                throw new NotFoundException("Không tìm thấy người dùng trong nhóm chat");
            }
            _unitOfWork.UserChatRoomRepository.Update(userChatRoom);
            return await _unitOfWork.CompleteAsync()
                ? UserChatRoomMapper.EntityToDto(userChatRoom)
                : throw new BadRequestException("Lỗi khi cập nhật người dùng trong nhóm chat");
        }

        public async Task<bool> DeleteUserChatRoom(string userId, int chatRoomId)
        {
            var userChatRoom = await _unitOfWork.UserChatRoomRepository.GetAsync(u => u.ChatRoomId == chatRoomId && u.UserId == userId);
            if (userChatRoom == null)
            {
                throw new NotFoundException("Không tìm thấy người dùng trong nhóm chat");
            }
            _unitOfWork.UserChatRoomRepository.Update(userChatRoom);

            return await _unitOfWork.CompleteAsync();
        }

        public Task<ChatRoomDto> GetChatRoomByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ChatRoomDto>> GetChatRoomsByUserAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<UserChatRoomDto> GetUserInChatRoom(Expression<Func<UserChatRoom, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<UserChatRoomDto>> GetUsersInChatRoomAsync(UserParams userParams, int chatRoomId)
        {
            throw new NotImplementedException();
        }

        
    }
}
