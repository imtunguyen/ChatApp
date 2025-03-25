using ChatApp.Application.DTOs.Group;
using ChatApp.Application.DTOs.UserGroup;
using ChatApp.Application.Services.Abstracts;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Presentation.Controllers
{
    public class GroupController : BaseApiController
    {
        private readonly IGroupService _GroupService;
        private readonly IUserGroupService _userGroupService;
        private readonly RoleManager<AppRole> _roleManager;
        public GroupController(IGroupService GroupService, IUserGroupService userGroupService, RoleManager<AppRole> roleManager)
        {
            _GroupService = GroupService;
            _userGroupService = userGroupService;
            _roleManager = roleManager;
        }
        [HttpPost("Add")]
        public async Task<IActionResult> CreateGroup([FromForm] GroupAddDto groupAddDto)
        {
            var result = await _GroupService.CreateGroupAsync(groupAddDto);

            var ownerRole = await _roleManager.FindByNameAsync("GroupOwner");
            var memberRole = await _roleManager.FindByNameAsync("Member");

            if (ownerRole == null || memberRole == null)
            {
                throw new BadRequestException("Vai trò không tồn tại");
            }
           
            var userGroups = new List<UserGroupAddDto>
            {
                new UserGroupAddDto
                {
                    GroupId = result.Id,
                    UserId = groupAddDto.CreatorId,
                    RoleId = ownerRole.Id
                }
            };
            

            userGroups.AddRange(groupAddDto.UserIds.Select(userId => new UserGroupAddDto
            {
                GroupId = result.Id,
                UserId = userId,
            }));

            await _userGroupService.AddMultipleUsersToGroup(userGroups);

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
