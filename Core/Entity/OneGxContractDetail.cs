using System;
using System.ComponentModel.DataAnnotations;

namespace WCDS.WebFuncions.Core.Entity
{
    public class OneGxContractDetail : EntityBase
    {
        [Key]
        public Guid OneGxContractId { get; set; }
        public string ContractWorkspace { get;set; }
        public string ContractNumber { get; set; }
        public string? ContractManager { get; set; }
        public Guid? CorporateRegion { get; set; }
        public string? PurchasingUnit { get; set; }
        public string? HoldbackAmount { get; set; }
        public string? RelatedContractId { get; set; }
    }
}
