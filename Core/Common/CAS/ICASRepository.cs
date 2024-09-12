

using System.Collections.Generic;
using System.Threading.Tasks;
using WCDS.WebFuncions.Core.Entity.CAS;
using WCDS.WebFuncions.Core.Model.CAS;

namespace WCDS.WebFuncions.Core.Common.CAS;

public interface ICASRepository
{
    Task<List<CASContract>> GetLast10Contracts();
    Task<DbReturnValue> InsertContract(CASContract newContract);

}
