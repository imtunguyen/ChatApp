using ChatApp.Application.DTOs.UserGroup;
using ChatApp.Domain.Entities;

namespace ChatApp.Application.Mappers
{
    public class UserGroupMapper
    {
        public static UserGroup AddDtoToEntity(UserGroupAddDto userGroupAddDto)
        {
            return new UserGroup
            {
                UserId = userGroupAddDto.UserId,
                GroupId = userGroupAddDto.GroupId,
                JoinedAt = DateTimeOffset.UtcNow,

            };
        }
        public static UserGroup UpdateDtoToEntity(UserGroupUpdateDto userGroupUpdateDto)
        {
            return new UserGroup
            {
                UserId = userGroupUpdateDto.UserId,
                GroupId = userGroupUpdateDto.GroupId,
                RemovedAt = DateTimeOffset.UtcNow,

            };
        }
        public static UserGroupDto EntityToDto(UserGroup userGroup)
        {
            return new UserGroupDto
            {
                UserId = userGroup.UserId,
                GroupId = userGroup.GroupId,
                JoinedAt = userGroup.JoinedAt,
                RemovedAt = userGroup.RemovedAt,
            };
        }

    }
}
