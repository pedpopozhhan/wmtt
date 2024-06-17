using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCDS.WebFuncions.Core.Model.FinanceDocument
{
    public class FinanceDocumentDto
    {
        public DateTime? LastChangeDate { get; set; }
        public DateTime? PostingDate { get; set; }
        public string CompanyCode { get; set; }
        public string AccountingDocumentType { get; set; }
        public string AccountingDocument { get; set; }
        public DateTime? DocumentDate { get; set; }
        public DateTime? AccountingDocumentCreationDate { get; set; }
        public string ExchangeRate { get; set; }
        public string FiscalPeriod { get; set; }
        public string LedgerGroup { get; set; }
        public string TransactionCode { get; set; }
        public string AccountingDocCreatedByUser { get; set; }
        public string ReverseDocument { get; set; }
        public string ReverseDocumentFiscalYear { get; set; }
        public string AccountingDocumentHeaderText { get; set; }
        public string Reference1InDocumentHeader { get; set; }
        public string Reference2InDocumentHeader { get; set; }
        public string FiscalYear { get; set; }
        public string DocumentReferenceID { get; set; }
        public string Ledger { get; set; }
        public string HouseBankAccount { get; set; }
        public string WithholdingTaxBaseAmount { get; set; }
        public string WithholdingTaxAmount { get; set; }
        public string ClearingAccountingDocument { get; set; }
        public string GLAccount { get; set; }
        public string HouseBank { get; set; }
        public string CostCenter { get; set; }
        public string Customer { get; set; }
        public string PaymentMethod { get; set; }
        public string PersonnelNumber { get; set; }
        public string PostingKey { get; set; }
        public string PaymentTerms { get; set; }
        public string Supplier { get; set; }
        public string PartnerCompany { get; set; }
        public string SpecialGLCode { get; set; }
        public string AmountInTransactionCurrency { get; set; }
        public string DebitCreditCode { get; set; }
        public string TaxCode { get; set; }
        public string OrderID { get; set; }
        public string ProfitCenter { get; set; }
        public string TaxJurisdiction { get; set; }
        public string FunctionalArea { get; set; }
        public string Fund { get; set; }
        public string FundsCenter { get; set; }
        public string PurchasingDocument { get; set; }
        public DateTime? DueCalculationBaseDate { get; set; }
        public string MasterFixedAsset { get; set; }
        public string FixedAsset { get; set; }
        public string AssignmentReference { get; set; }
        public string Reference1IDByBusinessPartner { get; set; }
        public string Reference2IDByBusinessPartner { get; set; }
        public string PartnerProfitCenter { get; set; }
        public DateTime? ClearingDate { get; set; }
        public string WithholdingTaxCode { get; set; }
        public string Reference3IDByBusinessPartner { get; set; }
        public string DocumentItemText { get; set; }
        public string AmountInCompanyCodeCurrency { get; set; }
        public string BaseUnit { get; set; }
        public string PaymentBlockingReason { get; set; }
        public string InvoiceReference { get; set; }
        public DateTime? NetDueDate { get; set; }
        public string PaymentMethodSupplement { get; set; }
        public string AccountingDocumentItemType { get; set; }
        public string InvoiceReferenceFiscalYear { get; set; }
        public string InvoiceItemReference { get; set; }
        public string CommitmentItem { get; set; }
        public string TaxAmount { get; set; }
        public string FundedProgram { get; set; }
    }
}
