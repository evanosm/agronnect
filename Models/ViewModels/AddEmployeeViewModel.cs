namespace AnnuaireCESI.Models;

public class AddEmployeeViewModel
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? Phone { get; set; }
    public string Mobile { get; set; }
    public string Email { get; set; }
    public int SiteId { get; set; }
    public int ServiceId { get; set; }
    public List<Site> Sites { get; set; }
    public List<Service> Services { get; set; }
}