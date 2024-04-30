using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCDS.WebFuncions.Core.Model.ChargeExtract
{
    public class ChargeExtractViewLogDto
    {
        public Guid ChargeExtractViewLogId { get; set; }
        public Guid ChargeExtractId { get; set; }
        public string ViewedBy { get; set; }
        public DateTime ViewedDateTime { get; set; }
    }
}
