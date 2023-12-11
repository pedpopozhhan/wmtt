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
            RuleFor(x => x.InvoiceID).NotEmpty().WithMessage("Please provide valid value for Invoice ID.");
            RuleFor(x => x.InvoiceID).Must(InvoiceIDExists).WithMessage("Invoice ID already exists.");
            RuleFor(x => x.DateOnInvoice).NotNull().WithMessage("Please provide value for Date On Invoice.");
            RuleFor(x => x.PeriodEndDate).NotNull().WithMessage("Please provide value for Period End Date.");
            RuleFor(x => x.InvoiceAmount).GreaterThan(0).WithMessage("Invoice Amount should be greater than Zero.");
            RuleFor(x => x.InvoiceReceivedDate).NotNull().WithMessage("Please provide value for Invoice Received Date.");
        }

        private bool InvoiceIDExists(string invoiceID)
        {
            return _invoiceController.InvoiceExists(invoiceID);
        }
    }
}
