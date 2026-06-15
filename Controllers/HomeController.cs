using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Global_Logistics_Management_System.Models;

namespace Global_Logistics_Management_System.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClient;

        // Use the exact same API port number you looked up in Step 6!
        private readonly string _apiBaseUrl = "https://localhost:7230/api/ContractsApi";

        // The constructor now takes the standard framework logger and the web connection agent
        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
        }

        // GET: Home/Index
        public async Task<IActionResult> Index()
        {
            try
            {
                // Asks the Web API for the contracts list over the local network port
                var contracts = await _httpClient.GetFromJsonAsync<List<Contract>>(_apiBaseUrl);

                // Takes the top 5 records to keep the frontend view layout looking neat
                var dashboardContracts = contracts ?? new List<Contract>();
                dashboardContracts = dashboardContracts.FindAll(c => true).GetRange(0, Math.Min(5, dashboardContracts.Count));

                return View(dashboardContracts);
            }
            catch (Exception)
            {
                _logger.LogError("Could not retrieve dashboard contracts from the backend service.");
                return View(new List<Contract>());
            }
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