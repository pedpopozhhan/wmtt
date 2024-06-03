﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WCDS.WebFuncions.Core.Context;

#nullable disable

namespace WCDS.WebFuncions.Migrations
{
    [DbContext(typeof(ApplicationDBContext))]
    [Migration("20240603170226_AddedInvoiceStatus")]
    partial class AddedInvoiceStatus
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.25")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("WCDS.WebFuncions.Core.Entity.AuditLog", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Info")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Operation")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.Property<string>("User")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("AuditLog");
                });

            modelBuilder.Entity("WCDS.WebFuncions.Core.Entity.ChargeExtract", b =>
                {
                    b.Property<Guid>("ChargeExtractId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("AuditCreationDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("AuditLastUpdatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("AuditLastUpdatedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ChargeExtractDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("ChargeExtractFileName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("ParentChargeExtractId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("RequestedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("VendorId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ChargeExtractId");

                    b.ToTable("ChargeExtract");
                });

            modelBuilder.Entity("WCDS.WebFuncions.Core.Entity.ChargeExtractDetail", b =>
                {
                    b.Property<Guid>("ChargeExtractDetailId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("AuditCreationDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("AuditLastUpdatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("AuditLastUpdatedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("ChargeExtractId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("InvoiceId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("ChargeExtractDetailId");

                    b.HasIndex("ChargeExtractId");

                    b.ToTable("ChargeExtractDetail");
                });

            modelBuilder.Entity("WCDS.WebFuncions.Core.Entity.ChargeExtractViewLog", b =>
                {
                    b.Property<Guid>("ChargeExtractViewLogId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ChargeExtractId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ViewedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ViewedDateTime")
                        .HasColumnType("datetime2");

                    b.HasKey("ChargeExtractViewLogId");

                    b.HasIndex("ChargeExtractId");

                    b.ToTable("ChargeExtractViewLog");
                });

            modelBuilder.Entity("WCDS.WebFuncions.Core.Entity.Invoice", b =>
                {
                    b.Property<Guid>("InvoiceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ChargeExtractId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ContractNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedByDateTime")
                        .HasColumnType("datetime2");

                    b.Property<decimal?>("InvoiceAmount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime?>("InvoiceDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("InvoiceNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("InvoiceReceivedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("InvoiceStatus")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PaymentStatus")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("PeriodEndDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ServiceDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UniqueServiceSheetName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedByDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("VendorBusinessId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("VendorName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("InvoiceId");

                    b.HasIndex("ChargeExtractId");

                    b.ToTable("Invoice");
                });

            modelBuilder.Entity("WCDS.WebFuncions.Core.Entity.InvoiceOtherCostDetails", b =>
                {
                    b.Property<Guid>("InvoiceOtherCostDetailId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Account")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Cost")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("CostCentre")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedByDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("FireNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("From")
                        .HasColumnType("datetime2");

                    b.Property<string>("Fund")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("InternalOrder")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("InvoiceId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("NoOfUnits")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("ProfitCentre")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("RatePerUnit")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("RateType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RateUnit")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Remarks")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("To")
                        .HasColumnType("datetime2");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedByDateTime")
                        .HasColumnType("datetime2");

                    b.HasKey("InvoiceOtherCostDetailId");

                    b.HasIndex("InvoiceId");

                    b.ToTable("InvoiceOtherCostDetails");
                });

            modelBuilder.Entity("WCDS.WebFuncions.Core.Entity.InvoiceStatusLog", b =>
                {
                    b.Property<Guid>("StatusLogId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CurrentStatus")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("InvoiceId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("PreviousStatus")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.Property<string>("User")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("StatusLogId");

                    b.HasIndex("InvoiceId");

                    b.ToTable("InvoiceStatusLog");
                });

            modelBuilder.Entity("WCDS.WebFuncions.Core.Entity.InvoiceTimeReportCostDetails", b =>
                {
                    b.Property<Guid>("FlightReportCostDetailsId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Account")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Ao02Number")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ContractRegistrationName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Cost")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("CostCenter")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedByDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("FireNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FireYear")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("FlightReportDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("FlightReportId")
                        .HasColumnType("int");

                    b.Property<string>("Fund")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("InternalOrder")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("InvoiceId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("NoOfUnits")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("ProfitCenter")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("RatePerUnit")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("RateType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RateUnit")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedByDateTime")
                        .HasColumnType("datetime2");

                    b.HasKey("FlightReportCostDetailsId");

                    b.HasIndex("InvoiceId");

                    b.ToTable("InvoiceTimeReportCostDetails");
                });

            modelBuilder.Entity("WCDS.WebFuncions.Core.Entity.ChargeExtractDetail", b =>
                {
                    b.HasOne("WCDS.WebFuncions.Core.Entity.ChargeExtract", "ChargeExtract")
                        .WithMany("ChargeExtractDetail")
                        .HasForeignKey("ChargeExtractId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ChargeExtract");
                });

            modelBuilder.Entity("WCDS.WebFuncions.Core.Entity.ChargeExtractViewLog", b =>
                {
                    b.HasOne("WCDS.WebFuncions.Core.Entity.ChargeExtract", "ChargeExtract")
                        .WithMany("ChargeExtractViewLog")
                        .HasForeignKey("ChargeExtractId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ChargeExtract");
                });

            modelBuilder.Entity("WCDS.WebFuncions.Core.Entity.Invoice", b =>
                {
                    b.HasOne("WCDS.WebFuncions.Core.Entity.ChargeExtract", "ChargeExtract")
                        .WithMany("Invoice")
                        .HasForeignKey("ChargeExtractId");

                    b.Navigation("ChargeExtract");
                });

            modelBuilder.Entity("WCDS.WebFuncions.Core.Entity.InvoiceOtherCostDetails", b =>
                {
                    b.HasOne("WCDS.WebFuncions.Core.Entity.Invoice", null)
                        .WithMany("InvoiceOtherCostDetails")
                        .HasForeignKey("InvoiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("WCDS.WebFuncions.Core.Entity.InvoiceStatusLog", b =>
                {
                    b.HasOne("WCDS.WebFuncions.Core.Entity.Invoice", "Invoice")
                        .WithMany("InvoiceStatusLogs")
                        .HasForeignKey("InvoiceId")
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

            modelBuilder.Entity("WCDS.WebFuncions.Core.Entity.ChargeExtract", b =>
                {
                    b.Navigation("ChargeExtractDetail");

                    b.Navigation("ChargeExtractViewLog");

                    b.Navigation("Invoice");
                });

            modelBuilder.Entity("WCDS.WebFuncions.Core.Entity.Invoice", b =>
                {
                    b.Navigation("InvoiceOtherCostDetails");

                    b.Navigation("InvoiceStatusLogs");

                    b.Navigation("InvoiceTimeReportCostDetails");
                });
#pragma warning restore 612, 618
        }
    }
}
