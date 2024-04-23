using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCDS.WebFuncions.Core.Entity
{
    internal class ChargeExtractViewLog
    {
        [Key]
        public Guid ChargeExtractViewLogId { get; set; }
        public Guid ChargeExtractId { get; set; }
        public string ViewedBy { get; set; }
        public DateTime ViewedDateTime { get; set; }
        public ChargeExtract ChargeExtract { get; set; }
    }
}
