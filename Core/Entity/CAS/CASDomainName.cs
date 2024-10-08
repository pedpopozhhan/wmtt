using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using WCDS.WebFuncions.Core.Common.CAS;

namespace WCDS.WebFuncions.Core.Entity.CAS;

[Table("CAS_CONTRACT")]
// [PrimaryKey(nameof(DomainNameId))]
public class CASDomainName : CASEntityBase
{
    [Key]
    public int DomainNameId { get; set; }
    public string DomainName { get; set; }
    public string? Description { get; set; }
}