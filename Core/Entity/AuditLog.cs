using System;
using System.ComponentModel.DataAnnotations;

namespace WCDS.WebFuncions.Core.Entity
{
    public class AuditLog
    {
        [Key]
        public Guid Id { get; set; }
        public string Operation { get; set; }
        public DateTime Timestamp { get; set; }
        public string User { get; set; }
        public string Info { get; set; }
    }
}
