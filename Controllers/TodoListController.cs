using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApp.Entities;
using TodoApp.Models;

namespace TodoApp.Controllers
{
    [Authorize]
    public class TodoListController : Controller
    {
        private readonly ILogger<TodoListController> _logger;
        private readonly TodoContext _context;
        private readonly IEmojisRemover _emojisRemover;
        private readonly UserManager<ApplicationUser> _userManager;

        public TodoListController(
            ILogger<TodoListController> logger,
            UserManager<ApplicationUser> userManager,
            TodoContext context,
            IEmojisRemover emojisRemover
        )
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
            _emojisRemover = emojisRemover;
        }

        [HttpGet]
        public IActionResult Index()
        {
            string userId = _userManager.GetUserId(User)!;

            List<TodoList> todoLists = _context
                .TodoLists.Where(x => x.UserId == userId)
                .AsEnumerable()
                .OrderBy(x => _emojisRemover.RemoveEmojis(x.Title).TrimStart())
                .ToList();
            ViewData["Title"] = "My Lists";
            return View(todoLists);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewData["Title"] = "Create";
            ViewData["Action"] = "Create";
            return View("CreateUpdate");
        }

        [HttpPost]
        public IActionResult Create(TodoList list)
        {
            list.UserId = _userManager.GetUserId(User)!;
            if (!ModelState.IsValid)
            {
                ViewData["Title"] = "Create";
                ViewData["Action"] = "Create";
                return View("CreateUpdate", list);
            }

            try
            {
                _context.TodoLists.Add(list);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.Message);
                ViewData["Title"] = "Create";
                ViewData["Action"] = "Create";
                ViewData["DuplicateListName"] = "there is a list has the same title !";
                return View("CreateUpdate", list);
            }
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            string userId = _userManager.GetUserId(User);
            TodoList? list = _context.TodoLists.Find(id)!;
            if (list is null || list.UserId != userId)
                return RedirectToAction("Index");
            try
            {
                ViewData["Title"] = "Edit";
                ViewData["Action"] = "Update";
                return View("CreateUpdate", list);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.Message);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Update(TodoList list)
        {
            string userId = _userManager.GetUserId(User)!;
            if (!_context.TodoLists.Any(x => x.Id == list.Id && x.UserId == userId))
                return RedirectToAction("Index");
            list.UserId = userId;
            if (!ModelState.IsValid)
            {
                ViewData["Title"] = "Edit";
                ViewData["Action"] = "Update";
                return View("CreateUpdate", list);
            }
            try
            {
                _context.TodoLists.Update(list);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.Message);
                ViewData["DuplicateListName"] = "there is a list has the same title !";
                ViewData["Title"] = "Edit";
                ViewData["Action"] = "Update";
            }
            return View("CreateUpdate", list);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            string userId = _userManager.GetUserId(User)!;
            TodoList? list = _context.TodoLists.Find(id);
            if (list is null || list.UserId != userId)
                return RedirectToAction("Index");
            try
            {
                _context.Todos.Where(x => x.ListId == id).ExecuteDelete();
                _context.TodoLists.Remove(list);
                _context.SaveChanges();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.Message);
            }
            return RedirectToAction("Index");
        }
    }
}
