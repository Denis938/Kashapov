using FluentValidation;
using WebApplication1.Models.DTO;

namespace WebApplication1.Validators;

public class ProductCreateValidator : AbstractValidator<ProductCreateDto>
{
    public ProductCreateValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Название продукта обязательно")
            .MaximumLength(200).WithMessage("Название не должно превышать 200 символов");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Цена должна быть больше 0");

        RuleFor(x => x.Subject)
            .NotEmpty().WithMessage("Предмет обязателен")
            .MaximumLength(100).WithMessage("Название предмета не должно превышать 100 символов");

        RuleFor(x => x.Difficulty)
            .Must(d => d == "Easy" || d == "Medium" || d == "Hard")
            .WithMessage("Сложность должна быть: Easy, Medium или Hard");

        RuleFor(x => x.EstimatedHours)
            .GreaterThan(0).WithMessage("Оценка времени должна быть больше 0");
    }
}

