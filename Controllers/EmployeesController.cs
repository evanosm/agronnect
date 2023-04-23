using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AnnuaireCESI.Models;
using AnnuaireCESI.Data;

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
        public async Task<IActionResult> Index()
        {
            //get employees from database
            var employees = await _context.Employees.ToListAsync();
            return View(employees);
        }

        //POST: Employees/Create
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var services = await _context.Services.ToListAsync();
            var sites = await _context.Sites.ToListAsync();

            var viewModel = new AddEmployeeViewModel()
            {
                Services = services,
                Sites = sites
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddEmployeeViewModel newEmployee)
        {
            Console.WriteLine("NEW EMPLOYEE SERVICE & SITE");
            Console.WriteLine(newEmployee.ServiceId);
            Console.WriteLine(newEmployee.SiteId);
                
            var newService = await _context.Services.FindAsync(newEmployee.ServiceId);
            var newSite = await _context.Sites.FindAsync(newEmployee.SiteId);
            
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

            return RedirectToAction("Add");
        }
    }
}