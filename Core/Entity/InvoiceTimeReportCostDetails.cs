﻿using System;
using System.ComponentModel.DataAnnotations;

namespace WCDS.WebFuncions.Core.Entity
{
    internal class InvoiceTimeReportCostDetails
    {
        [Key]
        public int InvoiceTimeReportCostDetailId { get; set; }
        public Guid TimeReportCostDetailReferenceId { get; set; }
        public int InvoiceId { get; set; }
        public DateTime Date { get; set; }
        public string RegistrationNumber { get; set; }
        public int ReportNumber { get; set; }
        public string AO02Number { get; set; }
        public string RateType { get; set; }
        public int NumberOfUnits { get; set; }
        public string RateUnit { get; set; }
        public double RatePerUnit { get; set; }
        public double Cost { get; set; }
        public int GlAccountNumber { get; set; }
        public string ProfitCentre { get; set; }
        public string CostCentre { get; set; }
        public string FireNumber { get; set; }
        public string InternalOrder { get; set; }
        public int Fund { get; set; }
        public Invoice Invoice { get; set; }
    }
}
