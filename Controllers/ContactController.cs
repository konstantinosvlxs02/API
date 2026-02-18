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
    public class ContactController : Controller
    {
        private readonly Database _context;

        public ContactController(Database context)
        {
            _context = context;
        }

        // GET: Contact
        public async Task<IActionResult> Index()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var contacts = await _context.Contact
                .Where(c => c.UserId == userId)
                .Include(c => c.ContactUser)
                .ToListAsync();

            ViewBag.AllUsers = await _context.Users
                .Where(u => u.Id != userId)
                .ToListAsync();

            return View(contacts);
        }

        // POST: Contact/Create
        [HttpPost]
        public async Task<IActionResult> Create(int contactUserId, string contactName)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            if (userId == contactUserId)
            {
                TempData["Error"] = "Δεν μπορείτε να προσθέσετε τον εαυτό σας.";
                return RedirectToAction("Index");
            }

            var existing = await _context.Contact
                .AnyAsync(c => c.UserId == userId && c.ContactUserId == contactUserId);

            if (existing)
            {
                TempData["Error"] = "Η επαφή υπάρχει ήδη.";
                return RedirectToAction("Index");
            }

            var contactUser = await _context.Users.FindAsync(contactUserId);
            if (contactUser == null)
            {
                TempData["Error"] = "Ο χρήστης δεν βρέθηκε.";
                return RedirectToAction("Index");
            }

            var contact = new Contact
            {
                UserId = userId,
                ContactUserId = contactUserId,
                Name = string.IsNullOrEmpty(contactName) ? contactUser.Username : contactName
            };

            _context.Contact.Add(contact);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Η επαφή '{contact.Name}' προστέθηκε!";
            return RedirectToAction("Index");
        }
    }
}