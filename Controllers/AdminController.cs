using System.Net;
using System.Net.Mail;using System.Security.Claims;
using AnnuaireCESI.Data;
using AnnuaireCESI.Models;
using AnnuaireCESI.Services;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AnnuaireCESI.Controllers;

[Authorize]
public class AdminController : Controller
{
    private readonly AnnuaireDbContext _context;
    private readonly IHashService _hashService;

    public AdminController(AnnuaireDbContext context, IHashService hashService)
    {
        _context = context;
        _hashService = hashService;
    }

    [HttpGet]
    public IActionResult Requests()
    {
        var requests = _context.AccessRequests.Include(r => r.Employee).ToList();
        return View(requests);
    }

    [HttpPost]
    public async Task<IActionResult> DenyRequest(Guid id)
    {
        var requestToDelete = await _context.AccessRequests.FindAsync(id);
        _context.AccessRequests.Remove(requestToDelete);
        await _context.SaveChangesAsync();

        ViewBag.Success = "La demande a bien été refusée";
        return View("Requests");
    }

    [HttpPost]
    public async Task<IActionResult> AcceptRequest(Guid id)
    {
        Console.WriteLine(id);
        var requestToAccept = await _context.AccessRequests.Include(r => r.Employee)
            .FirstOrDefaultAsync(r => r.RequestId == id);

        var employee = requestToAccept.Employee;

        employee.IsAdmin = true;
        var password = Guid.NewGuid().ToString()[..8];
        employee.TempPassword = _hashService.GetHash(password);

        var fromAddress = new MailAddress("evan.osmont@gmail.com", "Agronnect BOT");
        var toAddress = new MailAddress("evan.osmont@gmail.com", employee.FirstName + " " + employee.LastName);
        const string subject = "Agronnect - Demande d'accès approuvée !";
        var body = "Félicitation, votre demande d'accès à été approuvée ! Votre mot de passe temporaire est : " +
                   password + " . Changez le en vous connectant sur Agronnect avec !";

        var SmtpServer = "smtp.gmail.com";
        var SmtpPort = 587;
        var SmtpUsername = "evan.osmont@gmail.com";
        // TODO : Change password before pushing
        var SmtpPassword = "avrtojktuyfgdote";

        using (var message = new MailMessage(fromAddress, toAddress)
               {
                   Subject = subject,
                   Body = body
               })
        {
            using (var smtpClient = new SmtpClient(SmtpServer, SmtpPort))
            {
                smtpClient.Credentials =
                    new NetworkCredential(SmtpUsername, SmtpPassword);
                smtpClient.EnableSsl = true;
                smtpClient.Send(message);
            }
        }

        _context.AccessRequests.Remove(requestToAccept);
        await _context.SaveChangesAsync();
        return RedirectToAction("Requests");
    }
}