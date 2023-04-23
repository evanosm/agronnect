using AnnuaireCESI.Data;
using AnnuaireCESI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AnnuaireCESI.Controllers;

public class AuthController : Controller
{
    private readonly AnnuaireDbContext _context;

    public AuthController(AnnuaireDbContext context)
    {
        this._context = context;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpGet]
    public IActionResult RequestAccess()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> RequestAccess(string email, string details)
    {
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Email == email);

        if (employee == null)
        {
            return NotFound();
        }

        var request = new AccessRequest()
        {
            RequestId = Guid.NewGuid(),
            Employee = employee,
            Details = details
        };
        await _context.SaveChangesAsync();
        return RedirectToAction("Login");
    }
}