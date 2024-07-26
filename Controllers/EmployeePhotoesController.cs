using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartAttendanceSystem.Data;
using SmartAttendanceSystem.Models;

namespace SmartAttendanceSystem.Controllers
{
    public class EmployeePhotoesController : Controller
    {
        private readonly SADbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EmployeePhotoesController(SADbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: EmployeePhotoes
        public async Task<IActionResult> Index()
        {
            var sADbContext = _context.EmployeePhotos.Include(e => e.Employee);
            return View(await sADbContext.ToListAsync());
        }

        // GET: EmployeePhotoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeePhoto = await _context.EmployeePhotos
                .Include(e => e.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employeePhoto == null)
            {
                return NotFound();
            }

            return View(employeePhoto);
        }

        // GET: EmployeePhotoes/Create
        public IActionResult Create()
        {
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "Department");
            return View();
        }

        // POST: EmployeePhotoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ImageURL,EmployeeId")] EmployeePhoto employeePhoto)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employeePhoto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "Department", employeePhoto.EmployeeId);
            return View(employeePhoto);
        }

        // GET: EmployeePhotoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeePhoto = await _context.EmployeePhotos.FindAsync(id);
            if (employeePhoto == null)
            {
                return NotFound();
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "Department", employeePhoto.EmployeeId);
            return View(employeePhoto);
        }

        // POST: EmployeePhotoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ImageURL,EmployeeId")] EmployeePhoto employeePhoto)
        {
            if (id != employeePhoto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employeePhoto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeePhotoExists(employeePhoto.Id))
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
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "Department", employeePhoto.EmployeeId);
            return View(employeePhoto);
        }

        // GET: EmployeePhotoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeePhoto = await _context.EmployeePhotos
                .Include(e => e.Employee)
                .ThenInclude(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employeePhoto == null)
            {
                return NotFound();
            }

            return View(employeePhoto);
        }

        // POST: EmployeePhotoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //var employeePhoto = await _context.EmployeePhotos.FindAsync(id);
            var employeePhoto = await _context.EmployeePhotos
                                         .Include(e => e.Employee)
                                         .ThenInclude(u => u.User)
                                         .FirstOrDefaultAsync(m => m.Id == id);

            var username = employeePhoto.Employee.User.Username;

            if (employeePhoto != null)
            {
                var userDirectory = Path.Combine(_webHostEnvironment.WebRootPath, "images", username, employeePhoto.ImageURL);
                System.IO.File.Delete(userDirectory);
                _context.EmployeePhotos.Remove(employeePhoto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Employees");
        }

        private bool EmployeePhotoExists(int id)
        {
            return _context.EmployeePhotos.Any(e => e.Id == id);
        }
    }
}
