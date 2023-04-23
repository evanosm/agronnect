using AnnuaireCESI.Models;

namespace AnnuaireCESI.Models;

public class Service
{
    public int ServiceId { get; set; }
    public string Name { get; set; }
    public ICollection<Employee> Employees { get; set; }
}