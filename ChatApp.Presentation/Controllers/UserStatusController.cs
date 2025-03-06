using ChatApp.Application.Abstracts.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Presentation.Controllers
{
    public class UserStatusController : BaseApiController
    {
        private readonly IUserStatusService _userStatus;
        public UserStatusController(IUserStatusService userStatus)
        {
            _userStatus = userStatus;
        }
        [HttpPost("set-online/{userId}")]
        public async Task<IActionResult> SetOnline(string userId)
        {
            await _userStatus.SetUserOnline(userId);
            return Ok();
        }

        [HttpPost("set-offline/{userId}")]
        public async Task<IActionResult> SetOffline(string userId)
        {
            await _userStatus.SetUserOffline(userId);
            return Ok();
        }

        [HttpGet("is-online/{userId}")]
        public async Task<IActionResult> IsOnline(string userId)
        {
            var isOnline = await _userStatus.IsUserOnline(userId);
            return Ok(isOnline);
        }

       

        [HttpPost("users-online-status")]
        public async Task<IActionResult> GetUsersOnlineStatus([FromBody] List<string> userIds)
       {
            var result = new Dictionary<string, bool>();
            foreach (var userId in userIds)
            {
                var isOnline = await _userStatus.IsUserOnline(userId);
                result[userId] = isOnline;
            }
            return Ok(result);
        }
    }
}
