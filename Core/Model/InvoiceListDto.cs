using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCDS.WebFuncions.Core.Entity;
using WCDS.WebFuncions.Core.Model.ChargeExtract;

namespace WCDS.WebFuncions.Core.Model
{
    public class InvoiceListDto : BaseDto
    {
        public Guid? InvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal? InvoiceAmount { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime InvoiceReceivedDate { get; set; }
        public string InvoiceStatus { get; set; }
        public string VendorBusinessId { get; set; }
        public string VendorName { get; set; }
        public string UniqueServiceSheetName { get; set; }
        public int InvoiceAge
        {
            get
            {
                TimeSpan ts = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day) - this.InvoiceDate;
                return ts.Days;
            }
        }

    }
}
