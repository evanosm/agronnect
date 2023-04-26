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
        Console.WriteLine(email, details);
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Email == email);

        if (employee == null)
        {
            ViewBag.Error =
                "L'adresse email n'existe pas dans notre base de donnée, veuillez contacter votre administrateur.";
            return View("RequestAccess");
        }

        if (employee.IsAdmin)
        {
            ViewBag.Error = "Vous êtes déjà administrateur.";
            return View("RequestAccess");
        }

        var request = new AccessRequest()
        {
            RequestId = Guid.NewGuid(),
            Employee = employee,
            Details = details
        };
        await _context.AccessRequests.AddAsync(request);
        await _context.SaveChangesAsync();
        ViewBag.Success =
            "Votre demande a bien été prise en compte, vous recevrez un email de confirmation avec un mot de passe lorsque votre compte sera activé.";
        return View("RequestAccess");
    }
}