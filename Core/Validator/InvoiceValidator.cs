using FluentValidation;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCDS.WebFuncions.Controller;
using WCDS.WebFuncions.Core.Model;

namespace WCDS.WebFuncions.Core.Validator
{
    public class InvoiceValidator : AbstractValidator<InvoiceDto>
    {
        IInvoiceController _invoiceController;
        DateTime _earliestPossibleDateforInvoice = new DateTime(1950, 02, 01);

        int _maxRateAndNumberOfUnit = 99999;
        int _maxInvoiceAmount = 999999999;
        public InvoiceValidator(IInvoiceController invoiceController)
        {
            _invoiceController = invoiceController;
            RuleFor(x => x.InvoiceNumber).NotEmpty().WithMessage("Invoice number must not be null or an empty string.");
            RuleFor(x => x.InvoiceNumber).Matches(@"^[0-9a-zA-Z]+$").WithMessage("Invoice number must only be letters and numbers.");
            RuleFor(x => x.InvoiceId).Must(i => i.Equals(Guid.Empty)).WithMessage("Please provide valid value for Invoice ID.");
            RuleFor(x => new { x.InvoiceId, x.InvoiceNumber, x.ContractNumber }).Must(v => InvoiceNumberDoesNotExist(v.InvoiceId, v.InvoiceNumber, v.ContractNumber)).WithMessage("Invoice Number already exists for Contract Number.");
            RuleFor(x => x.InvoiceDate).NotNull().WithMessage("Please provide value for Invoice Date.");
            RuleFor(x => x.InvoiceDate).GreaterThan(_earliestPossibleDateforInvoice).WithMessage("Date cannot be 1950/02/01 or earlier.");
            RuleFor(x => new { x.InvoiceDate, x.PeriodEndDate }).Must(v => v.InvoiceDate >= v.PeriodEndDate).WithMessage("Invoice date Cannot be earlier than period ending date.");
            RuleFor(x => x.PeriodEndDate).NotNull().WithMessage("Please provide value for Period End Date.");
            RuleFor(x => x.InvoiceAmount).GreaterThan(0).WithMessage("Cannot invoice for $0.00");
            RuleFor(x => x.InvoiceAmount).LessThan(_maxInvoiceAmount).WithMessage("Cannot invoice over $999,999,999");
            RuleFor(x => x.InvoiceReceivedDate).NotNull().WithMessage("Please provide value for Invoice Received Date.");
            RuleFor(x => new { x.InvoiceTimeReportCostDetails, x.InvoiceOtherCostDetails }).Must(v => TimeReportOrOtherCostExists(v.InvoiceTimeReportCostDetails, v.InvoiceOtherCostDetails)).WithMessage("Invoice must have Time Report Costs or Other Costs");
            RuleFor(x => new { x.InvoiceOtherCostDetails }).Must(v => ValidateRateOfOtherCost(v.InvoiceOtherCostDetails)).WithMessage("Rate cannot be $0.00");
            RuleFor(x => new { x.InvoiceOtherCostDetails }).Must(v => ValidateMaxRateOfOtherCost(v.InvoiceOtherCostDetails)).WithMessage("Rate cannot exceed $99,999");
            RuleFor(x => new { x.InvoiceOtherCostDetails }).Must(v => ValidateNoOfUnitsOfOtherCost(v.InvoiceOtherCostDetails)).WithMessage("No. of units cannot be 0");
            RuleFor(x => new { x.InvoiceOtherCostDetails }).Must(v => ValidateMaxNoOfUnitsOfOtherCost(v.InvoiceOtherCostDetails)).WithMessage("No. of units cannot exceed 99,999");
        }

        private bool InvoiceNumberDoesNotExist(Guid? invoiceId, string invoiceNumber, string contractNumber)
        {

            return !_invoiceController.InvoiceExistsForContract(invoiceId, invoiceNumber, contractNumber);
        }
        private bool TimeReportOrOtherCostExists(List<InvoiceTimeReportCostDetailDto> invoiceTimeReportCostDetails, List<InvoiceOtherCostDetailDto> invoiceOtherCostDetails)
        {
            return (invoiceTimeReportCostDetails != null || invoiceOtherCostDetails != null);
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
