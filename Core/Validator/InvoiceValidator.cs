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
            RuleFor(x => x.InvoiceId).Must(i => i.Equals(Guid.Empty)).WithMessage("Please provide valid value for Invoice ID.");
            RuleFor(x => new { x.InvoiceNumber, x.ContractNumber }).Must(v => InvoiceNumberDoesNotExist(v.InvoiceNumber, v.ContractNumber)).WithMessage("Invoice Number already exists for Contract Number.");
            RuleFor(x => x.InvoiceDate).NotNull().WithMessage("Please provide value for Invoice Date.");
            RuleFor(x => x.PeriodEndDate).NotNull().WithMessage("Please provide value for Period End Date.");
            RuleFor(x => x.InvoiceAmount).GreaterThan(0).WithMessage("Cannot invoice for $0.00");
            RuleFor(x => x.InvoiceReceivedDate).NotNull().WithMessage("Please provide value for Invoice Received Date.");
            RuleFor(x => new { x.InvoiceTimeReportCostDetails, x.InvoiceOtherCostDetails }).Must(v => TimeReportOrOtherCostExists(v.InvoiceTimeReportCostDetails, v.InvoiceOtherCostDetails)).WithMessage("Invoice must have Time Report Costs or Other Costs");
        }

        private bool InvoiceNumberDoesNotExist(string invoiceNumber, string contractNumber)
        {
            return !_invoiceController.InvoiceExistsForContract(invoiceNumber, contractNumber);
        }
        private bool TimeReportOrOtherCostExists(List<InvoiceTimeReportCostDetailDto> invoiceTimeReportCostDetails, List<InvoiceOtherCostDetailDto> invoiceOtherCostDetails)
        {
            return (invoiceTimeReportCostDetails != null || invoiceOtherCostDetails != null);
        }
    }
}
