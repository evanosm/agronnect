using System.ComponentModel.DataAnnotations;

namespace AnnuaireCESI.Models;

public class AccessRequest
{
    [Key] public Guid RequestId { get; set; }
    public Employee Employee { get; set; }
    public string Details { get; set; }
}