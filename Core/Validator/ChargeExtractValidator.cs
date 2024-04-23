using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCDS.WebFuncions.Controller;
using WCDS.WebFuncions.Core.Model;

namespace WCDS.WebFuncions.Core.Validator
{
    public class ChargeExtractValidator : AbstractValidator<ChargeExtractDto>
    {
        IChargeExtractController _chargeExtractController;

        public ChargeExtractValidator(IChargeExtractController chargeExtractController)
        {
            _chargeExtractController = chargeExtractController;
            RuleFor(x => x.VendorId).NotEmpty().WithMessage("Vendor Id number must not be null or an empty string.");
        }
    }
}
