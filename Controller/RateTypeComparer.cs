using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using WCDS.WebFuncions.Core.Model.Services;

namespace WCDS.WebFuncions.Controller
{
    internal class RateTypeComparer : IEqualityComparer<RateType>
    {
        public bool Equals(RateType x, RateType y)
        {
            return x.Type == y.Type;
        }

        public int GetHashCode([DisallowNull] RateType obj)
        {
            return obj.RateTypeId.GetHashCode();
        }
    }
}
