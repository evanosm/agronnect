using System.ComponentModel.DataAnnotations;
using AnnuaireCESI.Models;

namespace AnnuaireCESI.Models;

public class Employee
{
    [Key] public Guid EmployeeId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? Phone { get; set; }
    public string Mobile { get; set; }
    public string Email { get; set; }
    public string? Password { get; set; }
    public bool IsAdmin { get; set; } = false;
    public string? TempPassword { get; set; }
    public Service Service { get; set; }
    public Site Site { get; set; }
}