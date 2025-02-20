using ChatApp.Application.DTOs.User;
using ChatApp.Application.DTOs.UserGroup;
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
    public class UserGroupService : IUserGroupService
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserGroupService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<UserGroupDto> AddUserToGroup(UserGroupAddDto userGroupAddDto)
        {
            var userGroup = UserGroupMapper.AddDtoToEntity(userGroupAddDto);
            await _unitOfWork.UserGroupRepository.AddAsync(userGroup);
            return await _unitOfWork.CompleteAsync()
                ? UserGroupMapper.EntityToDto(userGroup)
                : throw new BadRequestException("Lỗi khi thêm người dùng vào nhóm chat");
        }
        public async Task<UserGroupDto> UpdateUserGroup(UserGroupUpdateDto userGroupUpdateDto)
        {
            var userGroup = UserGroupMapper.UpdateDtoToEntity(userGroupUpdateDto);
            userGroup = await _unitOfWork.UserGroupRepository.GetAsync(u => u.GroupId == userGroupUpdateDto.GroupId && u.UserId == userGroupUpdateDto.UserId);
            if (userGroup == null)
            {
                throw new NotFoundException("Không tìm thấy người dùng trong nhóm chat");
            }
            _unitOfWork.UserGroupRepository.Update(userGroup);
            return await _unitOfWork.CompleteAsync()
                ? UserGroupMapper.EntityToDto(userGroup)
                : throw new BadRequestException("Lỗi khi cập nhật người dùng trong nhóm chat");
        }

        public async Task<bool> DeleteUserGroup(string userId, int GroupId)
        {
            var userGroup = await _unitOfWork.UserGroupRepository.GetAsync(u => u.GroupId == GroupId && u.UserId == userId);
            if (userGroup == null)
            {
                throw new NotFoundException("Không tìm thấy người dùng trong nhóm chat");
            }
            _unitOfWork.UserGroupRepository.Update(userGroup);

            return await _unitOfWork.CompleteAsync();
        }
        public async Task<IEnumerable<UserDto>> GetUsersInGroupAsync(int GroupId)
        {
            var users = await _unitOfWork.UserGroupRepository.GetUsersInGroup(GroupId);
            return users.Select(UserMapper.EntityToUserDto);
        }

    }
}
