using System;

namespace WCDS.WebFuncions.Core.Model.CAS;

public class CASContractDto
{
    public int ContractId { get; set; }
    public DateTime? EffectiveDate { get; set; }
    public string? VendorId { get; set; }
    public int DomainCodeIdSelectionType { get; set; }
    public int DomainCodeIdCurrencyType { get; set; }
    public int? DomainCodeIdPdType { get; set; }
    public string CorporateRegionName { get; set; }
    public int CorporateRegionId { get; set; }
    public int ContractTypeId { get; set; }
    public int CorporateServiceId { get; set; }
    public int EmployeeIdExpOfficer { get; set; }
    public int? EmployeeIdCertOfficer { get; set; }
    public int? EmployeeIdContactPerson { get; set; }
    public string ContractNumber { get; set; }
    public DateTime CommencementDate { get; set; }
    public DateTime CompletionDate { get; set; }
    public string ContractComment { get; set; }
    public string ProjectDescription { get; set; }
    public decimal MaximumAmount { get; set; }
    public decimal? HoldbackPercentage { get; set; }
    public string WcbAccountNumber { get; set; }
    public decimal? PerformanceDepositAmount { get; set; }
    public DateTime? PdReleaseOrExpiryDate { get; set; }
    public string ReportExceptionFlag { get; set; }
    public string CertOfRecogNumber { get; set; }
    public DateTime? CertOfRecogExpiryDate { get; set; }   
}
