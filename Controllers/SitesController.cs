using AnnuaireCESI.Data;
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
        var sites = _context.Sites.Include(s => s.Employees).ToListAsync();
        return View(await sites);
    }
}