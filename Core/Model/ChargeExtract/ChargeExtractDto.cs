using System;
using System.Collections.Generic;

namespace WCDS.WebFuncions.Core.Model.ChargeExtract
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
        public Guid? ParentChargeExtractId { get; set; }
        public List<ChargeExtractDetailDto> ChargeExtractDetail { get; set; }
        public List<ChargeExtractViewLogDto> ChargeExtractViewLog { get; set; }
        public List<ChargeExtractDto> ExtendedExtract { get; set; }
        public string ExtractFile { get; set; }
        public List<ChargeExtractFileDto> ExtractFiles { get; set; }
    }
}
