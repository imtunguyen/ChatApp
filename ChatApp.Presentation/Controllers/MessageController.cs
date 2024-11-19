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
        [HttpPost("Add")]
        public async Task<IActionResult> AddMessage([FromForm]MessageAddDto messageDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var message = await _messageService.AddMessageAsync(messageDto);
            return CreatedAtAction(nameof(GetAllMessages), new { id = message.Id }, message);
        }
    }
}
