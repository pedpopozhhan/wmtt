using System;

namespace WCDS.WebFuncions.Core.Entity
{
    public class EntityBase
    {
        public string CreatedBy { get; set; }
        public DateTime? CreatedByDateTime { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedByDateTime { get; set; }
    }
}
