using AnnuaireCESI.Data;
using AnnuaireCESI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AnnuaireCESI.Controllers;

public class SitesController : Controller
{
    private readonly AnnuaireDbContext _context;

    public SitesController(AnnuaireDbContext context)
    {
        this._context = context;
    }

    public async Task<IActionResult> Index()
    {
        var sites = await _context.Sites.Include(s => s.Employees).ToListAsync();
        return View(sites);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Add(string City, string Description)
    {
        var newSite = new Site()
        {
            City = City,
            Description = Description
        };
        _context.Sites.Add(newSite);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Update(int Id, string City, string Description)
    {
        Console.WriteLine(City, Description);
        var site = await _context.Sites.FindAsync(Id);
        if (site == null)
        {
            return NotFound();
        }

        site.City = City;
        site.Description = Description;
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Delete(int Id)
    {
        var site = await _context.Sites.FindAsync(Id);
        if (site == null)
        {
            return NotFound();
        }

        _context.Sites.Remove(site);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Details(int Id)
    {
        var site = _context.Sites
            .Include(s => s.Employees)
            .ThenInclude(e => e.Service)
            .FirstOrDefault(s => s.SiteId == Id);
        if (site == null)
        {
            return NotFound();
        }

        return View(site);
    }
}