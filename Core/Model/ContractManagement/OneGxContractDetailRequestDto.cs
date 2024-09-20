using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCDS.WebFuncions.Core.Model.ContractManagement
{
    public class OneGxContractDetailRequestDto
    {
        public Guid? OneGxContractId { get; set; }
        public string ContractWorkspace { get; set; }
        public string ContractNumber { get; set; }
        public string? ContractManager { get; set; }
        public Guid? CorporateRegion { get; set; }
        public string? PurchasingUnit { get; set; }
        public string? HoldbackAmount { get; set; }
        public string? RelatedContractId { get; set; }
    }
}
