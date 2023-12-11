using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols;
using System;
using WCDS.WebFuncions.Core.Entity;

namespace WCDS.WebFuncions.Core.Context
{
    internal class ApplicationDBContext : DbContext
    {
        public virtual DbSet<Invoice> Invoice { get; set; }
        public virtual DbSet<InvoiceTimeReport> InvoiceTimeReport { get; set; }
        public virtual DbSet<InvoiceDetail> InvoiceDetail { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("connectionstring"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<InvoiceTimeReport>()
                .HasOne(invoiceTimeReport => invoiceTimeReport.Invoice)
                .WithMany(Invoice => Invoice.TimeReports)
                .HasForeignKey(invoiceTimeReport => invoiceTimeReport.InvoiceKey);

            modelBuilder.Entity<InvoiceDetail>()
                .HasOne(invoiceDetail => invoiceDetail.TimeReport)
                .WithMany(InvoiceTimeReport => InvoiceTimeReport.InvoiceDetails)
                .HasForeignKey(invoiceDetail => invoiceDetail.InvoiceTimeReportKey);
        }
    }
}
