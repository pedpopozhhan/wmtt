using System.Collections.Generic;
using System.Threading.Tasks;
using WCDS.WebFuncions.Core.Entity.CAS;
using WCDS.WebFuncions.Core.Model.CAS;

public interface ICASService
{
    Task<List<CASContract>> GetLast10Contracts();
    Task<int> InsertContract(CASContractDto contract);
}
