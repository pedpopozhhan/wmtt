using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCDS.WebFuncions.Core.Entity.CAS
{
    public class CASCorporateRegion : CASEntityBase
    {
        public int? CorporateRegionId { get; set; }
        public int? ParentCorporateRegionId { get; set; }
        public int? DomainCodeIdType { get; set; }
        public string? CorporateRegionName { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? TerminationDate { get; set; }        
    }
}
