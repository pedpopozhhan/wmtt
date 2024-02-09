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
        public virtual DbSet<InvoiceTimeReportCostDetails> InvoiceTimeReportCostDetails { get; set; }
        public virtual DbSet<InvoiceOtherCostDetails> InvoiceOtherCostDetails { get; set; }
        public virtual DbSet<InvoiceServiceSheet> InvoiceServiceSheet { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("connectionstring"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Invoice>().HasMany(i => i.InvoiceTimeReportCostDetails).WithOne(i => i.Invoice).HasForeignKey(i => i.InvoiceKey);
            modelBuilder.Entity<Invoice>().HasMany(i => i.InvoiceOtherCostDetails).WithOne(i => i.Invoice).HasForeignKey(i => i.InvoiceKey);
            modelBuilder.Entity<Invoice>().HasOne(i => i.InvoiceServiceSheet).WithOne(i => i.Invoice).HasForeignKey<InvoiceServiceSheet>(i => i.InvoiceKey);
        }
    }
}
