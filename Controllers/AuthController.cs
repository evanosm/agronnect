using System.Security.Claims;
using AnnuaireCESI.Data;
using AnnuaireCESI.Models;
using AnnuaireCESI.Services;
using BCrypt.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AnnuaireCESI.Controllers;

public class AuthController : Controller
{
    private readonly AnnuaireDbContext _context;
    private readonly IHashService _hashService;

    public AuthController(AnnuaireDbContext context, IHashService hashService)
    {
        _context = context;
        _hashService = hashService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string Email, string Password)
    {
        Console.WriteLine(Password);
        //find employee with email
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Email == Email);

        if (employee == null)
        {
            ViewBag.Error = "Email ou mot de passe incorrect";
            return View();
        }


        //Hash password and compare it to Password in DB
        var hash = _hashService.GetHash(Password);

        if (employee.Password != null)
        {
            var isPasswordCorrect = hash == employee.Password;
            if (isPasswordCorrect)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, employee.FirstName),
                    new Claim(ClaimTypes.Role, employee.IsAdmin ? "Admin" : "User"),
                    new Claim(ClaimTypes.Email, employee.Email),
                    new Claim("Id", employee.EmployeeId.ToString())
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                var props = new AuthenticationProperties();
                HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    props).Wait();

                return RedirectToAction("Index", "Home");
            }
        }
        else if (employee.TempPassword != null)
        {
            var isTempPasswordCorrect = hash == employee.TempPassword;
            Console.WriteLine(employee.TempPassword);
            Console.WriteLine(hash);
            if (isTempPasswordCorrect)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, employee.EmployeeId.ToString()),
                    new Claim(ClaimTypes.Name, employee.FirstName),
                    new Claim(ClaimTypes.Role, employee.IsAdmin ? "Admin" : "User"),
                    new Claim(ClaimTypes.Email, employee.Email)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                var props = new AuthenticationProperties();
                HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    props).Wait();

                return RedirectToAction("ChangePassword", "Auth");
            }
        }

        ViewBag.Error = "Mot de passe incorrect";
        return View();
    }

    [Authorize]
    [HttpGet]
    public IActionResult ChangePassword()
    {
        // if user auth
        if (User.Identity.IsAuthenticated)
        {
            return View();
        }
        else
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> ChangePassword(string Password)
    {
        ClaimsPrincipal currentUser = this.User;
        Guid id = Guid.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value);
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeId == id);
        var hashedPassword = _hashService.GetHash(Password);
        employee.Password = hashedPassword;
        employee.TempPassword = null;
        await _context.SaveChangesAsync();
        return RedirectToAction("Index", "Home");
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

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}