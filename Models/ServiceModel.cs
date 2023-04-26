using System.ComponentModel.DataAnnotations;

namespace AnnuaireCESI.Models;

public class Service
{
    [Key] public int ServiceId { get; set; }
    public string Name { get; set; }
    public ICollection<Employee>? Employees { get; set; }
}