using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using WCDS.WebFuncions.Core.Common.CAS;

namespace WCDS.WebFuncions.Core.Entity.CAS;

[Table("CAS_CONTRACT")]
// [PrimaryKey(nameof(DomainCodeId))]
public class CASDomainCode : ICASAudit
{
    [Key]
    public int DomainCodeId { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime? TerminationDate { get; set; }
    public DateTime CreateTimestamp { get; set; }
    public string CreateUserId { get; set; }
    public DateTime? UpdateTimestamp { get; set; }
    public string? UpdateUserId { get; set; }

}
