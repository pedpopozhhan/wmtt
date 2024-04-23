using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCDS.WebFuncions.Core.Model;

namespace WCDS.WebFuncions.Controller
{
    public interface IChargeExtractController
    {
            public Task<Guid> CreateChargeExtract(ChargeExtractDto chargeExtract);            
    }
}
