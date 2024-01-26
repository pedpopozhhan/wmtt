﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WCDS.WebFuncions.Core.Context;

#nullable disable

namespace WCDS.WebFuncions.Migrations
{
    [DbContext(typeof(ApplicationDBContext))]
    partial class ApplicationDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.25")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("WCDS.WebFuncions.Core.Entity.Invoice", b =>
                {
                    b.Property<int>("InvoiceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("InvoiceId"), 1L, 1);

                    b.Property<string>("AssignedTo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ContractNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal?>("InvoiceAmount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime?>("InvoiceDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("InvoiceNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("InvoiceReceivedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("PeriodEndDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Vendor")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("InvoiceId");

                    b.ToTable("Invoice");
                });

            modelBuilder.Entity("WCDS.WebFuncions.Core.Entity.InvoiceOtherCostDetails", b =>
                {
                    b.Property<int>("InvoiceOtherCostDetailId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("InvoiceOtherCostDetailId"), 1L, 1);

                    b.Property<double>("Cost")
                        .HasColumnType("float");

                    b.Property<string>("CostCentre")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FireNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("From")
                        .HasColumnType("datetime2");

                    b.Property<string>("Fund")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("GlAccountNumber")
                        .HasColumnType("int");

                    b.Property<string>("InternalOrder")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("InvoiceId")
                        .HasColumnType("int");

                    b.Property<int>("NumberOfUnits")
                        .HasColumnType("int");

                    b.Property<string>("ProfitCentre")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("RatePerUnit")
                        .HasColumnType("float");

                    b.Property<string>("RateType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Remarks")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("To")
                        .HasColumnType("datetime2");

                    b.Property<string>("Unit")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("InvoiceOtherCostDetailId");

                    b.HasIndex("InvoiceId");

                    b.ToTable("InvoiceOtherCostDetails");
                });

            modelBuilder.Entity("WCDS.WebFuncions.Core.Entity.InvoiceServiceSheet", b =>
                {
                    b.Property<int>("InvoiceServiceSheetId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("InvoiceServiceSheetId"), 1L, 1);

                    b.Property<string>("AccountType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CommunityCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("InvoiceId")
                        .HasColumnType("int");

                    b.Property<string>("MaterialGroup")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("PurchaseGroup")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<string>("ServiceDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UniqueServiceSheetName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UnitOfMeasure")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("InvoiceServiceSheetId");

                    b.HasIndex("InvoiceId")
                        .IsUnique();

                    b.ToTable("InvoiceServiceSheet");
                });

            modelBuilder.Entity("WCDS.WebFuncions.Core.Entity.InvoiceTimeReportCostDetails", b =>
                {
                    b.Property<int>("InvoiceTimeReportCostDetailId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("InvoiceTimeReportCostDetailId"), 1L, 1);

                    b.Property<string>("AO02Number")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Cost")
                        .HasColumnType("float");

                    b.Property<string>("CostCentre")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("FireNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Fund")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("GlAccountNumber")
                        .HasColumnType("int");

                    b.Property<string>("InternalOrder")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("InvoiceId")
                        .HasColumnType("int");

                    b.Property<int>("NumberOfUnits")
                        .HasColumnType("int");

                    b.Property<string>("ProfitCentre")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("RatePerUnit")
                        .HasColumnType("float");

                    b.Property<string>("RateType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RateUnit")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RegistrationNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ReportNumber")
                        .HasColumnType("int");

                    b.Property<Guid>("TimeReportCostDetailReferenceId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("InvoiceTimeReportCostDetailId");

                    b.HasIndex("InvoiceId");

                    b.ToTable("InvoiceTimeReportCostDetails");
                });

            modelBuilder.Entity("WCDS.WebFuncions.Core.Entity.InvoiceOtherCostDetails", b =>
                {
                    b.HasOne("WCDS.WebFuncions.Core.Entity.Invoice", "Invoice")
                        .WithMany("InvoiceOtherCostDetails")
                        .HasForeignKey("InvoiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Invoice");
                });

            modelBuilder.Entity("WCDS.WebFuncions.Core.Entity.InvoiceServiceSheet", b =>
                {
                    b.HasOne("WCDS.WebFuncions.Core.Entity.Invoice", "Invoice")
                        .WithOne("InvoiceServiceSheet")
                        .HasForeignKey("WCDS.WebFuncions.Core.Entity.InvoiceServiceSheet", "InvoiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Invoice");
                });

            modelBuilder.Entity("WCDS.WebFuncions.Core.Entity.InvoiceTimeReportCostDetails", b =>
                {
                    b.HasOne("WCDS.WebFuncions.Core.Entity.Invoice", "Invoice")
                        .WithMany("InvoiceTimeReportCostDetails")
                        .HasForeignKey("InvoiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Invoice");
                });

            modelBuilder.Entity("WCDS.WebFuncions.Core.Entity.Invoice", b =>
                {
                    b.Navigation("InvoiceOtherCostDetails");

                    b.Navigation("InvoiceServiceSheet");

                    b.Navigation("InvoiceTimeReportCostDetails");
                });
#pragma warning restore 612, 618
        }
    }
}
