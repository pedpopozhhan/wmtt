using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCDS.WebFuncions.Core.Model.ContractManagement
{
    public class ContractWorkspaceResponseDto
    {
        public int Count { get; set; }
        public List<ContractWorkspaceDto> Data { get; set; }
    }
}
