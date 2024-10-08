using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
//using WCDS.WebFuncions.Core.Common.CAS;
using WCDS.WebFuncions.Core.Entity.CAS;
using WCDS.WebFuncions.Core.Model.CAS;
namespace WCDS.WebFuncions.Core.Services.CAS;

public interface ICASService
{
    Task<ContractUploadResponseDto> UploadContract(CASContractDto contract);
}

public class CASService : ICASService
{
    public CASService(ICASRepository repository, IMapper mapper)
    {
        Repository = repository;
        Mapper = mapper;
    }

    private ICASRepository Repository { get; }
    private IMapper Mapper { get; }

    

    public async Task<ContractUploadResponseDto> UploadContract(CASContractDto contract)
    {
        ContractUploadResponseDto responseDto = new ContractUploadResponseDto();
        try
        {
            var existingCasContract = await Repository.GetCASContract(contract.ContractNumber);
            if (existingCasContract != null)
            {
                responseDto.Error = "Contract already exists.";
                responseDto.Success = false;
                responseDto.Contract = Mapper.Map<CASContract, CASContractDto>(existingCasContract);
            }
            else
            {
                responseDto = await InsertContract(contract);
            }
        }
        catch (Exception Ex)
        {
            responseDto.Success =false;
            responseDto.Error = Ex.Message;
        }
        return responseDto;
    }


    private async Task<ContractUploadResponseDto> InsertContract(CASContractDto contract)
    {
        ContractUploadResponseDto responseDto = new ContractUploadResponseDto();
        try
        {
            var vendorLocation = await Repository.GetVendorLocationId(contract.VendorId);
            var vendorAddress = await Repository.GetVendorAddress(contract.VendorId);
            var corportateRegion = Repository.GetCASCorporateRegion(contract.CorporateRegionName);

            if (vendorLocation == null)
            {
                Console.WriteLine($"{contract.VendorId} either does not exist in CAS_VENDOR, or the primary key is not in CAS_VENDOR_LOCATION");
                throw new Exception($"{contract.VendorId} either does not exist in CAS_VENDOR, or the primary key is not in CAS_VENDOR_LOCATION");
            }
            if (vendorAddress == null)
            {
                Console.WriteLine($"{contract.VendorId} either does not exist in CAS_VENDOR, or the primary key is not in CAS_VENDOR_ADDRESS");
                throw new Exception($"{contract.VendorId} either does not exist in CAS_VENDOR, or the primary key is not in CAS_VENDOR_ADDRESS");
            }

            if (corportateRegion == null)
            {
                Console.WriteLine($"{contract.CorporateRegionName} does not exist in cas_corporate_region");
                throw new Exception($"{contract.CorporateRegionName} does not exist in cas_corporate_region");
            }

            // A transaction would be best, but we are required to use stored procedures for each of the inserts.
            var casContract = Mapper.Map<CASContractDto, CASContract>(contract);
            casContract.CorporateRegionId = corportateRegion.Result.CorporateRegionId.Value;
            casContract.DomainCodeIdSelectionType = 9;
            casContract.DomainCodeIdCurrencyType = 35;            
            casContract.ContractTypeId = 14;
            casContract.CorporateServiceId = 172;
            casContract.EmployeeIdExpOfficer = 3055;

            var retVal = await Repository.InsertContract(casContract);

            var vendorContract = new CASVendorContract
            {
                ContractId = Convert.ToInt32(retVal.Id),
                EffectiveDate = contract.EffectiveDate.Value,
                VendorAddressId = vendorAddress.VENDOR_ADDRESS_ID,
                VendorLocationId = vendorLocation.VENDOR_LOCATION_ID
            };

            var result = await Repository.InsertVendorContract(vendorContract);
            responseDto.Success = true;
        }
        catch (Exception ex)
        {
            responseDto.Error = "An error has occured while Inserting Contract in CAS (InsertContract)" + contract.ContractNumber + "Error - " + ex.Message;
            responseDto.Success = false;
        }
        return responseDto;
    }
}
