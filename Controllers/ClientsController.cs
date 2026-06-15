using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using Global_Logistics_Management_System.Models;

namespace Global_Logistics_Management_System.Controllers
{
    public class ClientsController : Controller
    {
        private readonly HttpClient _httpClient;

        // This is the default port local Web APIs use. We will double-check yours when we run it!
        private readonly string _apiBaseUrl = "https://localhost:7230/api/ClientsApi";

        public ClientsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        // GET: Clients
        public async Task<IActionResult> Index()
        {
            try
            {
                // Grabs raw JSON list from backend and maps it directly to your Client models
                var clients = await _httpClient.GetFromJsonAsync<List<Client>>(_apiBaseUrl);
                return View(clients ?? new List<Client>());
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Unable to connect to the backend logistics API service.");
                return View(new List<Client>());
            }
        }

        // GET: Clients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClientId,Name,Email")] Client client)
        {
            if (!ModelState.IsValid)
            {
                return View(client);
            }

            try
            {
                // Forwards your new client payload directly to the API backend
                var response = await _httpClient.PostAsJsonAsync(_apiBaseUrl, client);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Network error encountered while sending client records.");
            }

            ModelState.AddModelError("", "Server error encountered while registering client payload.");
            return View(client);
        }
    }
}