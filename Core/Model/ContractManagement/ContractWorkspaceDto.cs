using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCDS.WebFuncions.Core.Model.ContractManagement
{
    public class ContractWorkspacesDto
    {
        public string ContractWorkspace { get; set; }
        public DateTime Effectivedate { get; set; }
        public DateTime Origexpirationdate { get; set; }
        public DateTime CurrExpirationdate { get; set; }
        public string Amendmenttype { get; set; }
        public string Description { get; set; }
        public double CurrContractValue { get; set; }
        public string CurrencyType { get; set; }
        public string Status { get; set; }
        public string SolicitationType { get; set; }
        public string ContractType { get; set; }
        public List<CWSContractDto> Contracts { get; set; }
    }
}
