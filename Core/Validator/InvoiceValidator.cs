﻿using FluentValidation;
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
        public InvoiceValidator(IInvoiceController invoiceController)
        {
            _invoiceController = invoiceController;
            RuleFor(x => x.InvoiceId).Must(i => i.Equals(Guid.Empty)).WithMessage("Please provide valid value for Invoice ID.");
            RuleFor(x => new { x.InvoiceNumber, x.ContractNumber }).Must(v => InvoiceNumberDoesNotExist(v.InvoiceNumber, v.ContractNumber)).WithMessage("Invoice Number already exists for Contract Number.");
            RuleFor(x => x.InvoiceDate).NotNull().WithMessage("Please provide value for Invoice Date.");            
            RuleFor(x => x.InvoiceDate).GreaterThan(_earliestPossibleDateforInvoice).WithMessage("Date cannot be 1950/02/01 or earlier.");
            RuleFor(x => new { x.InvoiceDate, x.PeriodEndDate }).Must(v => v.InvoiceDate >= v.PeriodEndDate).WithMessage("Invoice date Cannot be earlier than period ending date.");
            RuleFor(x => x.PeriodEndDate).NotNull().WithMessage("Please provide value for Period End Date.");
            RuleFor(x => x.InvoiceAmount).GreaterThan(0).WithMessage("Cannot invoice for $0.00");
            RuleFor(x => x.InvoiceReceivedDate).NotNull().WithMessage("Please provide value for Invoice Received Date.");
            RuleFor(x => new { x.InvoiceTimeReportCostDetails, x.InvoiceOtherCostDetails }).Must(v => TimeReportOrOtherCostExists(v.InvoiceTimeReportCostDetails, v.InvoiceOtherCostDetails)).WithMessage("Invoice must have Time Report Costs or Other Costs");
            RuleFor(x => new { x.InvoiceOtherCostDetails }).Must(v => ValidateRateOfOtherCost(v.InvoiceOtherCostDetails)).WithMessage("Rate cannot be $0.00");
            RuleFor(x => new { x.InvoiceOtherCostDetails }).Must(v => ValidateNoOfUnitsOfOtherCost(v.InvoiceOtherCostDetails)).WithMessage("No. of units cannot be 0");
        }

        private bool InvoiceNumberDoesNotExist(string invoiceNumber, string contractNumber)
        {
            return !_invoiceController.InvoiceExistsForContract(invoiceNumber, contractNumber);
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
                if (item.RatePerUnit <= 0)
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
                if (item.NoOfUnits <= 0)
                {
                    bResult = false;
                }
            }
            return bResult;
        }

    }
}
