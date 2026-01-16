using FluentValidation;
using ToDo.Features.Users.DTO;
namespace ToDo.Features.Users.Validators
{
    public class UserCreateValidation : AbstractValidator<UserCreateDTO>
    {
        public UserCreateValidation()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required.")
                .MaximumLength(50).WithMessage("Username cannot exceed 50 characters.");
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email is required.")
                .MaximumLength(100).WithMessage("Email cannot exceed 100 characters.");
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("FullName is required.")
                .MaximumLength(100).WithMessage("FullName cannot exceed 100 characters.");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
            RuleFor(x => x.PasswordConfirm)
                .Equal(x => x.Password).WithMessage("Password confirmation does not match the password.");
            RuleFor(x => x.TwoFA)
                .NotNull().WithMessage("TwoFA status is required.");
            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Role is required.")
                .Must(role => role == "User" || role == "Admin")
                .WithMessage("Role must be either 'User' or 'Admin'.");
        }
    }
    public class UserUpdateValidation : AbstractValidator<UserCreateDTO>
    {
        public UserUpdateValidation()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required.")
                .MaximumLength(50).WithMessage("Username cannot exceed 50 characters.");
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email is required.")
                .MaximumLength(100).WithMessage("Email cannot exceed 100 characters.");
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("FullName is required.")
                .MaximumLength(100).WithMessage("FullName cannot exceed 100 characters.");
            RuleFor(x => x.TwoFA)
                .NotNull().WithMessage("TwoFA status is required.");
        }
    }   
}
