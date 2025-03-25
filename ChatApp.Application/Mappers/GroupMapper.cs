using ChatApp.Application.DTOs.Group;
using ChatApp.Application.DTOs.Message;
using ChatApp.Domain.Entities;

namespace ChatApp.Application.Mappers
{
    public class GroupMapper
    {
        public static Group GroupAddDtoToEntity(GroupAddDto Group)
        {
            return new Group
            {
                Name = Group.Name,
                CreatorId = Group.CreatorId,
                CreatedAt = DateTimeOffset.UtcNow,
            };
        }
        public static Group GroupUpdateDtoToEntity(GroupUpdateDto Group)
        {
            return new Group
            {
                Id = Group.Id,
                Name = Group.Name,
                CreatorId = Group.CreatorId,
            };
        }
        public static GroupDto EntityToGroupDto(Group Group)
        {
            return new GroupDto
            {
                Id = Group.Id,
                Name = Group.Name,
                CreatorId = Group.CreatorId,
                CreatedAt = Group.CreatedAt,
                UpdatedAt = Group.UpdatedAt,
                AvatarUrl = Group.AvatarUrl
            };
        }
       
    }
}
