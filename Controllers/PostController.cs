using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DataBase;
using Models;
using WebApplication1.Models;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class PostController : Controller
    {
        private readonly Database _context;

        public PostController(Database context)
        {
            _context = context;
        }

        // GET: Post
        public async Task<IActionResult> Index(string? search)
        {
            var query = _context.Posts.Include(p => p.User).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Title.Contains(search) || p.Content.Contains(search));
                ViewBag.Search = search;
            }

            var posts = await query.OrderByDescending(p => p.CreatedAt).ToListAsync();
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(posts);
        }

        // GET: Post/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Post/Create
        [HttpPost]
        public async Task<IActionResult> Create(string title, string content)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(content))
            {
                TempData["Error"] = "Ο τίτλος και το περιεχόμενο είναι υποχρεωτικά.";
                return View();
            }

            var post = new Post
            {
                Title = title,
                Content = content,
                UserId = userId
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Το post δημιουργήθηκε επιτυχώς!";
            return RedirectToAction("Index");
        }

        // GET: Post/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var post = await _context.Posts
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
                return NotFound();

            var categories = await _context.PostCategories
                .Where(pc => pc.PostId == id)
                .Include(pc => pc.Category)
                .Select(pc => pc.Category)
                .ToListAsync();

            ViewBag.PostCategories = categories;
            ViewBag.AllCategories = await _context.Categories.ToListAsync();
            return View(post);
        }

        // POST: Post/AddCategory
        [HttpPost]
        public async Task<IActionResult> AddCategory(int postId, int categoryId)
        {
            var exists = await _context.PostCategories
                .AnyAsync(pc => pc.PostId == postId && pc.CategoryId == categoryId);

            if (!exists)
            {
                _context.PostCategories.Add(new PostCategory
                {
                    PostId = postId,
                    CategoryId = categoryId
                });
                await _context.SaveChangesAsync();
                TempData["Success"] = "Η κατηγορία προστέθηκε!";
            }
            else
            {
                TempData["Error"] = "Η κατηγορία υπάρχει ήδη σε αυτό το post.";
            }

            return RedirectToAction("Details", new { id = postId });
        }

        // POST: Post/CreateCategory
        [HttpPost]
        public async Task<IActionResult> CreateCategory(string name, int? returnToPostId)
        {
            if (string.IsNullOrEmpty(name))
            {
                TempData["Error"] = "Το όνομα κατηγορίας είναι υποχρεωτικό.";
                return RedirectToAction("Index");
            }

            var category = new Category { Name = name };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Η κατηγορία '{name}' δημιουργήθηκε!";

            if (returnToPostId.HasValue)
                return RedirectToAction("Details", new { id = returnToPostId.Value });

            return RedirectToAction("Index");
        }

        // GET: Post/ByCategory/5
        public async Task<IActionResult> ByCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound();

            var posts = await _context.PostCategories
                .Where(pc => pc.CategoryId == id)
                .Include(pc => pc.Post)
                .ThenInclude(p => p.User)
                .Select(pc => pc.Post)
                .ToListAsync();

            ViewBag.CategoryName = category.Name;
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View("Index", posts);
        }
    }
}