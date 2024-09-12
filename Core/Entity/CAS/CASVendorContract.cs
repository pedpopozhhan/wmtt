using System;
using WCDS.WebFuncions.Core.Common.CAS;

namespace WCDS.WebFuncions.Core.Entity.CAS;
public class CASVendorContract : ICASAudit
{
    public DateTime CreateTimestamp { get; set; }
    public string CreateUserId { get; set; }
    public DateTime? UpdateTimestamp { get; set; }
    public string UpdateUserId { get; set; }
    public int ContractId { get; set; }
    public int VendorAddressId { get; set; }
    public int VendorLocationId { get; set; }
    public DateTime EffectiveDate { get; set; }

}