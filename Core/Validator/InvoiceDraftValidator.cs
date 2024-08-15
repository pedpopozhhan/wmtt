using FluentValidation;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCDS.WebFuncions.Controller;
using WCDS.WebFuncions.Core.Entity;
using WCDS.WebFuncions.Core.Model;

namespace WCDS.WebFuncions.Core.Validator
{
    public class InvoiceDraftValidator : AbstractValidator<InvoiceRequestDto>
    {
        IInvoiceController _invoiceController;
        DateTime _earliestPossibleDateforInvoice = new DateTime(1950, 02, 01);
        int _maxRateAndNumberOfUnit = 99999;
        int _maxInvoiceAmount = 999999999;
        public InvoiceDraftValidator(IInvoiceController invoiceController)
        {
            _invoiceController = invoiceController;
            RuleFor(x => x.InvoiceNumber).NotEmpty().WithMessage("Invoice number must not be null or an empty string.");
            RuleFor(x => x.InvoiceNumber).Matches(@"^[0-9a-zA-Z]+$").WithMessage("Invoice number must only be letters and numbers.");
            RuleFor(x => x.InvoiceId).Must(i => i.HasValue).WithMessage("Please provide valid value for Invoice ID.");
            RuleFor(x => x).Must(v => InvoiceNumberDoesNotExist(v)).WithMessage("Invoice Number already exists for Contract Number.");
            RuleFor(x => x.InvoiceDate).NotNull().WithMessage("Please provide value for Invoice Date.");
            RuleFor(x => x.InvoiceDate).GreaterThan(_earliestPossibleDateforInvoice).WithMessage("Date cannot be 1950/02/01 or earlier.");
            RuleFor(x => new { x.InvoiceDate, x.PeriodEndDate }).Must(v => v.InvoiceDate >= v.PeriodEndDate).WithMessage("Invoice date Cannot be earlier than period ending date.");
            RuleFor(x => x.PeriodEndDate).NotNull().WithMessage("Please provide value for Period End Date.");
            RuleFor(x => x.InvoiceAmount).GreaterThan(0).WithMessage("Cannot invoice for $0.00");
            RuleFor(x => x.InvoiceAmount).LessThan(_maxInvoiceAmount).WithMessage("Cannot invoice over $999,999,999");
            RuleFor(x => x.InvoiceReceivedDate).NotNull().WithMessage("Please provide value for Invoice Received Date.");
            RuleFor(x => new { x.InvoiceOtherCostDetails }).Must(v => v.InvoiceOtherCostDetails == null || ValidateRateOfOtherCost(v.InvoiceOtherCostDetails)).WithMessage("Rate cannot be $0.00");
            RuleFor(x => new { x.InvoiceOtherCostDetails }).Must(v => v.InvoiceOtherCostDetails == null || ValidateMaxRateOfOtherCost(v.InvoiceOtherCostDetails)).WithMessage("Rate cannot exceed $99,999");
            RuleFor(x => new { x.InvoiceOtherCostDetails }).Must(v => v.InvoiceOtherCostDetails == null || ValidateNoOfUnitsOfOtherCost(v.InvoiceOtherCostDetails)).WithMessage("No. of units cannot be 0");
            RuleFor(x => new { x.InvoiceOtherCostDetails }).Must(v => v.InvoiceOtherCostDetails == null || ValidateMaxNoOfUnitsOfOtherCost(v.InvoiceOtherCostDetails)).WithMessage("No. of units cannot exceed 99,999");
        }

        private bool InvoiceNumberDoesNotExist(InvoiceRequestDto request)
        {
            return !_invoiceController.InvoiceExistsForContract(request.InvoiceId, request.InvoiceNumber, request.ContractNumber);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoiceOtherCostDetails"></param>
        /// <returns></returns>
        private bool ValidateRateOfOtherCost(List<InvoiceOtherCostDetailDto> invoiceOtherCostDetails)
        {
            bool bResult = true;
            foreach (var item in invoiceOtherCostDetails)
            {
                if (item.RatePerUnit == 0)
                {
                    bResult = false;
                }
            }
            return bResult;
        }

        private bool ValidateMaxRateOfOtherCost(List<InvoiceOtherCostDetailDto> invoiceOtherCostDetails)
        {
            bool bResult = true;
            foreach (var item in invoiceOtherCostDetails)
            {
                if (item.RatePerUnit > _maxRateAndNumberOfUnit)
                {
                    bResult = false;
                }
            }
            return bResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoiceOtherCostDetails"></param>
        /// <returns></returns>
        private bool ValidateNoOfUnitsOfOtherCost(List<InvoiceOtherCostDetailDto> invoiceOtherCostDetails)
        {
            bool bResult = true;
            foreach (var item in invoiceOtherCostDetails)
            {
                if (item.NoOfUnits == 0)
                {
                    bResult = false;
                }
            }
            return bResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoiceOtherCostDetails"></param>
        /// <returns></returns>
        private bool ValidateMaxNoOfUnitsOfOtherCost(List<InvoiceOtherCostDetailDto> invoiceOtherCostDetails)
        {
            bool bResult = true;
            foreach (var item in invoiceOtherCostDetails)
            {
                if (item.NoOfUnits > _maxRateAndNumberOfUnit)
                {
                    bResult = false;
                }
            }
            return bResult;
        }
    }
}
