using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Policy;
using Microsoft.EntityFrameworkCore;
using WCDS.WebFuncions.Core.Common.CAS;

namespace WCDS.WebFuncions.Core.Entity.CAS;
[Table("CAS_EMPLOYEE")]
// [PrimaryKey(nameof(EmployeeId))]
public class CASEmployee : ICASAudit
{
    [Key]
    public int EmployeeId { get; set; }
    public string? EmpNumber { get; set; }
    public string? Surname { get; set; }
    public string? GivenNames { get; set; }
    public DateTime CreateTimestamp { get; set; }
    public string CreateUserId { get; set; }
    public DateTime? UpdateTimestamp { get; set; }
    public string? UpdateUserId { get; set; }
}
