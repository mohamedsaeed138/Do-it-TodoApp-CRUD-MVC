using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApp.Entities;
using TodoApp.Helpers;
using TodoApp.Models;

namespace TodoApp.Controllers
{
    [Authorize]
    public class TodoController : Controller
    {
        private readonly ILogger<TodoController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TodoContext _context;
        private readonly IEmojisRemover _emojisRemover;

        public TodoController(
            ILogger<TodoController> logger,
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

        private void AddModelsToViewData()
        {
            string userId = _userManager.GetUserId(User)!;
            ViewData["TodoLists"] = _context
                .TodoLists.Where(x => x.UserId == userId)
                .AsEnumerable()
                .OrderBy(x => _emojisRemover.RemoveEmojis(x.Title).TrimStart())
                .ToList();

            ViewData["Priorities"] = EnumHelper.ToArray<Priority>();
            ViewData["Statuses"] = EnumHelper.ToArray<Status>();
        }

        public IActionResult Index()
        {
            string userId = _userManager.GetUserId(User)!;
            var todos = _context
                .Todos.Include(x => x.List)
                .Where(x => x.UserId == userId)
                .AsEnumerable()
                .OrderBy(x => _emojisRemover.RemoveEmojis(x.Title).TrimStart())
                .ToList();

            ViewData["ListsTitles"] = _context
                .TodoLists.Where(x => x.UserId == userId)
                .Select(x => x.Title)
                .AsEnumerable()
                .OrderBy(x => _emojisRemover.RemoveEmojis(x).TrimStart())
                .ToList();
            ViewData["Priorities"] = EnumHelper.ToArray<Priority>();
            ViewData["Statuses"] = EnumHelper.ToArray<Status>();
            return View(todos);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewData["Title"] = "Create";
            ViewData["Action"] = "Create";
            AddModelsToViewData();
            return View("CreateUpdate");
        }

        [HttpPost]
        public IActionResult Create(Todo todo)
        {
            todo.UserId = _userManager.GetUserId(User);

            if (
                !ModelState.IsValid
                || (
                    todo.ListId is not null
                    && !_context.TodoLists.Any(x => x.Id == todo.ListId && x.UserId == todo.UserId)
                )
            )
            {
                ViewData["Title"] = "Create";
                ViewData["Action"] = "Create";
                AddModelsToViewData();
                return View("CreateUpdate", todo);
            }

            try
            {
                _context.Todos.Add(todo);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.Message);
                ViewData["Title"] = "Create";
                ViewData["Action"] = "Create";
                AddModelsToViewData();
                return View("CreateUpdate", todo);
            }
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            string userId = _userManager.GetUserId(User);
            Todo? todo = _context.Todos.Find(id);
            if (todo is null || todo.UserId != userId)
                return RedirectToAction("Index");
            try
            {
                ViewData["Title"] = "Edit";
                ViewData["Action"] = "Update";
                AddModelsToViewData();
                return View("CreateUpdate", todo);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.Message);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Update(Todo todo)
        {
            string userId = _userManager.GetUserId(User)!;
            if (!_context.Todos.Any(x => x.Id == todo.Id && x.UserId == userId))
                return RedirectToAction("Index");
            var isListnotBelongToUser =
                todo.ListId is not null
                && !_context.TodoLists.Any(x => x.Id == todo.ListId && x.UserId == userId);
            if (isListnotBelongToUser)
                return RedirectToAction("Index");
            todo.UserId = userId;
            if (!ModelState.IsValid)
            {
                ViewData["Title"] = "Edit";
                ViewData["Action"] = "Update";
                AddModelsToViewData();
                return View("CreateUpdate", todo);
            }

            try
            {
                _context.Todos.Update(todo);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.Message);
                ViewData["Title"] = "Edit";
                ViewData["Action"] = "Update";
                AddModelsToViewData();
            }

            return View("CreateUpdate", todo);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            string userId = _userManager.GetUserId(User)!;
            Todo? todo = _context.Todos.Find(id);
            if (todo is null || todo.UserId != userId)
                return RedirectToAction("Index");
            try
            {
                _context.Todos.Remove(todo!);
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
