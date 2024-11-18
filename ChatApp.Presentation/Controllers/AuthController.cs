
using ChatApp.Application.DTOs.Auth;
using ChatApp.Application.Mappers;
using ChatApp.Application.Services.Abstracts;
using ChatApp.Domain.Entities;
using ChatApp.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Presentation.Controllers
{
    public class AuthController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;

        public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
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
                : await _userManager.FindByNameAsync(username.Value);


            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password.Value, false);

            if (!result.Succeeded)
            {
                return Unauthorized("Mật khẩu không đúng");
            }

            var userDto = UserMapper.EntityToUserDto(user);
            userDto.Token = await _tokenService.CreateToken(user);
            return Ok(userDto);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var emailExists = await CheckEmailAndUserNameExistAsync(dto.Email.Value, dto.UserName.Value);
            if (emailExists.Item1)
            {
                return BadRequest("Email đã tồn tại");
            }

            if (emailExists.Item2)
            {
                return BadRequest("Tên người dùng đã tồn tại");
            }

            var user = new AppUser
            {
                FullName = dto.FullName,
                UserName = dto.UserName.Value,
                Email = dto.Email.Value,
                Gender = dto.Gender
            };

            var result = await _userManager.CreateAsync(user, dto.Password.Value);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            var userDto = UserMapper.EntityToUserDto(user);
            userDto.Token = await _tokenService.CreateToken(user);
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

