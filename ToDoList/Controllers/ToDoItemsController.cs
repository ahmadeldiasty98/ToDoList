using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Threading.Tasks;
using ToDoList.Data;
using ToDoList.Models;

namespace ToDoList.Controllers
{
    public class ToDoItemsController : Controller
    {
        ApplicationDbContext DbContext = new ApplicationDbContext();

        #region Add New Name
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddName(String PersonName)
        {
            CookieOptions options = new CookieOptions { Expires = DateTime.Now.AddDays(1) };
            HttpContext.Response.Cookies.Append("Name", PersonName ,options);
            return RedirectToAction("Items");
        }
        #endregion

        #region Displaying Items 
        public IActionResult Items()
        {
            ViewBag.Name = HttpContext.Request.Cookies["Name"] ;
            var ItemsToDo = DbContext.TasksToDos.ToList();
            return View(ItemsToDo);
        }
        #endregion
        
        #region Create New Item To Do
        public IActionResult CreateItem()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateItem(TasksToDo tasksToDo , IFormFile PdfUrl)
        {
            if (PdfUrl.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(PdfUrl.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Pdfs", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    PdfUrl.CopyTo(stream);
                }

                tasksToDo.PdfUrl = fileName;
            }

            DbContext.TasksToDos.Add(tasksToDo);
            DbContext.SaveChanges();
            TempData["Created"] = tasksToDo.Title + " Created Successfully";
            return RedirectToAction("Items");
        }
        #endregion

        #region Edit Item
        public IActionResult EditItem(int tasksToDoId)
        {
            var task = DbContext.TasksToDos.Find(tasksToDoId);
            return View(task);
        }

        [HttpPost]
        public IActionResult EditItem(TasksToDo tasksToDo, IFormFile PdfUrl)
        {
            var oldTask = DbContext.TasksToDos.AsNoTracking().FirstOrDefault(e => e.Id == tasksToDo.Id);
            if (PdfUrl !=null && PdfUrl.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(PdfUrl.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Pdfs", fileName);
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Pdfs", oldTask.PdfUrl);
                using (var stream = System.IO.File.Create(filePath))
                {
                    PdfUrl.CopyTo(stream);
                }
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
                tasksToDo.PdfUrl = fileName;
            }
            else
            {
                tasksToDo.PdfUrl = oldTask.PdfUrl;
            }

            DbContext.TasksToDos.Update(tasksToDo);
            DbContext.SaveChanges();
            TempData["Created"] = tasksToDo.Title + " Edited Successfully";
            return RedirectToAction("Items");
        }
        #endregion

        #region Delete Item
        public IActionResult DeleteItem(int tasksToDoId) {
            var task = DbContext.TasksToDos.Find(tasksToDoId);
            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Pdfs", task.PdfUrl);

            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }

            DbContext.TasksToDos.Remove(task);
            DbContext.SaveChanges();
            TempData["Created"] = task.Title + " Deleted Successfully";
            return RedirectToAction(nameof(Items));
        }
        #endregion

        #region Download Task File
        public IActionResult Download(string pdfUrl)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Pdfs", pdfUrl);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }
            var contentType = "application/pdf";
            var fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, contentType, pdfUrl);
            
        }
        #endregion
    }
}
