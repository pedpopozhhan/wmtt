using System;

namespace WCDS.WebFuncions.Core.Model.CAS;

public class CASContractDto
{

    public int ContractId { get; set; }
    public int DomainCodeIdSelectionType { get; set; }
    public int DomainCodeIdCurrencyType { get; set; }
    public int? DomainCodeIdPdType { get; set; }
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

    public CASContractDto()
    {

        DomainCodeIdSelectionType = 9;
        DomainCodeIdCurrencyType = 35;
        CorporateRegionId = 121;
        ContractTypeId = 29;
        CorporateServiceId = 172;
        EmployeeIdExpOfficer = 3055;




    }

}
/*p_DOMAIN_CODE_ID_SELECt_TYPE				IN CAS_CONTRACT.DOMAIN_CODE_ID_SELECTION_TYPE%TYPE,
  p_DOMAIN_CODE_ID_CURR_TYPE				IN CAS_CONTRACT.DOMAIN_CODE_ID_CURRENCY_TYPE%TYPE,
  p_CONTRACT_TYPE_ID				IN CAS_CONTRACT.CONTRACT_TYPE_ID%TYPE,
  p_CORPORATE_SERVICE_ID				IN CAS_CONTRACT.CORPORATE_SERVICE_ID%TYPE,
  p_CORPORATE_REGION_ID				IN CAS_CONTRACT.CORPORATE_REGION_ID%TYPE,
  p_EMPLOYEE_ID_EXP_OFFICER				IN CAS_CONTRACT.EMPLOYEE_ID_EXP_OFFICER%TYPE,
  p_EMPLOYEE_ID_CERT_OFFICER				IN CAS_CONTRACT.EMPLOYEE_ID_CERT_OFFICER%TYPE,
  p_EMPLOYEE_ID_CONTACT_PERSON				IN CAS_CONTRACT.EMPLOYEE_ID_CONTACT_PERSON%TYPE,
  p_CONTRACT_NUMBER				IN CAS_CONTRACT.CONTRACT_NUMBER%TYPE,
  p_COMMENCEMENT_DATE				IN CAS_CONTRACT.COMMENCEMENT_DATE%TYPE,
  p_COMPLETION_DATE				IN CAS_CONTRACT.COMPLETION_DATE%TYPE,
  p_PROJECT_DESCRIPTION				IN CAS_CONTRACT.PROJECT_DESCRIPTION%TYPE,
  p_MAXIMUM_AMOUNT				IN CAS_CONTRACT.MAXIMUM_AMOUNT%TYPE,
  p_HOLDBACK_PERCENTAGE				IN CAS_CONTRACT.HOLDBACK_PERCENTAGE%TYPE,
  p_DOMAIN_CODE_ID_PD_TYPE         		IN CAS_CONTRACT.DOMAIN_CODE_ID_PD_TYPE%TYPE,
  p_WCB_ACCOUNT_NUMBER             		IN CAS_CONTRACT.WCB_ACCOUNT_NUMBER%TYPE,
  p_PERFORMANCE_DEPOSIT_AMOUNT			IN CAS_CONTRACT.PERFORMANCE_DEPOSIT_AMOUNT%TYPE,
  p_PD_RELEASE_OR_EXPIRY_DATE  			IN CAS_CONTRACT.PD_RELEASE_OR_EXPIRY_DATE%TYPE,
  p_CONTRACT_COMMENT             IN CAS_CONTRACT.CONTRACT_COMMENT%TYPE,
  p_CERT_OF_RECOG_NUMBER		IN CAS_CONTRACT.CERT_OF_RECOG_NUMBER%TYPE,
  p_CERT_OF_RECOG_EXPIRY_DATE	IN CAS_CONTRACT.CERT_OF_RECOG_EXPIRY_DATE%TYPE,
  id_rv					OUT CAS_CONTRACT.CONTRACT_ID%TYPE,
  create_timestamp_rv			OUT CAS_CONTRACT.create_timestamp%TYPE,
  create_userid_rv			OUT CAS_CONTRACT.create_userid%TYPE*/