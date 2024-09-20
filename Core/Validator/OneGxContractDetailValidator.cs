using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCDS.WebFuncions.Controller;
using WCDS.WebFuncions.Core.Model;
using WCDS.WebFuncions.Core.Model.ContractManagement;

namespace WCDS.WebFuncions.Core.Validator
{
    public class OneGxContractDetailValidator: AbstractValidator<OneGxContractDetailDto>
    {
        public OneGxContractDetailValidator()
        {
            RuleFor(x => x.ContractNumber).NotEmpty().WithMessage("Contract number must not be null or empty.");
            RuleFor(x => x.ContractWorkspace).NotEmpty().WithMessage("Contract workspace must not be null or empty.");
        }
    }
}
