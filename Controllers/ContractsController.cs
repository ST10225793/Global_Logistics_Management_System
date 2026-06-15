using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Global_Logistics_Management_System.Models;

namespace Global_Logistics_Management_System.Controllers
{
    public class ContractsController : Controller
    {
        private readonly HttpClient _httpClient;

        // Use the exact same running API port number from your launchSettings.json!
        private readonly string _apiBaseUrl = "https://localhost:7230/api/ContractsApi";

        public ContractsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        // GET: Contracts
        public async Task<IActionResult> Index()
        {
            try
            {
                // Calls the Web API backend to retrieve all contract records
                var contracts = await _httpClient.GetFromJsonAsync<List<Contract>>(_apiBaseUrl);
                return View(contracts ?? new List<Contract>());
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Unable to connect to the backend logistics contract service.");
                return View(new List<Contract>());
            }
        }

        // GET: Contracts/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var contract = await _httpClient.GetFromJsonAsync<Contract>($"{_apiBaseUrl}/{id}");
                if (contract == null) return NotFound();
                return View(contract);
            }
            catch (Exception)
            {
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        // POST: Contracts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contract contract)
        {
            if (!ModelState.IsValid)
            {
                return View(contract);
            }

            try
            {
                // Forwards the form data payload over the network port to the backend API
                var response = await _httpClient.PostAsJsonAsync(_apiBaseUrl, contract);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Network error encountered while sending contract data.");
            }

            ModelState.AddModelError("", "Server error encountered while saving contract records.");
            return View(contract);
        }
    }
}