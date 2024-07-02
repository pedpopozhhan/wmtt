using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCDS.WebFuncions.Core.Model.ContractManagement
{
    public class CWSContractsResponseDto
    {
        public int Count { get; set; }
        public List<CWSContractDto> Data { get; set; }
    }
}
