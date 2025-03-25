using ChatApp.Application.DTOs.User;
using ChatApp.Application.DTOs.UserGroup;
using ChatApp.Application.Parameters;
using ChatApp.Application.Services.Abstracts;
using ChatApp.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChatApp.Presentation.Controllers
{
    public class UserGroupController : BaseApiController
    {
        private readonly IUserGroupService _userGroupService;
        private readonly RoleManager<AppRole> _roleManager;
        public UserGroupController(IUserGroupService userGroupService, RoleManager<AppRole> roleManager)
        {
            _userGroupService = userGroupService;
            _roleManager = roleManager;
        }
        [HttpPost("AddMultiple")]
        public async Task<IActionResult> AddUserToGroup(List<UserGroupAddDto> userGroupAddDtos)
        {
            var result = await _userGroupService.AddMultipleUsersToGroup(userGroupAddDtos);
            return Ok(result);
        }
        //[Authorize(Roles = "GroupOwner")]
        [HttpPut("Remove")]
        public async Task<IActionResult> RemoveUserFromGroup(string userId, int groupId)
        {

            var result = await _userGroupService.RemoveUserFromGroup(userId, groupId);
            return Ok(new { Message = "Xóa khỏi nhóm thành công" });

        }

        [HttpGet("Get")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsersInGroup(int GroupId)
        {
            var result = await _userGroupService.GetUsersInGroupAsync(GroupId);
            return Ok(result);
        }

        [HttpPut("UpdateRole")]
        public async Task<IActionResult> UpdateRoleUserGroup(UserGroupUpdateDto userDto)
        {
           var result = await _userGroupService.UpdateRole(userDto);
            return Ok(result);

        }
    }
}
