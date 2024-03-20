using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using WCDS.WebFuncions.Core.Entity;

namespace WCDS.WebFuncions.Core.Model
{
    public class InvoiceDataSyncMessageDto
    {
        public DateTime TimeStamp { get; set; }
        public string Action { get; set; }
        public InvoiceDto Invoice { get; set; }

    }
}
