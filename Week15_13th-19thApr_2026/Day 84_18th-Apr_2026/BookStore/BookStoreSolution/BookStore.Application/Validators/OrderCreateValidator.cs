using BookStore.Application.DTOs;
using FluentValidation;

namespace BookStore.Application.Validators;

public class OrderCreateValidator : AbstractValidator<OrderCreateDto>
{
    public OrderCreateValidator()
    {
        RuleFor(x => x.Items).NotEmpty().WithMessage("Order must contain at least one item.");
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.BookId).GreaterThan(0);
            item.RuleFor(i => i.Qty).GreaterThan(0).WithMessage("Quantity must be at least 1.");
        });
    }
}