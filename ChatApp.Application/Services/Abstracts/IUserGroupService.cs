using ChatApp.Application.DTOs.User;
using ChatApp.Application.DTOs.UserGroup;
using ChatApp.Application.Parameters;
using ChatApp.Application.Utilities;
using ChatApp.Domain.Entities;
using System.Linq.Expressions;

namespace ChatApp.Application.Services.Abstracts
{
    public interface IUserGroupService
    {
        Task<List<UserGroupDto>> AddMultipleUsersToGroup(List<UserGroupAddDto> userGroupAddDtos);
        Task<UserGroupDto> UpdateUserGroup(UserGroupUpdateDto userGroupUpdateDto);
        Task<bool> RemoveUserFromGroup(string userId, int groupId);
        Task<UserGroup> UpdateRole(UserGroupUpdateDto userGroupUpdateDto);
        Task<IEnumerable<UserDto>> GetUsersInGroupAsync(int GroupId);
    }
}
