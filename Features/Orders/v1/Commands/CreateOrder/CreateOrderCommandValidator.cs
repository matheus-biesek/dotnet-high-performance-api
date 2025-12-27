using FluentValidation;

namespace DotNetHighPerformanceApi.Features.Orders.v1.Commands.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.Payload).NotNull();
        RuleFor(x => x.Payload.ProductIds)
            .NotEmpty().WithMessage("A lista de produtos não pode estar vazia.")
            .Must(ids => ids != null && ids.All(id => id > 0)).WithMessage("IDs de produtos inválidos.");
    }
}
