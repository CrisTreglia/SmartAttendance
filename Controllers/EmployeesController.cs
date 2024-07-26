using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartAttendanceSystem.Data;
using SmartAttendanceSystem.Models;

namespace SmartAttendanceSystem.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly SADbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EmployeesController(SADbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            var sADbContext = _context.Employees.Include(e => e.User);
            return View(await sADbContext.ToListAsync());
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                                         .Include(e => e.User)
                                         .ThenInclude(u => u.Profile)
                                         .Include(e => e.EmployeePhoto)
                                         .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            ViewData["UserID"] = new SelectList(_context.Users, "EmployeeId", "Email");
            return View();
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeId,FirstName,LastName,Email,PhoneNumber,UserID,Department,Position,HireDate")] Employee employee, List<IFormFile> imageFiles, string base64Images)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    employee.User = await _context.Users
                                                  .Include(u => u.Profile)
                                                  .FirstOrDefaultAsync(u => u.EmployeeId == employee.UserID);

                    if (employee.User != null)
                    {
                        var username = employee.User.Username;

                        // Handle uploaded files
                        if (imageFiles != null && imageFiles.Count > 0)
                        {
                            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/" + username);
                            Directory.CreateDirectory(uploadsFolder);

                            foreach (var imageFile in imageFiles)
                            {
                                if (imageFile.Length > 0)
                                {
                                    var fileName = GenerateUniqueFilename(username, Path.GetExtension(imageFile.FileName));
                                    var filePath = Path.Combine(uploadsFolder, fileName);
                                    using (var stream = new FileStream(filePath, FileMode.Create))
                                    {
                                        await imageFile.CopyToAsync(stream);
                                    }

                                    employee.EmployeePhoto.Add(new EmployeePhoto
                                    {
                                        ImageURL = fileName
                                    });
                                }
                            }
                        }

                        // Handle webcam images
                        if (!string.IsNullOrEmpty(base64Images))
                        {
                            var imageDataArray = JsonSerializer.Deserialize<List<string>>(base64Images);

                            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/" + username);
                            Directory.CreateDirectory(uploadsFolder);

                            foreach (var base64Image in imageDataArray)
                            {
                                var imageData = base64Image.Replace("data:image/png;base64,", "");
                                var fileName = GenerateUniqueFilename(username, ".png");
                                var filePath = Path.Combine(uploadsFolder, fileName);

                                await System.IO.File.WriteAllBytesAsync(filePath, Convert.FromBase64String(imageData));
                                employee.EmployeePhoto.Add(new EmployeePhoto
                                {
                                    ImageURL = fileName
                                });
                            }
                        }

                        _context.Add(employee);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                }
            }

            ViewData["UserID"] = new SelectList(_context.Users, "EmployeeId", "Email", employee.UserID);
            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.EmployeePhoto)
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }
            ViewData["UserID"] = new SelectList(_context.Users, "EmployeeId", "Email", employee.UserID);
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EmployeeId,FirstName,LastName,Email,PhoneNumber,UserID,Department,Position,HireDate")] Employee employee, List<IFormFile> imageFiles, string base64Images)
        {
            if (id != employee.EmployeeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingEmployee = await _context.Employees
                                                         .Include(e => e.EmployeePhoto)
                                                         .Include(e => e.User)
                                                         .FirstOrDefaultAsync(e => e.EmployeeId == id);

                    if (existingEmployee == null)
                    {
                        return NotFound();
                    }

                    if (existingEmployee.User == null)
                    {
                        ModelState.AddModelError("", "User not found for the employee.");
                        return View(employee);
                    }

                    // Handle uploaded files
                    if (imageFiles != null && imageFiles.Count > 0)
                    {
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/" + existingEmployee.User.Username);
                        Directory.CreateDirectory(uploadsFolder);

                        foreach (var imageFile in imageFiles)
                        {
                            if (imageFile.Length > 0)
                            {
                                var fileName = GenerateUniqueFilename(existingEmployee.User.Username, Path.GetExtension(imageFile.FileName));
                                var filePath = Path.Combine(uploadsFolder, fileName);
                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    await imageFile.CopyToAsync(stream);
                                }

                                existingEmployee.EmployeePhoto.Add(new EmployeePhoto
                                {
                                    ImageURL = fileName
                                });
                            }
                        }
                    }

                    // Handle webcam images
                    if (!string.IsNullOrEmpty(base64Images))
                    {
                        var imageDataArray = JsonSerializer.Deserialize<List<string>>(base64Images);

                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/" + existingEmployee.User.Username);
                        Directory.CreateDirectory(uploadsFolder);

                        foreach (var base64Image in imageDataArray)
                        {
                            var imageData = base64Image.Replace("data:image/png;base64,", "");
                            var fileName = GenerateUniqueFilename(existingEmployee.User.Username, ".png");
                            var filePath = Path.Combine(uploadsFolder, fileName);

                            await System.IO.File.WriteAllBytesAsync(filePath, Convert.FromBase64String(imageData));
                            existingEmployee.EmployeePhoto.Add(new EmployeePhoto
                            {
                                ImageURL = fileName
                            });
                        }
                    }

                    _context.Entry(existingEmployee).CurrentValues.SetValues(employee);
                    _context.Update(existingEmployee);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.EmployeeId))
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
            ViewData["UserID"] = new SelectList(_context.Users, "EmployeeId", "Email", employee.UserID);
            return View(employee);
        }



        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees
                                         .Include(e => e.User)
                                         .ThenInclude(u => u.Profile)
                                         .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employee != null)
            {
                var username = employee.User.Username;

                var userDirectory = Path.Combine(_webHostEnvironment.WebRootPath, "images", username);

                if (Directory.Exists(userDirectory))
                {
                    try
                    {
                        clearFolder(userDirectory);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error deleting directory: {ex.Message}");
                    }
                }

                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }

        private void clearFolder(string folderName)
        {
            DirectoryInfo dir = new DirectoryInfo(folderName);

            foreach (FileInfo fi in dir.GetFiles())
            {
                fi.Delete();
            }

            foreach (DirectoryInfo di in dir.GetDirectories())
            {
                clearFolder(di.FullName);
                di.Delete();
            }
        }

        private string GenerateUniqueFilename(string username, string extension)
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff"); // Timestamp with milliseconds
            return $"{username}_{timestamp}{extension}";
        }
    }
}
