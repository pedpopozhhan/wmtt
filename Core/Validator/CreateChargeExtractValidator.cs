using FluentValidation;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCDS.WebFuncions.Controller;
using WCDS.WebFuncions.Core.Model.ChargeExtract;

namespace WCDS.WebFuncions.Core.Validator
{
    public class CreateChargeExtractValidator : AbstractValidator<CreateChargeExtractRequestDto>
    {
        IChargeExtractController _chargeExtractController;

        public CreateChargeExtractValidator(IChargeExtractController chargeExtractController)
        {
            _chargeExtractController = chargeExtractController;
            RuleFor(x => x.ChargeExtractDateTime).NotEmpty().WithMessage("Charge Extract datetime must not be null or empty.");
            RuleFor(x => x.Invoices.Count).GreaterThan(0).WithMessage("At least once invoice required for extract.");
            RuleFor(x =>new { x.Invoices, x.ContractNumber }).Must(v => InvoiceIsEligibleToBeExtracted(v.Invoices, v.ContractNumber)).WithMessage("One or more invoices have already been transferred.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoiceNumbers"></param>
        /// <param name="contractNumber"></param>
        /// <returns></returns>
        private bool InvoiceIsEligibleToBeExtracted(List<string> invoiceNumbers, string contractNumber)
        {
            bool bResult = true;
            foreach (var item in invoiceNumbers)
            {
                if (_chargeExtractController.InvoiceAlreadyExtracted(item, contractNumber))
                    bResult = false;
            }
            return bResult;
        }

    }
}
