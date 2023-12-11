using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCDS.WebFuncions.Core.Model
{
    public abstract class BaseDto
    {
        public DateTime AuditCreationDateTime { get; set; }
        public DateTime AuditLastUpdateDateTime { get; set; }
    }
}
