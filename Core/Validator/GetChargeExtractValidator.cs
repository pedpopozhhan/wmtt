using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCDS.WebFuncions.Controller;
using WCDS.WebFuncions.Core.Model.ChargeExtract;

namespace WCDS.WebFuncions.Core.Validator
{
    public class GetChargeExtractValidator : AbstractValidator<ChargeExtractRequestDto>
    {
        IChargeExtractController _chargeExtractController;

        public GetChargeExtractValidator(IChargeExtractController chargeExtractController)
        {
            _chargeExtractController = chargeExtractController;
            
        }
    }
}
