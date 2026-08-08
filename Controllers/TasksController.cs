using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoListApp.Data;
using ToDoListApp.Models;
using ToDoListApp.ViewModels;

namespace ToDoListApp.Controllers
{
    public class TasksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public TasksController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Tasks (عرض المهام + البحث + Pagination)
        public async Task<IActionResult> Index(string searchTerm, int page = 1)
        {
            int pageSize = 5;
            var query = _context.Tasks.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(t => t.Title.Contains(searchTerm) || (t.Description != null && t.Description.Contains(searchTerm)));
            }

            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var tasks = await query
                .OrderBy(t => t.Deadline)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new TaskListViewModel
            {
                Tasks = tasks,
                SearchTerm = searchTerm,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize
            };

            return View(viewModel);
        }

        // GET: Tasks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tasks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string? dbFilePath = null;
                string? originalFileName = null;

                if (model.FormFile != null && model.FormFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    originalFileName = Path.GetFileName(model.FormFile.FileName);
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + originalFileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.FormFile.CopyToAsync(fileStream);
                    }

                    dbFilePath = "/uploads/" + uniqueFileName;
                }

                var task = new TaskItem
                {
                    Title = model.Title,
                    Description = model.Description,
                    Deadline = model.Deadline,
                    FilePath = dbFilePath,
                    OriginalFileName = originalFileName
                };

                _context.Add(task);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Tasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return NotFound();

            var viewModel = new TaskEditViewModel
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Deadline = task.Deadline,
                ExistingFilePath = task.FilePath,
                ExistingFileName = task.OriginalFileName
            };

            return View(viewModel);
        }

        // POST: Tasks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TaskEditViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var task = await _context.Tasks.FindAsync(id);
                if (task == null) return NotFound();

                task.Title = model.Title;
                task.Description = model.Description;
                task.Deadline = model.Deadline;

                if (model.NewFile != null && model.NewFile.Length > 0)
                {
                    if (!string.IsNullOrEmpty(task.FilePath))
                    {
                        string oldPath = Path.Combine(_environment.WebRootPath, task.FilePath.TrimStart('/'));
                        if (System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath);
                        }
                    }

                    string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string originalFileName = Path.GetFileName(model.NewFile.FileName);
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + originalFileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.NewFile.CopyToAsync(fileStream);
                    }

                    task.FilePath = "/uploads/" + uniqueFileName;
                    task.OriginalFileName = originalFileName;
                }

                _context.Update(task);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // POST: Tasks/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task != null)
            {
                if (!string.IsNullOrEmpty(task.FilePath))
                {
                    string filePath = Path.Combine(_environment.WebRootPath, task.FilePath.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Tasks/Download/5
        public async Task<IActionResult> Download(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null || string.IsNullOrEmpty(task.FilePath))
            {
                return NotFound();
            }

            string absolutePath = Path.Combine(_environment.WebRootPath, task.FilePath.TrimStart('/'));
            if (!System.IO.File.Exists(absolutePath))
            {
                return NotFound();
            }

            byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(absolutePath);
            string fileName = task.OriginalFileName ?? "downloaded_file";

            return File(fileBytes, "application/octet-stream", fileName);
        }
    }
}