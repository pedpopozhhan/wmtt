using FluentValidation;
using WCDS.WebFuncions.Core.Model;

namespace WCDS.WebFuncions.Core.Validator
{
    public class InvoiceByKeyValidator: AbstractValidator<InvoiceByKeyRequestDto>
    {
        public InvoiceByKeyValidator()
        {
            RuleFor(x => x.InvoiceKey).GreaterThan(0).WithMessage("InvoiceKey is not valid.");
        }
    }
}
