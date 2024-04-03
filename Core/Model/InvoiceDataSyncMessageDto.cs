﻿using Microsoft.EntityFrameworkCore.Metadata.Conventions;
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
        public InvoiceDataSyncMessageDetailDto Tables { get; set; }
    }
    public class InvoiceDataSyncMessageDetailDto
    { 
        public InvoiceDataSyncMessageDetailInvoiceDto Invoice { get; set; }
    }
    public class InvoiceDataSyncMessageDetailInvoiceDto
    {
        public Guid? InvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal? InvoiceAmount { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public DateTime? PeriodEndDate { get; set; }
        public DateTime? InvoiceReceivedDate { get; set; }
        public string PaymentStatus { get; set; }
        public string VendorBusinessId { get; set; }
        public string VendorName { get; set; }
        public string ContractNumber { get; set; }
        public string Type { get; set; }
        public string UniqueServiceSheetName { get; set; }
        public string ServiceDescription { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedByDateTime { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedByDateTime { get; set; }
        public InvoiceDataSyncMessageDetailCostDto Tables { get; set; }
    }
    public class InvoiceDataSyncMessageDetailCostDto
    {
        public List<InvoiceTimeReportCostDetailDto> InvoiceTimeReportCostDetails { get; set; }
        public List<InvoiceOtherCostDetailDto> InvoiceOtherCostDetails { get; set; }

    }


}