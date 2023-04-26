using System.Diagnostics;
using AnnuaireCESI.Data;
using Microsoft.AspNetCore.Mvc;
using AnnuaireCESI.Models;

namespace AnnuaireCESI.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AnnuaireDbContext _context;

    public HomeController(ILogger<HomeController> logger, AnnuaireDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {   
        var employeeCount = _context.Employees.Count();
        var servicesCount = _context.Services.Count();
        var sitesCount = _context.Sites.Count();
        
        var model = new HomeViewModel
        {
            EmployeesCount = employeeCount,
            ServicesCount = servicesCount,
            SitesCount = sitesCount
        };
        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}