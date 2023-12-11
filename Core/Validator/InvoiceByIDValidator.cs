using FluentValidation;
using WCDS.WebFuncions.Core.Model;

namespace WCDS.WebFuncions.Core.Validator
{
    public class InvoiceByIDValidator : AbstractValidator<InvoiceByIDRequestDto>
    {
        public InvoiceByIDValidator()
        {
            RuleFor(x => x.InvoiceID).NotEmpty().WithMessage("Invoice ID should not be null or empty string.");
        }
    }
}
