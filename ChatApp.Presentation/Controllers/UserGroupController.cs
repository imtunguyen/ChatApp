using ChatApp.Application.DTOs.User;
using ChatApp.Application.DTOs.UserGroup;
using ChatApp.Application.Parameters;
using ChatApp.Application.Services.Abstracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Presentation.Controllers
{
    public class UserGroupController : BaseApiController
    {
        private readonly IUserGroupService _userGroupService;
        public UserGroupController(IUserGroupService userGroupService)
        {
            _userGroupService = userGroupService;
        }
        [HttpPost("Add")]
        public async Task<IActionResult> AddUserToGroup(UserGroupAddDto userGroupAddDto)
        {
            var result = await _userGroupService.AddUserToGroup(userGroupAddDto);
            return Ok(result);
        }
        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteUserFromGroup(string userId, int GroupId)
        {
            await _userGroupService.DeleteUserGroup(userId, GroupId);
            return Ok();
        }
        [HttpGet("Get")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsersInGroup(int GroupId)
        {
            var result = await _userGroupService.GetUsersInGroupAsync(GroupId);
            return Ok(result);
        }

    }
}
