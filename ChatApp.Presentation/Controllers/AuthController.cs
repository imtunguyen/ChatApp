
using ChatApp.Application.Abstracts.Services;
using ChatApp.Application.DTOs.Auth;
using ChatApp.Application.DTOs.Email;
using ChatApp.Application.DTOs.User;
using ChatApp.Application.Mappers;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Enums;
using ChatApp.Domain.ValueObjects;
using ChatApp.Infrastructure.Configuration;
using ChatApp.Infrastructure.Configurations;
using ChatApp.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;
using static ChatApp.Application.Abstracts.Services.Identity.Response;

namespace ChatApp.Presentation.Controllers
{
    public class AuthController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly EmailConfig _emailConfig;
        private readonly ICloudinaryService _cloudinaryService;

        public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
            ITokenService tokenService, IEmailService emailService, ICloudinaryService cloudinaryService, IOptions<EmailConfig> emailConfig)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _emailService = emailService;
            _cloudinaryService = cloudinaryService;
            _emailConfig = emailConfig.Value;
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
                return Ok();
            }
            return BadRequest(result.Errors);
        }

        [HttpGet("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(CancellationToken cancellationToken, string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest("Email không hợp lệ");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Ok(new { Message = "Vui lòng kiểm tra Email" });
            }

            string host = _emailConfig.AppUrl?.TrimEnd('/');
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            string encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            string resetPasswordUrl = $"{host}/reset-password?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(encodedToken)}";

            string body = $@"
                <p>Chào {user.UserName},</p>
                <p>Bạn đã yêu cầu đặt lại mật khẩu. Vui lòng bấm vào link bên dưới để đặt lại:</p>
                <p><a href=""{resetPasswordUrl}"">Bấm vào đây để đổi lại mật khẩu mới</a></p>
                <p>Nếu bạn không yêu cầu, hãy bỏ qua email này.</p>";

            await _emailService.SendMailAsync(cancellationToken, new EmailRequest
            {
                To = email,
                Subject = "Reset Password",
                Content = body
            });

            return Ok(new { Message = "Vui lòng kiểm tra Email" });
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            string decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));
            var result = await _userManager.ResetPasswordAsync(user, decodedToken, request.NewPassword);

            if (!result.Succeeded)
            {
                return BadRequest("Token không hợp lệ hoặc đã hết hạn!");
            }

            return Ok(new { Message = "Đổi mật khẩu thành công!" });
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

        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromForm] UserUpdateDto user)
        {
            if (string.IsNullOrEmpty(user.Id))
            {
                return BadRequest("ID người dùng không hợp lệ.");
            }

            var userEntity = await _userManager.FindByIdAsync(user.Id);
            if (userEntity == null)
            {
                return NotFound("Không tìm thấy người dùng.");
            }

            // Kiểm tra và cập nhật ảnh đại diện nếu có
            if (user.ProfilePicture != null)
            {
                try
                {
                    userEntity.ProfilePictureUrl = (await _cloudinaryService.UploadPhotoAsync(user.ProfilePicture)).Url;
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Lỗi khi tải ảnh lên: {ex.Message}");
                }
            }

            userEntity.FullName = user.FullName;

            if (!string.IsNullOrEmpty(user.Email) && !user.Email.Equals(userEntity.Email, StringComparison.OrdinalIgnoreCase))
            {
                var emailExists = await _userManager.Users.AnyAsync(u => u.Email.ToLower() == user.Email.ToLower() && u.Id != userEntity.Id);
                if (emailExists)
                {
                    return BadRequest("Email đã tồn tại.");
                }
                userEntity.Email = user.Email;
            }


            // Kiểm tra và chuyển đổi giới tính một cách an toàn
            if (Enum.TryParse(user.Gender, true, out GenderType gender))
            {
                userEntity.Gender = gender;
            }
            else
            {
                return BadRequest("Giới tính không hợp lệ.");
            }

            var result = await _userManager.UpdateAsync(userEntity);
            if (!result.Succeeded)
            {
                return BadRequest("Cập nhật thất bại.");
            }

            var userDto = UserMapper.EntityToUserDto(userEntity);
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

