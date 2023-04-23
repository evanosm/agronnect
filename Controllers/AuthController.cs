using AnnuaireCESI.Data;
using Microsoft.AspNetCore.Mvc;

namespace AnnuaireCESI.Controllers;

public class AuthController : Controller
{
    private readonly AnnuaireDbContext _context;

    public AuthController(AnnuaireDbContext context)
    {
        this._context = context;
    }

    public IActionResult Login()
    {
        return View();
    }
}