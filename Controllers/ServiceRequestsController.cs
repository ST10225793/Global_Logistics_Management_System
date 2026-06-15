using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Global_Logistics_Management_System.Models;

namespace Global_Logistics_Management_System.Controllers
{
    public class ServiceRequestsController : Controller
    {
        private readonly HttpClient _httpClient;

        // Use your exact running API port number!
        private readonly string _apiBaseUrl = "https://localhost:7230/api/ServiceRequestsApi";

        public ServiceRequestsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        // GET: ServiceRequests
        public async Task<IActionResult> Index()
        {
            try
            {
                // Calls the Web API backend to retrieve logistics booking requests
                var requests = await _httpClient.GetFromJsonAsync<List<ServiceRequest>>(_apiBaseUrl);
                return View(requests ?? new List<ServiceRequest>());
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Unable to connect to the backend service requests API.");
                return View(new List<ServiceRequest>());
            }
        }

        // GET: ServiceRequests/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var serviceRequest = await _httpClient.GetFromJsonAsync<ServiceRequest>($"{_apiBaseUrl}/{id}");
                if (serviceRequest == null) return NotFound();
                return View(serviceRequest);
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

        // POST: ServiceRequests/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceRequest serviceRequest)
        {
            if (!ModelState.IsValid)
            {
                return View(serviceRequest);
            }

            try
            {
                // Forwards the form data payload over the network port to the backend API
                var response = await _httpClient.PostAsJsonAsync(_apiBaseUrl, serviceRequest);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Network error encountered while sending booking requests.");
            }

            ModelState.AddModelError("", "Server error encountered while saving booking records.");
            return View(serviceRequest);
        }
    }
}