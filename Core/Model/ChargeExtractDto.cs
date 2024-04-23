using System;
using System.Collections.Generic;

namespace WCDS.WebFuncions.Core.Model
{
    public class ChargeExtractDto
    {
        public Guid ChargeExtractId { get; set; }
        public DateTime ChargeExtractDateTime { get; set; }
        public string ChargeExtractFileName { get; set; }
        public string RequestedBy { get; set; }
        public string VendorId { get; set; }
        public DateTime AuditCreationDateTime { get; set; }
        public string AuditLastUpdatedBy { get; set; }
        public DateTime AuditLastUpdatedDateTime { get; set; }
        public List<ChargeExtractDetailDto> ChargeExtractDetailDto { get; set; }
        public List<ChargeExtractViewLogDto> ChargeExtractViewLogDto { get; set; }
    }
}
