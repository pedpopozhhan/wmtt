using System;
//using WCDS.WebFuncions.Core.Common.CAS;

namespace WCDS.WebFuncions.Core.Entity.CAS;
public class CASVendorContract : CASEntityBase
{
    public int ContractId { get; set; }
    public int VendorAddressId { get; set; }
    public int VendorLocationId { get; set; }
    public DateTime EffectiveDate { get; set; }

}