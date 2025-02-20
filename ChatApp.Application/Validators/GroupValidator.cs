using ChatApp.Application.DTOs.Group;
using FluentValidation;
using System.Data;

namespace ChatApp.Application.Validators
{
    public class GroupValidator : AbstractValidator<GroupAddDto>
    {
        public GroupValidator()
        {
            RuleFor(c => c.CreatorId)
                .NotEmpty().WithMessage("Creator is required");

            RuleFor(n => n.Name)
                .NotEmpty().WithMessage("Name is required")
                .Length(3, 50).WithMessage("Name must be between 3 and 50 characters.");

            RuleFor(u => u.UserIds)
                .NotEmpty().WithMessage("Member is required")
                .Must(u => u.Count >= 2 && u.Count <= 99)
                .WithMessage("Member count must be between 3 and 100.");



        }
    }
}
