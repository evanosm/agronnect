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
    public async Task<IActionResult> Add(string Name)
    {
        var newService = new Service()
        {
            Name = Name
        };
        _context.Services.Add(newService);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Update(int Id, string Name)
    {
        var service = await _context.Services.FindAsync(Id);
        if (service == null)
        {
            return NotFound();
        }

        service.Name = Name;
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int Id)
    {
        var service = await _context.Services.FindAsync(Id);
        if (service == null)
        {
            return NotFound();
        }

        _context.Services.Remove(service);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Details(int Id)
    {
        var service = await _context.Services
            .Include(s => s.Employees)
            .ThenInclude(e => e.Site)
            .FirstOrDefaultAsync(s => s.ServiceId == Id);
        if (service == null)
        {
            return NotFound();
        }

        return View(service);
    }
}