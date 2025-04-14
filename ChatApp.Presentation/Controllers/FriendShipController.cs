﻿using ChatApp.Application.DTOs.FriendShip;
using ChatApp.Application.Services.Abstracts;
using ChatApp.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Presentation.Controllers
{
    public class FriendShipController : BaseApiController
    {
        private readonly IFriendShipService _friendShipService;
        public FriendShipController(IFriendShipService friendShipService)
        {
            _friendShipService = friendShipService;
        }

        [HttpPost("Add")]
        public async Task<ActionResult<FriendShipDto>> AddFriendShip(FriendShipAddDto friendShipAddDto)
        {
            var friendShip = await _friendShipService.AddFriendShip(friendShipAddDto);
            return Ok(friendShip);
        }

        [HttpGet("Get")]
        public async Task<ActionResult<FriendShipDto>> GetFriendShip(string requesterId, string addresseeId)
        {
            var friendShip = await _friendShipService.GetFriendShip(requesterId, addresseeId);
            if (friendShip == null)
            {
                return NotFound();
            }
            return Ok(friendShip);
        }
        [HttpGet("GetByUserId")]
        public async Task<ActionResult<IEnumerable<FriendShipDto>>> GetFriendShips(string userId, FriendShipStatus? status = null)
        {
            var friendShips = await _friendShipService.GetFriendShips(userId, status);
            return Ok(friendShips);
        }

        [HttpPut("Update")]
        public async Task<ActionResult<FriendShipDto>> UpdateFriendShip(FriendShipUpdateDto friendShipUpdateDto)
        {
            var friendShip = await _friendShipService.UpdateFriendShip(friendShipUpdateDto);
            return Ok(friendShip);
        }

        [HttpGet("pending-requests/{userId}")]
        public async Task<ActionResult<List<FriendShipDto>>> GetPendingRequest(string userId)
        {
            var friendShips = await _friendShipService.GetPendingRequest(userId);
            return Ok(friendShips);
        }

        [HttpGet("friends/{userId}")]
        public async Task<ActionResult<List<FriendShipDto>>> GetFriends(string userId)
        {
            var friendShips = await _friendShipService.GetFriends(userId);
            return Ok(friendShips);
        }
    }
}
