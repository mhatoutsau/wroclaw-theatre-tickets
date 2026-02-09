namespace WroclawTheatreTickets.Application.Validators;

using FluentValidation;
using WroclawTheatreTickets.Application.Contracts.Dtos;

public class CreateReviewValidator : AbstractValidator<CreateReviewRequest>
{
    public CreateReviewValidator()
    {
        RuleFor(x => x.ShowId)
            .NotEmpty().WithMessage("Show ID is required");

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5");

        RuleFor(x => x.Comment)
            .MaximumLength(1000).WithMessage("Comment must not exceed 1000 characters");
    }
}
