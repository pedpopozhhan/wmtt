using Microsoft.EntityFrameworkCore;
using System;
using WCDS.WebFuncions.Core.Entity;

namespace WCDS.WebFuncions.Core.Context
{
    public class ApplicationDBContext : DbContext
    {
        public virtual DbSet<AuditLog> AuditLog { get; set; }
        public virtual DbSet<Invoice> Invoice { get; set; }
        public virtual DbSet<InvoiceTimeReportCostDetails> InvoiceTimeReportCostDetails { get; set; }
        public virtual DbSet<InvoiceOtherCostDetails> InvoiceOtherCostDetails { get; set; }
        public virtual DbSet<InvoiceStatusLog> InvoiceStatusLog { get; set; }
        public virtual DbSet<ChargeExtract> ChargeExtract { get; set; }
        public virtual DbSet<ChargeExtractDetail> ChargeExtractDetail { get; set; }
        public virtual DbSet<ChargeExtractViewLog> ChargeExtractViewLog { get; set; }
        public virtual DbSet<InvoiceTimeReports> InvoiceTimeReports { get; set; }

        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
             : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Invoice>().HasMany(i => i.InvoiceTimeReportCostDetails).WithOne(i => i.Invoice).HasForeignKey(i => i.InvoiceId);
            modelBuilder.Entity<Invoice>().HasMany(i => i.InvoiceTimeReports).WithOne(i => i.Invoice).HasForeignKey(i => i.InvoiceId);
            modelBuilder.Entity<Invoice>().HasMany(i => i.InvoiceStatusLogs).WithOne(i => i.Invoice).HasForeignKey(i => i.InvoiceId);

            modelBuilder.Entity<ChargeExtract>().HasMany(i => i.ChargeExtractDetail).WithOne(i => i.ChargeExtract).HasForeignKey(i => i.ChargeExtractId);
            modelBuilder.Entity<ChargeExtract>().HasMany(i => i.ChargeExtractViewLog).WithOne(i => i.ChargeExtract).HasForeignKey(i => i.ChargeExtractId);
            modelBuilder.Entity<ChargeExtract>().HasMany(i => i.Invoice).WithOne(i => i.ChargeExtract).HasForeignKey(i => i.ChargeExtractId);
        }
    }
}
