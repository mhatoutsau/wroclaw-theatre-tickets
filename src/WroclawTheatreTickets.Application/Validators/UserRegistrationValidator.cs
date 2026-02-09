namespace WroclawTheatreTickets.Application.Validators;

using FluentValidation;
using WroclawTheatreTickets.Application.Contracts.Dtos;

public class UserRegistrationValidator : AbstractValidator<UserRegistrationRequest>
{
    public UserRegistrationValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email must be a valid email address");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters");

        RuleFor(x => x.FirstName)
            .MaximumLength(100).WithMessage("First name must not exceed 100 characters");

        RuleFor(x => x.LastName)
            .MaximumLength(100).WithMessage("Last name must not exceed 100 characters");
    }
}
