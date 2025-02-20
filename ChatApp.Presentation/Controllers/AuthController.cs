
using ChatApp.Application.Abstracts.Services;
using ChatApp.Application.DTOs.Auth;
using ChatApp.Application.DTOs.Email;
using ChatApp.Application.Mappers;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Enums;
using ChatApp.Domain.ValueObjects;
using ChatApp.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static ChatApp.Application.Abstracts.Services.Identity.Response;

namespace ChatApp.Presentation.Controllers
{
    public class AuthController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly ICloudinaryService _cloudinaryService;

        public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
            ITokenService tokenService, IEmailService emailService, ICloudinaryService cloudinaryService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _emailService = emailService;
            _cloudinaryService = cloudinaryService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.UserNameOrEmail))
            {
                return BadRequest("Tên đăng nhập hoặc email không thể trống.");
            }

            EmailAddress email = null;
            Username username = null;
            var password = new Password(dto.Password);

            try
            {
                email = new EmailAddress(dto.UserNameOrEmail);
            }
            catch (Exception)
            {
                username = new Username(dto.UserNameOrEmail);
            }

            var user = email != null
                ? await _userManager.FindByEmailAsync(email.Value)
                : await _userManager.FindByNameAsync(username?.Value);

            if (user == null)
            {
                return Unauthorized("Người dùng không tồn tại.");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, password.Value, false);

            if (!result.Succeeded)
            {
                return Unauthorized("Mật khẩu không đúng");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            };

            var accessToken = _tokenService.GenerateToken(claims);
            var refreshToken = _tokenService.GenerateRefreshToken();

            var res = new LoginRespone
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                RefreshTokenExpriedtime = DateTime.Now.AddDays(1),
                User = UserMapper.EntityToUserDto(user)
            };

            return Ok(res);

        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromForm] RegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userName = new Username(dto.UserName);
            var email = new EmailAddress(dto.Email);
            var password = new Password(dto.Password);

            var emailExists = await CheckEmailAndUserNameExistAsync(email.Value, userName.Value);
            if (emailExists.Item1)
            {
                return BadRequest("Email đã tồn tại");
            }

            if (emailExists.Item2)
            {
                return BadRequest("Tên người dùng đã tồn tại");
            }

            var avatarUrl = dto.ProfilePicture != null && dto.ProfilePicture.Length > 0
                ? (await _cloudinaryService.UploadPhotoAsync(dto.ProfilePicture)).Url
                : @"https://res.cloudinary.com/dlhwuvhhp/image/upload/v1734098200/user_rvnqoh.png";

            var user = new AppUser
            {
                UserName = dto.UserName,
                FullName = dto.FullName,
                Email = dto.Email,
                Gender = Enum.Parse<GenderType>(dto.Gender, true),
                ProfilePictureUrl = avatarUrl
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (result.Succeeded)
            {
                //var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                //var confirmationLink = Url.Action("ConfirmEmail", "Auth", new { userId = user.Id, token }, Request.Scheme);
                //if (confirmationLink == null)
                //{
                //    return BadRequest("Confirmation link is null. Check if the action and controller exist.");
                //}
                //var emailRequest = new EmailRequest
                //{
                //    To = user.Email,
                //    Subject = "Confirm your email",
                //    Content = $"Please confirm your email by clicking <a href='{confirmationLink}'>here</a>"
                //};
                //await _emailService.SendMailAsync(CancellationToken.None, emailRequest);
                //var resultAddRole = await _userManager.AddToRoleAsync(user, "User");
                //if (resultAddRole.Succeeded)
                //{
                    return Ok();
                //}

            }
            return BadRequest(result.Errors);
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                return BadRequest("User not found");
            }
            var result = await _userManager.ConfirmEmailAsync(user.Result, token);
            if (result.Succeeded)
            {
                return Ok("Email Confirmed successfully");
            }
            return BadRequest("Email confirmation failed");
        }

        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var userDtos = users.Select(UserMapper.EntityToUserDto);
            return Ok(userDtos);
        }

        [HttpGet("GetUserById")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("Không tìm thấy người dùng");
            }
            var userDto = UserMapper.EntityToUserDto(user);
            return Ok(userDto);
        }


        private async Task<(bool, bool)> CheckEmailAndUserNameExistAsync(string email, string userName)
        {
            var emailExists = await _userManager.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower());
            var userNameExists = await _userManager.Users.AnyAsync(u => u.UserName.ToLower() == userName.ToLower());

            return (emailExists, userNameExists);
        }
    }
}

