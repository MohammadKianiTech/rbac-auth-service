using FluentValidation;

namespace Evalify.Application.Users.Commands.Register;

public sealed class UserRegisterCommandValidator : AbstractValidator<UserRegisterCommand>
{
    public UserRegisterCommandValidator()
    {
        RuleFor(c => c.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Please enter a valid email address.");

        RuleFor(c => c.FirstName)
            .NotEmpty().WithMessage("FirstName is required")
            .MaximumLength(200);

        RuleFor(c => c.LastName)
            .NotEmpty().WithMessage("LastName is required")
            .MaximumLength(200);

        RuleFor(c => c.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
            .Matches(@"[A-Za-z]").WithMessage("Password must contain at least one letter.")
            .Matches(@"\d").WithMessage("Password must contain at least one digit.")
            .Matches(@"[^A-Za-z0-9]").WithMessage("Password must contain at least one special character.")
            .MaximumLength(100).WithMessage("Password is too long.");
    }
}