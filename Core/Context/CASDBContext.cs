using System;
using System.Data;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using WCDS.WebFuncions.Core.Entity.CAS;

namespace WCDS.WebFuncions.Core.Context;

public class CASDBContext : DbContext
{
    public DbSet<CASContract> Contracts { get; set; }
    public DbSet<CASVendor> Vendors { get; set; }
    public DbSet<CASVendorAddress> VendorAddresses { get; set; }
    public DbSet<CASVendorLocation> VendorLocations { get; set; }
    //TODO: enable this logging if necessary
    //     public static readonly ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
    // {
    //     builder.AddConsole().AddFilter(DbLoggerCategory.Database.Command.Name, LogLevel.Trace);
    // });

    public CASDBContext(DbContextOptions<CASDBContext> options)
                 : base(options)
    {
    }
    public async Task<int> ExecuteStoredProcedure(string sql, OracleParameter[] parameters)
    {
        return await Database.ExecuteSqlRawAsync(sql, parameters);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //TODO: enable this logging if necessary
        // optionsBuilder.UseOracle(_connectionString).UseLoggerFactory(loggerFactory);

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        TransformContract(modelBuilder.Entity<CASContract>());
        TransformDomainCode(modelBuilder.Entity<CASDomainCode>());
        TransformDomainName(modelBuilder.Entity<CASDomainName>());
        TransformEmployee(modelBuilder.Entity<CASEmployee>());
        TransformVendorAddress(modelBuilder.Entity<CASVendorAddress>());
        TransformVendorLocation(modelBuilder.Entity<CASVendorLocation>());
        TransformVendor(modelBuilder.Entity<CASVendor>());
    }

    private void TransformContract(EntityTypeBuilder<CASContract> entity)
    {
        entity.ToTable("CAS_CONTRACT", "CAS");
        entity.HasKey(e => e.ContractId);
        entity.Property(e => e.ContractId)
                      .HasColumnName("CONTRACT_ID");
        entity.Property(e => e.DomainCodeIdSelectionType)
                      .HasColumnName("DOMAIN_CODE_ID_SELECTION_TYPE");
        entity.Property(e => e.DomainCodeIdCurrencyType)
                      .HasColumnName("DOMAIN_CODE_ID_CURRENCY_TYPE");
        entity.Property(e => e.DomainCodeIdPdType)
                      .HasColumnName("DOMAIN_CODE_ID_PD_TYPE");
        entity.Property(e => e.CorporateRegionId)
                      .HasColumnName("CORPORATE_REGION_ID");
        entity.Property(e => e.ContractTypeId)
                      .HasColumnName("CONTRACT_TYPE_ID");
        entity.Property(e => e.CorporateServiceId)
                      .HasColumnName("CORPORATE_SERVICE_ID");
        entity.Property(e => e.EmployeeIdExpOfficer)
                      .HasColumnName("EMPLOYEE_ID_EXP_OFFICER");
        entity.Property(e => e.EmployeeIdCertOfficer)
                      .HasColumnName("EMPLOYEE_ID_CERT_OFFICER");
        entity.Property(e => e.EmployeeIdContactPerson)
                      .HasColumnName("EMPLOYEE_ID_CONTACT_PERSON");
        entity.Property(e => e.ContractNumber)
                      .HasColumnName("CONTRACT_NUMBER")
                      .HasMaxLength(12)
                      .IsUnicode(false);
        entity.Property(e => e.CommencementDate)
                      .HasColumnName("COMMENCEMENT_DATE");
        entity.Property(e => e.CompletionDate)
                      .HasColumnName("COMPLETION_DATE");
        entity.Property(e => e.ContractComment)
                      .HasColumnName("CONTRACT_COMMENT")
                      .HasMaxLength(2000)
                      .IsUnicode(false);
        entity.Property(e => e.ProjectDescription)
                      .HasColumnName("PROJECT_DESCRIPTION")
                      .HasMaxLength(2000)
                      .IsUnicode(false);
        entity.Property(e => e.MaximumAmount)
                      .HasColumnName("MAXIMUM_AMOUNT").HasColumnType("decimal(12,2)");
        entity.Property(e => e.HoldbackPercentage)
                      .HasColumnName("HOLDBACK_PERCENTAGE").HasColumnType("decimal(5,2)");
        entity.Property(e => e.WcbAccountNumber)
                      .HasColumnName("WCB_ACCOUNT_NUMBER")
                      .HasMaxLength(8)
                      .IsUnicode(false);
        entity.Property(e => e.PerformanceDepositAmount)
                      .HasColumnName("PERFORMANCE_DEPOSIT_AMOUNT").HasColumnType("decimal(12,2)");
        entity.Property(e => e.PdReleaseOrExpiryDate)
                      .HasColumnName("PD_RELEASE_OR_EXPIRY_DATE");
        entity.Property(e => e.CreateTimestamp)
                      .HasColumnName("CREATE_TIMESTAMP");
        entity.Property(e => e.CreateUserId)
                      .HasColumnName("CREATE_USERID")
                      .HasMaxLength(30)
                      .IsUnicode(false);
        entity.Property(e => e.UpdateTimestamp)
                      .HasColumnName("UPDATE_TIMESTAMP");
        entity.Property(e => e.UpdateUserId)
                      .HasColumnName("UPDATE_USERID")
                      .HasMaxLength(30)
                      .IsUnicode(false);
        entity.Property(e => e.ReportExceptionFlag)
                      .HasColumnName("REPORT_EXCEPTION_FLAG")
                      .HasMaxLength(1)
                      .IsUnicode(false);
        entity.Property(e => e.CertOfRecogNumber)
                      .HasColumnName("CERT_OF_RECOG_NUMBER")
                      .HasMaxLength(20)
                      .IsUnicode(false);
        entity.Property(e => e.CertOfRecogExpiryDate)
                      .HasColumnName("CERT_OF_RECOG_EXPIRY_DATE");
    }

    private void TransformEmployee(EntityTypeBuilder<CASEmployee> entity)
    {
        entity.ToTable("CAS_EMPLOYEE", "CAS");
        entity.HasKey(e => e.EmployeeId);
        entity.Property(e => e.EmployeeId).HasColumnName("EMPLOYEE_ID");
        entity.Property(e => e.EmpNumber).HasColumnName("EMP_NUMBER");
        entity.Property(e => e.GivenNames).HasColumnName("GIVEN_NAMES");
        entity.Property(e => e.CreateTimestamp).HasColumnName("CREATE_TIMESTAMP");
        entity.Property(e => e.CreateUserId).HasColumnName("CREATE_USERID");
        entity.Property(e => e.UpdateTimestamp).HasColumnName("UPDATE_TIMESTAMP");
        entity.Property(e => e.UpdateUserId).HasColumnName("UPDATE_USERID");

    }

    private void TransformDomainCode(EntityTypeBuilder<CASDomainCode> entity)
    {
        entity.ToTable("CAS_DOMAIN_CODE", "CAS");
        entity.HasKey(e => e.DomainCodeId);
        entity.Property(e => e.DomainCodeId).HasColumnName("DOMAIN_CODE_ID");
        entity.Property(e => e.Description).HasColumnName("DESCRIPTION");
        entity.Property(e => e.Code).HasColumnName("CODE");
        entity.Property(e => e.EffectiveDate).HasColumnName("EFFECTIVE_DATE");
        entity.Property(e => e.TerminationDate).HasColumnName("TERMINATION_DATE");
        entity.Property(e => e.CreateTimestamp).HasColumnName("CREATE_TIMESTAMP");
        entity.Property(e => e.CreateUserId).HasColumnName("CREATE_USERID");
        entity.Property(e => e.UpdateTimestamp).HasColumnName("UPDATE_TIMESTAMP");
        entity.Property(e => e.UpdateUserId).HasColumnName("UPDATE_USERID");

    }

    private void TransformDomainName(EntityTypeBuilder<CASDomainName> entity)
    {
        entity.ToTable("CAS_DOMAIN_Name", "CAS");
        entity.HasKey(e => e.DomainNameId);
        entity.Property(e => e.DomainNameId).HasColumnName("DOMAIN_NAME_ID");
        entity.Property(e => e.DomainName).HasColumnName("DOMAIN_NAME");
        entity.Property(e => e.Description).HasColumnName("DESCRIPTION");
        entity.Property(e => e.CreateTimestamp).HasColumnName("CREATE_TIMESTAMP");
        entity.Property(e => e.CreateUserId).HasColumnName("CREATE_USERID");
        entity.Property(e => e.UpdateTimestamp).HasColumnName("UPDATE_TIMESTAMP");
        entity.Property(e => e.UpdateUserId).HasColumnName("UPDATE_USERID");

    }

    private void TransformVendorAddress(EntityTypeBuilder<CASVendorAddress> entity)
    {
        entity.ToTable("CAS_VENDOR_ADDRESS", "CAS");
        entity.HasKey(e => e.VENDOR_ADDRESS_ID);
        entity.Property(e => e.CreateTimestamp).HasColumnName("CREATE_TIMESTAMP");
        entity.Property(e => e.CreateUserId).HasColumnName("CREATE_USERID");
        entity.Property(e => e.UpdateTimestamp).HasColumnName("UPDATE_TIMESTAMP");
        entity.Property(e => e.UpdateUserId).HasColumnName("UPDATE_USERID");
    }

    private void TransformVendorLocation(EntityTypeBuilder<CASVendorLocation> entity)
    {
        entity.ToTable("CAS_VENDOR_LOCATION", "CAS");
        entity.HasKey(e => e.VENDOR_LOCATION_ID);
        entity.Property(e => e.CreateTimestamp).HasColumnName("CREATE_TIMESTAMP");
        entity.Property(e => e.CreateUserId).HasColumnName("CREATE_USERID");
        entity.Property(e => e.UpdateTimestamp).HasColumnName("UPDATE_TIMESTAMP");
        entity.Property(e => e.UpdateUserId).HasColumnName("UPDATE_USERID");
    }

    private void TransformVendor(EntityTypeBuilder<CASVendor> entity)
    {
        entity.ToTable("CAS_VENDOR", "CAS");
        entity.HasKey(e => e.VENDOR_PK_ID);
        entity.Property(e => e.CreateTimestamp).HasColumnName("CREATE_TIMESTAMP");
        entity.Property(e => e.CreateUserId).HasColumnName("CREATE_USERID");
        entity.Property(e => e.UpdateTimestamp).HasColumnName("UPDATE_TIMESTAMP");
        entity.Property(e => e.UpdateUserId).HasColumnName("UPDATE_USERID");
    }



}
