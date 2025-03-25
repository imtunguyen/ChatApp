using ChatApp.Application.DTOs.User;
using ChatApp.Domain.Entities;
using ChatApp.Domain.ValueObjects;

namespace ChatApp.Application.Mappers
{
    public class UserMapper
    {
        public static UserDto EntityToUserDto(AppUser user)
        {
            return new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                UserName = user.UserName, 
                Email = user.Email,  
                Gender = user.Gender.ToString(),
                ProfilePictureUrl = user.ProfilePictureUrl, 
                
            };
        }
    }
}
