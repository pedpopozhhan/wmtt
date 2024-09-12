using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using WCDS.WebFuncions.Core.Common.CAS;

namespace WCDS.WebFuncions.Core.Entity.CAS;

[Table("CAS_CONTRACT")]
// [PrimaryKey(nameof(DomainNameId))]
public class CASDomainName : ICASAudit
{
    [Key]
    public int DomainNameId { get; set; }
    public string DomainName { get; set; }
    public string? Description { get; set; }
    public DateTime CreateTimestamp { get; set; }
    public string CreateUserId { get; set; }
    public DateTime? UpdateTimestamp { get; set; }
    public string? UpdateUserId { get; set; }
}