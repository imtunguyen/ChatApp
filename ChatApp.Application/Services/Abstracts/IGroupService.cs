
using ChatApp.Application.DTOs.Group;
using ChatApp.Application.Parameters;
using ChatApp.Application.Utilities;
using ChatApp.Domain.Entities;
using System.Linq.Expressions;

namespace ChatApp.Application.Services.Abstracts
{
    public interface IGroupService
    {
        Task<GroupDto> CreateGroupAsync(GroupAddDto Group);
        Task<GroupDto> UpdateGroupAsync(GroupUpdateDto Group);
        Task<IEnumerable<GroupDto>> GetGroupsByUserAsync(string userId);
        Task<bool> DeleteGroupAsync(Expression<Func<Group, bool>> expression);
        Task<GroupDto> GetGroupByIdAsync(int id);


    }
}
