using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Global_Logistics_Management_System.Data;
using Global_Logistics_Management_System.Models;

namespace Global_Logistics_Management_System.Controllers
{
    public class ContractsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContractsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Contracts
        public async Task<IActionResult> Index(DateTime? filterStartDate, DateTime? filterEndDate, ContractStatus? filterStatus)
        {
            // 1. Base query incorporating relational Client entities
            var contractsQuery = _context.Contracts.Include(c => c.Client).AsQueryable();

            // 2. Status Dropdown Evaluation
            if (filterStatus.HasValue)
            {
                contractsQuery = contractsQuery.Where(c => c.Status == filterStatus.Value);
            }

            // 3. Start Date Evaluation Matrix
            if (filterStartDate.HasValue)
            {
                contractsQuery = contractsQuery.Where(c => c.StartDate >= filterStartDate.Value);
            }

            // 4. End Date Evaluation Matrix
            if (filterEndDate.HasValue)
            {
                contractsQuery = contractsQuery.Where(c => c.EndDate <= filterEndDate.Value);
            }

            // Maintain state inside ViewBag hooks to prevent client inputs resetting after filter dispatch
            ViewBag.CurrentStartDate = filterStartDate?.ToString("yyyy-MM-dd");
            ViewBag.CurrentEndDate = filterEndDate?.ToString("yyyy-MM-dd");
            ViewBag.CurrentStatus = filterStatus;

            return View(await contractsQuery.ToListAsync());
        }

        // GET: Contracts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts
                .Include(c => c.Client)
                .FirstOrDefaultAsync(m => m.ContractId == id);

            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        // GET: Contracts/Create
        public IActionResult Create()
        {
            ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "Email");
            return View();
        }

        // POST: Contracts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ContractId,ContractName,Description,Status,ServiceLevel,StartDate,EndDate,CostUSD,ClientId")] Contract contract, IFormFile? pdfFile)
        {
            // --- BUSINESS RULE WORKFLOW & FILE VALIDATION LAYER ---

            // Rule A: Enforce timeline boundaries gracefully
            if (contract.StartDate >= contract.EndDate)
            {
                ModelState.AddModelError("EndDate", "Validation Rule: The contract end date must fall after the starting initialization timestamp.");
            }

            // Rule B: Enforce file handling policies (Strict PDF constraints with UUID renaming)
            if (pdfFile != null && pdfFile.Length > 0)
            {
                var fileExtension = Path.GetExtension(pdfFile.FileName).ToLower();
                if (fileExtension != ".pdf")
                {
                    ModelState.AddModelError("", "Security Breach Blocked: Only official .pdf documentation layout files are accepted.");
                }
                else
                {
                    // Generate a distinct UUID string to protect host tracking structures from overwrites
                    var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                    var targetUploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                    if (!Directory.Exists(targetUploadFolder))
                    {
                        Directory.CreateDirectory(targetUploadFolder);
                    }

                    var serverSavePath = Path.Combine(targetUploadFolder, uniqueFileName);
                    using (var fileStream = new FileStream(serverSavePath, FileMode.Create))
                    {
                        await pdfFile.CopyToAsync(fileStream);
                    }

                    // Save clean location path variables directly inside database object tracking fields
                    contract.SignedAgreementPath = uniqueFileName;
                }
            }

            // Output diagnostic debug validation alerts directly to IDE output panels if invalid 
            if (!ModelState.IsValid)
            {
                foreach (var stateItem in ModelState.Values)
                {
                    foreach (var validationError in stateItem.Errors)
                    {
                        System.Diagnostics.Debug.WriteLine($"BUSINESS CRITERIA EXCEPTION: {validationError.ErrorMessage}");
                    }
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(contract);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "Email", contract.ClientId);
            return View(contract);
        }

        // GET: Contracts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null)
            {
                return NotFound();
            }

            ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "Email", contract.ClientId);
            return View(contract);
        }

        // POST: Contracts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ContractId,ContractName,Description,Status,ServiceLevel,StartDate,EndDate,CostUSD,ClientId,SignedAgreementPath")] Contract contract, IFormFile? pdfFile)
        {
            if (id != contract.ContractId)
            {
                return NotFound();
            }

            // Timeline boundary verification checks
            if (contract.StartDate >= contract.EndDate)
            {
                ModelState.AddModelError("EndDate", "Validation Rule: The contract end date must fall after the starting initialization timestamp.");
            }

            // Check if user is uploading a replacement SLA document context sheet
            if (pdfFile != null && pdfFile.Length > 0)
            {
                var fileExtension = Path.GetExtension(pdfFile.FileName).ToLower();
                if (fileExtension != ".pdf")
                {
                    ModelState.AddModelError("", "Security Breach Blocked: Only official .pdf documentation layout files are accepted.");
                }
                else
                {
                    // Generate new unique UUID string identifier
                    var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                    var targetUploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                    if (!Directory.Exists(targetUploadFolder))
                    {
                        Directory.CreateDirectory(targetUploadFolder);
                    }

                    var serverSavePath = Path.Combine(targetUploadFolder, uniqueFileName);
                    using (var fileStream = new FileStream(serverSavePath, FileMode.Create))
                    {
                        await pdfFile.CopyToAsync(fileStream);
                    }

                    contract.SignedAgreementPath = uniqueFileName;
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contract);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContractExists(contract.ContractId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "Email", contract.ClientId);
            return View(contract);
        }

        // GET: Contracts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts
                .Include(c => c.Client)
                .FirstOrDefaultAsync(m => m.ContractId == id);

            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        // POST: Contracts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract != null)
            {
                _context.Contracts.Remove(contract);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContractExists(int id)
        {
            return _context.Contracts.Any(e => e.ContractId == id);
        }
    }
}