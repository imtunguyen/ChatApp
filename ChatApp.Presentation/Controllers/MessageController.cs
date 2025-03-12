using ChatApp.Application.DTOs.Message;
using ChatApp.Application.Parameters;
using ChatApp.Application.Services.Abstracts;
using ChatApp.Presentation.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Presentation.Controllers
{
    public class MessageController : BaseApiController
    {
        private readonly IMessageService _messageService;
        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }
        [HttpGet("Get/{id}")]
        public async Task<IActionResult> GetMessageById(int id)
        {
            var result = await _messageService.GetMessageByIdAsync(id);
            return Ok(result);
        }
        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetAllMessages([FromQuery] MessageParams messageParams)
        {
            var messages = await _messageService.GetAllAsync(messageParams, false);
            Response.AddPaginationHeader(messages);
            return Ok(messages);
        }
        [HttpGet("GetMessagesThread")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesThread([FromQuery] MessageParams messageParams, string senderId, string recipientId)
        {
            var messages = await _messageService.GetMessagesThreadAsync(messageParams, senderId, recipientId);
            Response.AddPaginationHeader(messages);
            return Ok(messages);
        }
        [HttpGet("GetMessagesGroup")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesGroup([FromQuery] MessageParams messageParams, int groupId)
        {
            var messages = await _messageService.GetMessagesGroupAsync(messageParams, groupId);
            Response.AddPaginationHeader(messages);
            return Ok(messages);
        }
        [HttpGet("GetLastMessage")]
        public async Task<ActionResult<MessageDto?>> GetLastMessage(string senderId, string recipientId)
        {
            var message = await _messageService.GetLastMessageAsync(senderId, recipientId);
            return Ok(message);
        } 
        [HttpPost("Add")]
        public async Task<IActionResult> AddMessage([FromForm] MessageAddDto messageDto)
        {
            var message = await _messageService.AddMessageAsync(messageDto);
            return CreatedAtAction(nameof(GetAllMessages), new { id = message.Id }, message);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateMessage([FromForm] MessageUpdateDto messageDto)
        {
            var message = await _messageService.UpdateMessageAsync(messageDto);
            return Ok(message);
        }

        [HttpPut("Delete")]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            await _messageService.DeleteMessageAsync(m => m.Id == id);
            return NoContent();
        }

    }
}
