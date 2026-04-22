using BookStore.Application.DTOs;
using FluentValidation;

namespace BookStore.Application.Validators;

public class BookCreateValidator : AbstractValidator<BookCreateDto>
{
    public BookCreateValidator()
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required.").MaximumLength(200);
        RuleFor(x => x.ISBN).NotEmpty().Matches(@"^[0-9\-]{10,17}$").WithMessage("Invalid ISBN format.");
        RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than zero.");
        RuleFor(x => x.Stock).GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative.");
        RuleFor(x => x.CategoryId).GreaterThan(0);
        RuleFor(x => x.AuthorId).GreaterThan(0);
        RuleFor(x => x.PublisherId).GreaterThan(0);
    }
}