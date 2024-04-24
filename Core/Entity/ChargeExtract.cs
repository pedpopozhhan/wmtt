using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCDS.WebFuncions.Core.Entity
{
    internal class ChargeExtract
    {
        [Key]
        public Guid ChargeExtractId { get; set; }
        public DateTime ChargeExtractDateTime { get; set; }
        public string ChargeExtractFileName { get; set; }        
        public string RequestedBy { get; set; }        
        public string VendorId { get; set; }
        public DateTime AuditCreationDateTime { get; set; }
        public string AuditLastUpdatedBy { get; set; }
        public DateTime AuditLastUpdatedDateTime { get; set; }
        public List<ChargeExtractDetail> ChargeExtractDetail { get; set;}
        public List<ChargeExtractViewLog> ChargeExtractViewLog { get; set; }
        public List<Invoice> Invoice { get; set; }
        public Guid? ParentChargeExtractId { get; set; }
    }
}
