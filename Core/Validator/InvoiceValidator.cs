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
        public InvoiceValidator(IInvoiceController invoiceController)
        {
            _invoiceController = invoiceController;
            RuleFor(x => x.InvoiceNumber).NotEmpty().WithMessage("Please provide valid value for Invoice Number.");
            RuleFor(x => x.InvoiceNumber).Must(InvoiceNumberDoesNotExist).WithMessage("Invoice Number already exists.");
            RuleFor(x => x.InvoiceDate).NotNull().WithMessage("Please provide value for Invoice Date.");
            RuleFor(x => x.PeriodEndDate).NotNull().WithMessage("Please provide value for Period End Date.");
            RuleFor(x => x.InvoiceAmount).GreaterThan(0).WithMessage("Invoice Amount should be greater than Zero.");
            RuleFor(x => x.InvoiceReceivedDate).NotNull().WithMessage("Please provide value for Invoice Received Date.");
            RuleFor(x => new { x.InvoiceTimeReportCostDetails, x.InvoiceOtherCostDetails }).Must(v => TimeReportOrOtherCostExists(v.InvoiceTimeReportCostDetails, v.InvoiceOtherCostDetails)).WithMessage("Invoice must have Time Report Costs or Other Costs");
        }

        private bool InvoiceNumberDoesNotExist(string invoiceNumber)
        {
            return !_invoiceController.InvoiceExists(invoiceNumber);
        }
        private bool TimeReportOrOtherCostExists(List<InvoiceTimeReportCostDetailDto> invoiceTimeReportCostDetails, List<InvoiceOtherCostDetailDto> invoiceOtherCostDetails) 
        {
          return (invoiceTimeReportCostDetails != null || invoiceOtherCostDetails != null);
        }
    }
}
