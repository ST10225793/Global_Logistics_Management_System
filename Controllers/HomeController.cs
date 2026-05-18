using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Global_Logistics_Management_System.Data;
using Global_Logistics_Management_System.Models;

namespace Global_Logistics_Management_System.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        // Combined constructor to handle both framework logging and our SQL Database Context
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // GET: Home/Index
        public async Task<IActionResult> Index()
        {
            // Fetch top 5 contracts including relational Client data for the summary dashboard layout
            var dashboardContracts = await _context.Contracts
                                                  .Include(c => c.Client)
                                                  .Take(5)
                                                  .ToListAsync();

            return View(dashboardContracts);
        }

        // GET: Home/Privacy
        public IActionResult Privacy()
        {
            return View();
        }

        // GET: Home/Error
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}