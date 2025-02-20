using ChatApp.Application.DTOs.Group;
using ChatApp.Application.DTOs.UserGroup;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Mappers;
using ChatApp.Application.Parameters;
using ChatApp.Application.Services.Abstracts;
using ChatApp.Application.Utilities;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Exceptions;
using FluentValidation;
using System.Linq.Expressions;

namespace ChatApp.Application.Services.Implementations
{
    public class GroupService : IGroupService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<GroupAddDto> _validator;
        public GroupService(IUnitOfWork unitOfWork, IValidator<GroupAddDto> validator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task<GroupDto> CreateGroupAsync(GroupAddDto Group)
        {
            var result = await _validator.ValidateAsync(Group);
            if (!result.IsValid)
            {
                throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.ErrorMessage)));
            }
            var GroupEntity = GroupMapper.GroupAddDtoToEntity(Group);

            await _unitOfWork.GroupRepository.AddAsync(GroupEntity);
            return await _unitOfWork.CompleteAsync()
                ? GroupMapper.EntityToGroupDto(GroupEntity)
                : throw new BadRequestException("Lỗi khi tạo nhóm chat");
        }

        public async Task<GroupDto> UpdateGroupAsync(GroupUpdateDto Group)
        {
            var GroupEntity = GroupMapper.GroupUpdateDtoToEntity(Group);
            if (GroupEntity == null)
            {
                throw new NotFoundException("Không tìm thấy nhóm chat");
            }
            await _unitOfWork.GroupRepository.UpdateGroupAsync(GroupEntity);
            return await _unitOfWork.CompleteAsync()
                ? GroupMapper.EntityToGroupDto(GroupEntity)
                : throw new BadRequestException("Lỗi khi cập nhật nhóm chat");
        }
        public async Task<bool> DeleteGroupAsync(Expression<Func<Group, bool>> expression)
        {
            var Group = await _unitOfWork.GroupRepository.GetAsync(expression);
            if (Group == null)
            {
                throw new NotFoundException("Không tìm thấy nhóm chat");
            }
            Group.IsDeleted = true;
            await _unitOfWork.GroupRepository.UpdateGroupAsync(Group);
            return await _unitOfWork.CompleteAsync();
        }
        

        public Task<GroupDto> GetGroupByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<GroupDto>> GetGroupsByUserAsync(string userId)
        {
            var Groups = await _unitOfWork.GroupRepository.GetGroupsByUser(userId);
            return Groups.Select(GroupMapper.EntityToGroupDto);
        }

        
        
    }
}
