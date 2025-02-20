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
        Task<UserGroupDto> AddUserToGroup(UserGroupAddDto userGroupAddDto);
        Task<UserGroupDto> UpdateUserGroup(UserGroupUpdateDto userGroupUpdateDto);
        Task<bool> DeleteUserGroup(string userId, int GroupId);
        Task<IEnumerable<UserDto>> GetUsersInGroupAsync(int GroupId);
    }
}
