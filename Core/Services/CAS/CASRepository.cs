using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using WCDS.WebFuncions.Core.Common.CAS;
using WCDS.WebFuncions.Core.Context;
using WCDS.WebFuncions.Core.Entity.CAS;
using WCDS.WebFuncions.Core.Model.CAS;

namespace WCDS.WebFuncions.Core.Services.CAS;

public class CASRepository : ICASRepository
{

    private readonly CASDBContext _context;

    public CASRepository(CASDBContext context)
    {
        _context = context;
    }

    public async Task<List<CASContract>> GetLast10Contracts()
    {
        var list = await _context.contracts.OrderByDescending(x => x.CreateTimestamp).Take(10).ToListAsync<CASContract>();
        return list;
    }



    public async Task<DbReturnValue> InsertContract(CASContract newContract)
    {
        var idRv = new OracleParameter("id_rv", OracleDbType.Decimal, ParameterDirection.Output);
        var createTimestampRv = new OracleParameter("create_timestamp_rv", OracleDbType.Date, ParameterDirection.Output);
        var createUserIdRv = new OracleParameter("create_userid_rv", OracleDbType.Varchar2, ParameterDirection.Output)
        {
            Size = 30
        };



        OracleParameter[] parameters ={
        new("p_DOMAIN_CODE_ID_SELECT_TYPE", OracleDbType.Int32, newContract.DomainCodeIdSelectionType, ParameterDirection.Input),
        new("p_DOMAIN_CODE_ID_CURR_TYPE", OracleDbType.Int32, newContract.DomainCodeIdCurrencyType, ParameterDirection.Input),
        new("p_CONTRACT_TYPE_ID", OracleDbType.Int32, newContract.ContractTypeId, ParameterDirection.Input),
         new("p_CORPORATE_SERVICE_ID", OracleDbType.Int32, newContract.CorporateServiceId, ParameterDirection.Input),
         new("p_CORPORATE_REGION_ID", OracleDbType.Int32, newContract.CorporateRegionId, ParameterDirection.Input),
         new("p_EMPLOYEE_ID_EXP_OFFICER", OracleDbType.Int32, newContract.EmployeeIdExpOfficer, ParameterDirection.Input),
         new("p_EMPLOYEE_ID_CERT_OFFICER", OracleDbType.Int32, newContract.EmployeeIdCertOfficer.HasValue ? newContract.EmployeeIdCertOfficer.Value : DBNull.Value, ParameterDirection.Input)
        {
            IsNullable = true
        },
         new("p_EMPLOYEE_ID_CONTACT_PERSON", OracleDbType.Int32, newContract.EmployeeIdContactPerson.HasValue ? newContract.EmployeeIdContactPerson.Value : DBNull.Value, ParameterDirection.Input)
        {
            IsNullable = true
        },
         new("p_CONTRACT_NUMBER", OracleDbType.Varchar2, 12, newContract.ContractNumber.ToUpper(), ParameterDirection.Input),
         new("p_COMMENCEMENT_DATE", OracleDbType.Date, newContract.CommencementDate, ParameterDirection.Input),
         new("p_COMPLETION_DATE", OracleDbType.Date, newContract.CompletionDate, ParameterDirection.Input),
         new("p_PROJECT_DESCRIPTION", OracleDbType.Varchar2, 2000, newContract.ProjectDescription, ParameterDirection.Input),
         new("p_MAXIMUM_AMOUNT", OracleDbType.Decimal, newContract.MaximumAmount, ParameterDirection.Input)
        {
            Precision = 12,
            Scale = 2
        },
         new("p_HOLDBACK_PERCENTAGE", OracleDbType.Decimal, newContract.HoldbackPercentage.HasValue ? newContract.HoldbackPercentage.Value : DBNull.Value, ParameterDirection.Input)
        {
            IsNullable = true,
            Precision = 5,
            Scale = 2
        },
         new("p_DOMAIN_CODE_ID_PD_TYPE", OracleDbType.Int32, newContract.DomainCodeIdPdType.HasValue ? newContract.DomainCodeIdPdType.Value : DBNull.Value, ParameterDirection.Input)
        {
            IsNullable = true
        },
         new("p_WCB_ACCOUNT_NUMBER", OracleDbType.Varchar2, 8, newContract.WcbAccountNumber, ParameterDirection.Input),
         new("p_PERFORMANCE_DEPOSIT_AMOUNT", OracleDbType.Decimal, newContract.PerformanceDepositAmount.HasValue ? newContract.PerformanceDepositAmount.Value : DBNull.Value, ParameterDirection.Input)
        {
            IsNullable = true,
            Precision = 12,
            Scale = 2
        },
         new("p_PD_RELEASE_OR_EXPIRY_DATE", OracleDbType.Date, newContract.PdReleaseOrExpiryDate.HasValue ? newContract.PdReleaseOrExpiryDate.Value : DBNull.Value, ParameterDirection.Input)
        {
            IsNullable = true
        },
        new("p_CONTRACT_COMMENT", OracleDbType.Varchar2, 2000, newContract.ContractComment, ParameterDirection.Input),
         new("p_CERT_OF_RECOG_NUMBER", OracleDbType.Varchar2, 20, newContract.CertOfRecogNumber, ParameterDirection.Input),
         new("p_CERT_OF_RECOG_EXPIRY_DATE", OracleDbType.Date, newContract.CertOfRecogExpiryDate.HasValue ? newContract.CertOfRecogExpiryDate.Value : DBNull.Value, ParameterDirection.Input)
        {
            IsNullable = true
        },
        idRv,
        createTimestampRv,
        createUserIdRv};


        var result = await _context.ExecuteStoredProcedure(@"BEGIN CAS.CAS_CONTRACT_pkg.ins_CAS_CONTRACT(
            :p_DOMAIN_CODE_ID_SELECT_TYPE,
            :p_DOMAIN_CODE_ID_CURR_TYPE,
            :p_CONTRACT_TYPE_ID,
            :p_CORPORATE_SERVICE_ID,
            :p_CORPORATE_REGION_ID,
            :p_EMPLOYEE_ID_EXP_OFFICER,
            :p_EMPLOYEE_ID_CERT_OFFICER,
            :p_EMPLOYEE_ID_CONTACT_PERSON,
            :p_CONTRACT_NUMBER,
            :p_COMMENCEMENT_DATE,
            :p_COMPLETION_DATE,
            :p_PROJECT_DESCRIPTION,
            :p_MAXIMUM_AMOUNT,
            :p_HOLDBACK_PERCENTAGE,
            :p_DOMAIN_CODE_ID_PD_TYPE,
            :p_WCB_ACCOUNT_NUMBER,
            :p_PERFORMANCE_DEPOSIT_AMOUNT,
            :p_PD_RELEASE_OR_EXPIRY_DATE,
            :p_CONTRACT_COMMENT,
            :p_CERT_OF_RECOG_NUMBER,
            :p_CERT_OF_RECOG_EXPIRY_DATE,
            :id_rv,
            :create_timestamp_rv,
            :create_userid_rv); END;", parameters);


        Console.WriteLine($"updated {result} rows ");

        var ret = new DbReturnValue
        {
            CreateUserId = createUserIdRv.ToString()
        };
        if (idRv.Value is OracleDecimal oracleDecimalOutput)
        {
            ret.Id = oracleDecimalOutput.Value;
        }
        if (createTimestampRv.Value is OracleDate oracleDateOutput)
        {
            ret.CreateTimestamp = oracleDateOutput.Value;
        }
        return ret;
    }

    public async Task<CASVendorLocation> GetVendorLocationId(string vendorId)
    {
        // select va.vendor_address_id, vl.vendor_location_id from cas.cas_vendor v inner join cas.cas_vendor_address va on va.vendor_pk_id=v.vendor_pk_id inner join cas.cas_vendor_location vl on vl.vendor_pk_id=v.vendor_pk_id where v.vendor_id like '%20042213%'
        var result = await (
        from v in _context.vendors
        join vl in _context.vendorLocations on v.VENDOR_PK_ID equals vl.VENDOR_PK_ID
        where v.VENDOR_ID.Contains(vendorId)
        select vl).FirstOrDefaultAsync();

        return result;
    }

    public async Task<CASVendorAddress> GetVendorAddress(string vendorId)
    {
        var result = await (
        from v in _context.vendors
        join va in _context.vendorAddresses on v.VENDOR_PK_ID equals va.VENDOR_PK_ID
        where v.VENDOR_ID.Contains(vendorId)
        select va).FirstOrDefaultAsync();

        return result;
    }

    public async Task<DbReturnValue> InsertVendorContract(CASVendorContract vendorContract)
    {
        var idRv = new OracleParameter("id_rv", OracleDbType.Decimal, ParameterDirection.Output);
        var createTimestampRv = new OracleParameter("create_timestamp_rv", OracleDbType.Date, ParameterDirection.Output);
        var createUserIdRv = new OracleParameter("create_userid_rv", OracleDbType.Varchar2, ParameterDirection.Output)
        {
            Size = 30
        };


        /*PROCEDURE ins_cas_contract_vendor
     (p_CONTRACT_ID            IN cas_contract_vendor.CONTRACT_ID%TYPE,
      p_VENDOR_ADDRESS_ID      IN cas_contract_vendor.VENDOR_ADDRESS_ID%TYPE,
      p_VENDOR_LOCATION_ID     IN cas_contract_vendor.VENDOR_LOCATION_ID%TYPE,
      p_EFFECTIVE_DATE         IN cas_contract_vendor.EFFECTIVE_DATE%TYPE,
      id_rv                    OUT cas_contract_vendor.CONTRACT_VENDOR_ID%TYPE,
      create_timestamp_rv      OUT cas_contract_vendor.create_timestamp%TYPE,
      create_userid_rv         OUT cas_contract_vendor.create_userid%TYPE
     )*/
        OracleParameter[] parameters ={
        new("p_CONTRACT_ID", OracleDbType.Int32, vendorContract.ContractId, ParameterDirection.Input),
        new("p_VENDOR_ADDRESS_ID", OracleDbType.Int32, vendorContract.VendorAddressId, ParameterDirection.Input),
        new("p_VENDOR_LOCATION_ID", OracleDbType.Int32, vendorContract.VendorLocationId, ParameterDirection.Input),
         new("p_EFFECTIVE_DATE", OracleDbType.Date, vendorContract.EffectiveDate, ParameterDirection.Input),
        idRv,
        createTimestampRv,
        createUserIdRv};

        var result = await _context.ExecuteStoredProcedure(@"BEGIN CAS.CAS_CONTRACT_VENDOR_pkg.ins_cas_contract_vendor(
            :p_CONTRACT_ID,
            :p_VENDOR_ADDRESS_ID,
            :p_VENDOR_LOCATION_ID,
            :p_EFFECTIVE_DATE,
            :id_rv,
            :create_timestamp_rv,
            :create_userid_rv); END;", parameters);



        Console.WriteLine($"updated {result} rows ");
        var ret = new DbReturnValue
        {
            CreateUserId = createUserIdRv.ToString()
        };
        if (idRv.Value is OracleDecimal oracleDecimalOutput)
        {
            ret.Id = oracleDecimalOutput.Value;
        }
        if (createTimestampRv.Value is OracleDate oracleDateOutput)
        {
            ret.CreateTimestamp = oracleDateOutput.Value;
        }
        return ret;
    }

}