using ChatApp.Application.DTOs.ChatRoom;
using ChatApp.Application.Services.Abstracts;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Presentation.Controllers
{
    public class ChatRoomController : BaseApiController
    {
        private readonly IChatRoomService _chatRoomService;
        public ChatRoomController(IChatRoomService chatRoomService)
        {
            _chatRoomService = chatRoomService;
        }
        [HttpPost("Add")]
        public async Task<IActionResult> CreateChatRoom(ChatRoomAddDto chatRoomAddDto)
        {
            var result = await _chatRoomService.CreateChatRoomAsync(chatRoomAddDto);
            return Ok(result);
        }
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateChatRoom(ChatRoomUpdateDto chatRoomUpdateDto)
        {
            var result = await _chatRoomService.UpdateChatRoomAsync(chatRoomUpdateDto);
            return Ok(result);
        }
        [HttpGet("Get")]
        public async Task<IActionResult> GetChatRoomsByUser(string userId)
        {
            var result = await _chatRoomService.GetChatRoomsByUserAsync(userId);
            return Ok(result);
        }
        [HttpDelete("Delete")]
        public async Task<IActionResult> RemoveChatRoom(int chatRoomId)
        {
            await _chatRoomService.DeleteChatRoomAsync(c => c.Id == chatRoomId);
            return Ok();
        }

    }
}
