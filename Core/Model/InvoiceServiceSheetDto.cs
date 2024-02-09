using System;
using System.ComponentModel.DataAnnotations;


namespace WCDS.WebFuncions.Core.Model
{
    public class InvoiceServiceSheetDto
    {
        public int InvoiceServiceSheetId { get; set; }
        public int InvoiceKey { get; set; }
        public string UniqueServiceSheetName { get; set; }
        public string PurchaseGroup { get; set; }
        public string ServiceDescription { get; set; }
        public string CommunityCode { get; set; }
        public string MaterialGroup { get; set; }
        public string AccountType { get; set; }
        public int Quantity { get; set; }
        public string UnitOfMeasure { get; set; }
        public decimal Price { get; set; }

    }
}
