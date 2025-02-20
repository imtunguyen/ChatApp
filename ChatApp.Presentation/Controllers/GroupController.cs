using ChatApp.Application.DTOs.Group;
using ChatApp.Application.DTOs.UserGroup;
using ChatApp.Application.Services.Abstracts;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Presentation.Controllers
{
    public class GroupController : BaseApiController
    {
        private readonly IGroupService _GroupService;
        private readonly IUserGroupService _userGroupService;
        public GroupController(IGroupService GroupService, IUserGroupService userGroupService)
        {
            _GroupService = GroupService;
            _userGroupService = userGroupService;
        }
        [HttpPost("Add")]
        public async Task<IActionResult> CreateGroup([FromForm] GroupAddDto GroupAddDto)
        {

            var result = await _GroupService.CreateGroupAsync(GroupAddDto);
            var userGroupAddDto = new UserGroupAddDto
            {
                GroupId = result.Id,
                UserId = GroupAddDto.CreatorId
            };
            await _userGroupService.AddUserToGroup(userGroupAddDto);

            foreach(var userId in GroupAddDto.UserIds)
            {
                if (userId != GroupAddDto.CreatorId) 
                {
                    var additionalUserGroupDto = new UserGroupAddDto
                    {
                        GroupId = result.Id,
                        UserId = userId,
                    };
                    await _userGroupService.AddUserToGroup(additionalUserGroupDto);
                }
            }
            return Ok(result);
        }
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateGroup(GroupUpdateDto GroupUpdateDto)
        {
            var result = await _GroupService.UpdateGroupAsync(GroupUpdateDto);
            return Ok(result);
        }
        [HttpGet("Get")]
        public async Task<IActionResult> GetGroupsByUser(string userId)
        {
            var result = await _GroupService.GetGroupsByUserAsync(userId);
            return Ok(result);
        }
        [HttpDelete("Delete")]
        public async Task<IActionResult> RemoveGroup(int GroupId)
        {
            await _GroupService.DeleteGroupAsync(c => c.Id == GroupId);
            return Ok();
        }

    }
}
