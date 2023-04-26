using AnnuaireCESI.Data;
using AnnuaireCESI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AnnuaireCESI.Controllers;

public class ServicesController : Controller
{
    private readonly AnnuaireDbContext _context;

    public ServicesController(AnnuaireDbContext context)
    {
        this._context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var services = await _context.Services.Include(s => s.Employees).ToListAsync();
        return View(services);
    }

    [HttpPost]
    public async Task<IActionResult> Index(string name)
    {
        var newService = new Service()
        {
            Name = name
        };
        _context.Services.Add(newService);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }
}