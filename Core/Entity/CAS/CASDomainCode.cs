using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using WCDS.WebFuncions.Core.Common.CAS;

namespace WCDS.WebFuncions.Core.Entity.CAS;

[Table("CAS_CONTRACT")]
// [PrimaryKey(nameof(DomainCodeId))]
public class CASDomainCode : CASEntityBase
{
    [Key]
    public int DomainCodeId { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime? TerminationDate { get; set; }
}
