using FluentValidation;
using ToDo.Features.ToDos.DTO;

namespace ToDo.Features.ToDos.Validators
{
    public class ToDoCreateValidation : AbstractValidator<ToDoCreateDTO>
    {
        public ToDoCreateValidation()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
            RuleFor(x => x.DueDate)
                .GreaterThan(DateTime.Now).When(x => x.DueDate.HasValue)
                .WithMessage("DueDate must be later than right now");
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.")
                .GreaterThan(0).WithMessage("UserId must be a positive integer.");
        }
    }
    public class ToDoUpdateValidation : AbstractValidator<ToDoUpdateDTO>
    {
           public ToDoUpdateValidation()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
            RuleFor(x => x.DueDate)
                .GreaterThan(DateTime.Now).When(x => x.DueDate.HasValue)
                .WithMessage("DueDate must be later than right now");
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.")
                .GreaterThan(0).WithMessage("UserId must be a positive integer.");
        }
    }

}
