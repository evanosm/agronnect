using System.ComponentModel.DataAnnotations;

namespace AnnuaireCESI.Models;

public class Site
{
    [Key] public int SiteId { get; set; }
    public string City { get; set; }
    public string Description { get; set; }
    public ICollection<Employee>? Employees { get; set; }
}