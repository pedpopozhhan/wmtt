using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCDS.WebFuncions.Core.Model.CAS;
using WCDS.WebFuncions.Core.Model.ChargeExtract;
using WCDS.WebFuncions.Core.Services.CAS;

namespace WCDS.WebFuncions.Controller
{
    public interface ICASContractController
    {
        public Task<ContractUploadResponseDto> UploadContractToCAS(CASContractDto contractDto);
    }
    public class CASContractController : ICASContractController
    {
        ICASService _casService = null;

        public CASContractController(ICASService casService)
        {
            _casService = casService;
        }            

        public async Task<ContractUploadResponseDto> UploadContractToCAS(CASContractDto contractDto)
        {   
            var result = await _casService.UploadContract(contractDto);
            return result;
        }
    }
}
