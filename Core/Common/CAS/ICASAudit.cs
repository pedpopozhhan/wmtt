using System;

namespace WCDS.WebFuncions.Core.Common.CAS;

public interface ICASAudit
{
    DateTime CreateTimestamp { get; set; }
    string CreateUserId { get; set; }
    DateTime? UpdateTimestamp { get; set; }
    string UpdateUserId { get; set; }
}


