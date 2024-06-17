using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCDS.WebFuncions.Core.Model.FinanceDocument
{
    public class FinanceDocumentResponseDto
    {
        public int Count { get; set; }
        public List<FinanceDocumentDto> Data { get; set; }
    }
}
