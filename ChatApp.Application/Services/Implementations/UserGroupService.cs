using ChatApp.Application.DTOs.User;
using ChatApp.Application.DTOs.UserGroup;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Mappers;
using ChatApp.Application.Parameters;
using ChatApp.Application.Services.Abstracts;
using ChatApp.Application.Utilities;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Exceptions;
using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace ChatApp.Application.Services.Implementations
{
    public class UserGroupService : IUserGroupService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleManager<AppRole> _roleManager;
        public UserGroupService(IUnitOfWork unitOfWork, RoleManager<AppRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
        }
        public async Task<List<UserGroupDto>> AddMultipleUsersToGroup(List<UserGroupAddDto> userGroupAddDtos)
        {
            var userGroupDtos = new List<UserGroupDto>();

            foreach (var userGroupAddDto in userGroupAddDtos)
            {
                var memberRole = await _roleManager.FindByNameAsync("Member");
                if(userGroupAddDto.RoleId == null)
                {
                    userGroupAddDto.RoleId = memberRole.Id;

                }
                var userGroup = UserGroupMapper.AddDtoToEntity(userGroupAddDto);
                await _unitOfWork.UserGroupRepository.AddAsync(userGroup);
                userGroupDtos.Add(UserGroupMapper.EntityToDto(userGroup));
            }

            if (!await _unitOfWork.CompleteAsync())
            {
                throw new BadRequestException("Lỗi khi thêm nhiều người dùng vào nhóm chat");
            }

            return userGroupDtos;
        }

        public async Task<UserGroup> UpdateRole(UserGroupUpdateDto userGroupUpdateDto)
        {
            var memberRole = await _roleManager.FindByNameAsync("Member");
            var ownerRole = await _roleManager.FindByNameAsync("GroupOwner");
            var userGroup = UserGroupMapper.UpdateDtoToEntity(userGroupUpdateDto);
            userGroup = await _unitOfWork.UserGroupRepository.GetAsync(u => u.GroupId == userGroupUpdateDto.GroupId && u.UserId == userGroupUpdateDto.UserId);
           
            if (userGroup.RoleId == memberRole.Id)
            {
                userGroup.RoleId = ownerRole.Id;
            }
            _unitOfWork.UserGroupRepository.Update(userGroup);
            await _unitOfWork.CompleteAsync();

            return userGroup;
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

        public async Task<bool> RemoveUserFromGroup(string userId, int groupId)
        {

            var userGroup = await _unitOfWork.UserGroupRepository.GetAsync(u =>
                u.UserId == userId && u.GroupId == groupId);

            if (userGroup == null)
            {
                throw new NotFoundException("Không tìm thấy người dùng trong nhóm chat");
            }
            userGroup.IsRemoved = true;
            userGroup.RemovedAt = DateTimeOffset.UtcNow;
            _unitOfWork.UserGroupRepository.Update(userGroup);
            return await _unitOfWork.CompleteAsync();

        }
        public async Task<IEnumerable<UserDto>> GetUsersInGroupAsync(int groupId)
        {
            var userGroups = await _unitOfWork.UserGroupRepository.GetUsersInGroupAsync(groupId);

            return userGroups.Select(ug => new UserDto
            {
                Id = ug.UserId,
                FullName = ug.User.FullName,
                Email = ug.User.Email,
                ProfilePictureUrl = ug.User.ProfilePictureUrl,
                RoleId = ug.RoleId,
                IsRemoved = ug.IsRemoved,
                RemovedAt = ug.RemovedAt
            });
        }

    }
}
