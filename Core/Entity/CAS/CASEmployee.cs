using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using WCDS.WebFuncions.Core.Common.CAS;

namespace WCDS.WebFuncions.Core.Entity.CAS;
[Table("CAS_EMPLOYEE")]
// [PrimaryKey(nameof(EmployeeId))]
public class CASEmployee : CASEntityBase
{
    [Key]
    public int EmployeeId { get; set; }
    public string? EmpNumber { get; set; }
    public string? Surname { get; set; }
    public string? GivenNames { get; set; }
}
