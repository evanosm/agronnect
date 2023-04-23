namespace AnnuaireCESI.Models;

public class Site
{
    public int SiteId { get; set; }
    public string City { get; set; }
    public ICollection<Employee> Employees { get; set; }
}