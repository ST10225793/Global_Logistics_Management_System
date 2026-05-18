using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            // 1. Start with a base LINQ query including the Client table
            var contractsQuery = _context.Contracts.Include(c => c.Client).AsQueryable();

            // 2. LINQ Filter: Status Dropdown
            if (filterStatus.HasValue)
            {
                contractsQuery = contractsQuery.Where(c => c.Status == filterStatus.Value);
            }

            // 3. LINQ Filter: Start Date (Contracts starting on or after this date)
            if (filterStartDate.HasValue)
            {
                contractsQuery = contractsQuery.Where(c => c.StartDate >= filterStartDate.Value);
            }

            // 4. LINQ Filter: End Date (Contracts ending on or before this date)
            if (filterEndDate.HasValue)
            {
                contractsQuery = contractsQuery.Where(c => c.EndDate <= filterEndDate.Value);
            }

            // Pass the selected filters back to the view using ViewBag so the form inputs don't reset clear after clicking search
            ViewBag.CurrentStartDate = filterStartDate?.ToString("yyyy-MM-dd");
            ViewBag.CurrentEndDate = filterEndDate?.ToString("yyyy-MM-dd");
            ViewBag.CurrentStatus = filterStatus;

            // 5. Execute the query and send the filtered list to the view
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ContractId,ContractName,Description,Status,ServiceLevel,StartDate,EndDate,CostUSD,ClientId")] Contract contract, IFormFile pdfFile)
        {
            if (!ModelState.IsValid)
            {
                // list every error in your Visual Studio "Output" window
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    System.Diagnostics.Debug.WriteLine("ENTITY ERROR: " + error.ErrorMessage);
                }
            }

            if (pdfFile != null && pdfFile.Length > 0)
            {
                // 1. Create a unique filename
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(pdfFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                // 2. Save file to server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await pdfFile.CopyToAsync(stream);
                }

                // 3. Save the path to the database object
                contract.SignedAgreementPath = fileName;
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ContractId,ContractName,Description,Status,ServiceLevel,StartDate,EndDate,CostUSD,ClientId,SignedAgreementPath")] Contract contract, IFormFile? pdfFile)
        {
            if (id != contract.ContractId)
            {
                return NotFound();
            }

            // Process a replacement PDF file if uploaded
            if (pdfFile != null && pdfFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(pdfFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await pdfFile.CopyToAsync(stream);
                }

                // Assign new file path
                contract.SignedAgreementPath = fileName;
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
            ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "Name", contract.ClientId);
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
