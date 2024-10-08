using System;

namespace WCDS.WebFuncions.Core.Entity.CAS
{
    public class CASEntityBase
    {
        public DateTime CreateTimestamp { get; set; }
        public string CreateUserId { get; set; }
        public DateTime? UpdateTimestamp { get; set; }
        public string UpdateUserId { get; set; }
    }
}
