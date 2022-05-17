using Lab1.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace Lab1.Controllers
{
    public class TaskController : Controller
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly AppDbContext _taskList;
      

        public TaskController(AppDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _taskList = context;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<IActionResult> Index(string sortOrder)
        {
            IEnumerable<TaskEx> tasks = _taskList.Tasks;
            List<TaskEx> list = await _taskList.Tasks.ToListAsync();
            switch (sortOrder)
            {
                case "Название":
                    tasks = tasks.OrderBy(s => s.Name);
                    break;
                case "Дата завершения":
                    tasks = tasks.OrderBy(s => s.Date);
                    break;
                case "Статус":
                    tasks = tasks.OrderBy(s => s.Status);
                    break;
                default:
                    tasks = tasks.OrderBy(s => s.Date);
                    break;
            }

            SortingViewModel filerViewModel = new SortingViewModel
            {
                Tasks = tasks,
                SortOrder = new SelectList(new List<string>()
                {
                    "Название",
                    "Дата завершения",
                    "Статус",
                }),
            };
            return View(filerViewModel);
        }

        // GET: TaskController/Create
        public ActionResult Create()
        {
            ViewBag.Statuses = new SelectList(new List<string>()
                {
                    "active",
                    "complited",
                });

            return View();
        }

        // POST: TaskController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateTaskModel model)
        {

            if (ModelState.IsValid)
            {
                //Save image to wwwroot/image
                if (model.ImageFile != null)
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);
                    string extension = Path.GetExtension(model.ImageFile.FileName);
                    model.ImageName = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    string path = Path.Combine(wwwRootPath + "/Image/", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(fileStream);
                    }
                }

                TaskEx taskEx = new TaskEx
                {
                    Id = Guid.NewGuid().ToString(),
                    Status = model.Status,
                    Name = model.Name,
                    ImageName = model.ImageName,
                    Date = model.Date
                };

                _taskList.Tasks.Add(taskEx);
                await _taskList.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        // GET: TaskController/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {


            if (id == null)
            {
                return NotFound();
            }

            var task = await _taskList.Tasks.FirstOrDefaultAsync(t => t.Id == id.ToString());

            ViewBag.Statuses = new SelectList(new List<string>()
                {
                    "active",
                    "complited",
                }, task.Status);

            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // POST: TaskController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Guid? id, TaskEx task)
        {
            if (id.ToString() != task.Id && id == null)
            {
                return NotFound();
            }

            var data = await _taskList.Tasks
                            .FirstOrDefaultAsync(c => c.Id == id.ToString());

            if (ModelState.IsValid)
            {
                if (task.ImageFile != null)
                {
                    //Save image to wwwroot/image
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(task.ImageFile.FileName);
                    string extension = Path.GetExtension(task.ImageFile.FileName);
                    task.ImageName = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    string path = Path.Combine(wwwRootPath + "/Image/", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await task.ImageFile.CopyToAsync(fileStream);
                    }
                    //Insert record
                    data.ImageName = task.ImageName;
                }
                data.Date = task.Date;
                data.Name = task.Name;
                data.Status = task.Status;

                _taskList.Update(data);

                await _taskList.SaveChangesAsync();

                return RedirectToAction(nameof(Index), nameof(Task), await _taskList.Tasks.ToListAsync());
            }

            return View(data);
        }

        // POST: TaskController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(Guid id)
        {
            var task = await _taskList.Tasks.FindAsync(id.ToString());

            if (task.ImageName != "NoImage.png" && task.ImageName != null)
            {
                var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "image", task.ImageName);
                if (System.IO.File.Exists(imagePath))
                    System.IO.File.Delete(imagePath);
            }

            _taskList.Tasks.Remove(task);
            await _taskList.SaveChangesAsync();

            return RedirectToAction(nameof(Index), nameof(Task), await _taskList.Tasks.ToListAsync());
        }

        public async Task<IActionResult> Filter(string sortOrder)
        {
            var tasks = from s in _taskList.Tasks
                        select s;


            switch (sortOrder)
            {
                case "Названию":
                    tasks = tasks.OrderBy(s => s.Name);
                    break;
                case "Дате":
                    tasks = tasks.OrderBy(s => s.Date);
                    break;
                case "Статусу":
                    tasks = tasks.OrderBy(s => s.Status);
                    break;
                default:
                    tasks = tasks.OrderBy(s => s.Date);
                    break;
            }

            return RedirectToAction(nameof(Index), nameof(Task), await tasks.ToListAsync());
        }
    }
}
