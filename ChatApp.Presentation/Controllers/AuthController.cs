
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
        private readonly ICloudinaryService _cloudinaryService;

        public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
            ITokenService tokenService, ICloudinaryService cloudinaryService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
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
                : await _userManager.FindByNameAsync(username.Value);


            var result = await _signInManager.CheckPasswordSignInAsync(user, password.Value, false);

            if (!result.Succeeded)
            {
                return Unauthorized("Mật khẩu không đúng");
            }

            var userDto = UserMapper.EntityToUserDto(user);
            userDto.Token = await _tokenService.CreateToken(user);
            return Ok(userDto);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterDto dto)
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

            var user = new AppUser
            {
                FullName = dto.FullName,
                UserName = userName.Value,
                Email = email.Value,
                Gender = dto.Gender
            };

            if (dto.ProfilePicture != null && dto.ProfilePicture.Length > 0)
            {
                var uploadResult = await _cloudinaryService.UploadPhotoAsync(dto.ProfilePicture);
                user.ProfilePictureUrl = uploadResult?.Url;
            }
            else
            {
                user.ProfilePictureUrl = "avatar.jpg";
            }


            var result = await _userManager.CreateAsync(user, password.Value);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            var userDto = UserMapper.EntityToUserDto(user);
            userDto.Token = await _tokenService.CreateToken(user);
            return Ok(userDto);
        }

        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var userDtos = users.Select(UserMapper.EntityToUserDto);
            return Ok(userDtos);
        }


        private async Task<(bool, bool)> CheckEmailAndUserNameExistAsync(string email, string userName)
        {
            var emailExists = await _userManager.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower());
            var userNameExists = await _userManager.Users.AnyAsync(u => u.UserName.ToLower() == userName.ToLower());

            return (emailExists, userNameExists);
        }
    }
}

