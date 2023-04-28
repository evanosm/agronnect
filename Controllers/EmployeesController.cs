using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AnnuaireCESI.Models;
using AnnuaireCESI.Data;
using Microsoft.AspNetCore.Authorization;

namespace AnnuaireCESI.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly AnnuaireDbContext _context;

        public EmployeesController(AnnuaireDbContext context)
        {
            this._context = context;
        }

        //GET: Employees
        [HttpGet]
        public async Task<IActionResult> Index(int Page = 1)
        {
            var empPerPage = 10;
            var employees = await _context.Employees
                .Include(e => e.Site)
                .Include(e => e.Service)
                .OrderBy(e => e.LastName)
                .Skip((Page - 1) * empPerPage)
                .Take(empPerPage)
                .ToListAsync();

            var sites = await _context.Sites.ToListAsync();
            var services = await _context.Services.ToListAsync();

            var Data = new EmployeeIndexViewModel()
            {
                Employees = employees,
                Sites = sites,
                Services = services,
            };
            return View(Data);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Add(AddEmployeeViewModel newEmployee)
        {
            var newService = await _context.Services.FindAsync(newEmployee.ServiceId);
            var newSite = await _context.Sites.FindAsync(newEmployee.SiteId);

            var emailExist = await _context.Employees.AnyAsync(e => e.Email == newEmployee.Email);

            if (!emailExist)
            {
                var employee = new Employee()
                {
                    EmployeeId = Guid.NewGuid(),
                    FirstName = newEmployee.FirstName,
                    LastName = newEmployee.LastName,
                    Email = newEmployee.Email,
                    Phone = newEmployee.Phone,
                    Mobile = newEmployee.Mobile,
                    Service = newService,
                    Site = newSite
                };

                await _context.Employees.AddAsync(employee);
                await _context.SaveChangesAsync();

                ViewBag.Success = "L'employé a bien été ajouté";
                return View("Index");
            }
            else
            {
                ViewBag.Error = "L'employé avec cette adresse mail existe déjà";
                return View("Index");
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Update(UpdateEmployeeViewModel updatedEmployee)
        {
            var newService = await _context.Services.FindAsync(updatedEmployee.ServiceId);
            var newSite = await _context.Sites.FindAsync(updatedEmployee.SiteId);
            var employee = await _context.Employees.Include(e => e.Service).Include(e => e.Site)
                .FirstOrDefaultAsync(e => e.Email == updatedEmployee.Email);

            if (employee != null)
            {
                employee.FirstName = updatedEmployee.FirstName;
                employee.LastName = updatedEmployee.LastName;
                employee.Email = updatedEmployee.Email;
                employee.Phone = updatedEmployee.Phone;
                employee.Mobile = updatedEmployee.Mobile;
                employee.Site = newSite;
                employee.Service = newService;


                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            ViewBag.Error = "L'employé n'a pas pu être modifié";
            return View("Index");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            Console.WriteLine("Delete: " + id);
            var employee = await _context.Employees.FindAsync(id);
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Search()
        {
            
            var search = Request.Query["Search"].ToString();
            var trimmedSearch = search.Trim().ToLower();
            var employees = await _context.Employees
                .Include(e => e.Site)
                .Include(e => e.Service)
                .Where(e => e.FirstName.Trim().ToLower().Contains(trimmedSearch)
                            || e.LastName.Trim().ToLower().Contains(trimmedSearch)
                            || e.Email.Trim().ToLower().Contains(trimmedSearch)
                            || e.Phone.Trim().ToLower().Contains(trimmedSearch)
                            || e.Mobile.Trim().ToLower().Contains(trimmedSearch)
                            || e.Service.Name.Trim().ToLower().Contains(trimmedSearch)
                            || e.Site.City.Trim().ToLower().Contains(trimmedSearch))
                .ToListAsync();

            var sites = await _context.Sites.ToListAsync();
            var services = await _context.Services.ToListAsync();

            var Data = new EmployeeIndexViewModel()
            {
                Employees = employees,
                Sites = sites,
                Services = services,
                Search = search
            };
            ViewBag.Search = search;
            return View("Index", Data);
        }
    }
}