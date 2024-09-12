using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using WCDS.WebFuncions.Core.Common.CAS;
using WCDS.WebFuncions.Core.Entity.CAS;
using WCDS.WebFuncions.Core.Model.CAS;
namespace WCDS.WebFuncions.Core.Services.CAS;


public class CASService : ICASService
{
    public CASService(ICASRepository repository, IMapper mapper)
    {
        Repository = repository;
        Mapper = mapper;
    }

    private ICASRepository Repository { get; }
    private IMapper Mapper { get; }

    public async Task<List<CASContract>> GetLast10Contracts()
    {
        return await Repository.GetLast10Contracts();
    }

    public async Task<int> InsertContract(CASContractDto contract)
    {

        var casContract = Mapper.Map<CASContractDto, CASContract>(contract);
        var retVal = await Repository.InsertContract(casContract);


        return Convert.ToInt32(retVal.Id);
    }
}
