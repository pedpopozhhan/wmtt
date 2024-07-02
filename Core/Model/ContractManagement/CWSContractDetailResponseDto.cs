using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCDS.WebFuncions.Core.Model.ContractManagement
{
    public class CWSContractDetailResponseDto
    {
        public long ID { get; set; }
        public long ContractWorkspaceID { get; set; }
        public string ContractWorkspaceRef { get; set; }
        public string ContractNumber { get; set; }
        public string BusinessArea { get; set; }
        public string Userid { get; set; }
        public string Supplierid { get; set; }
        public string SupplierName { get; set; }
        public ContractWorkspaceDto Workspace { get; set; }
    }
}
