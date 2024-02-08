using System;
using System.ComponentModel.DataAnnotations;


namespace WCDS.WebFuncions.Core.Model
{
    public class UpdateServiceSheetDto
    {
        public int InvoiceKey { get; set; }
        public string UniqueServiceSheetName { get; set; }
    }
}
