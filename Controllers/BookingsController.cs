using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TourismMVC.Data;
using TourismMVC.Models;

namespace TourismMVC.Controllers
{
    [Authorize]
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Bookings/MyBookings
        public async Task<IActionResult> MyBookings()
        {
            var userEmail = User.Identity?.Name;
            if (string.IsNullOrEmpty(userEmail)) return RedirectToAction("Index", "Home");

            // Fetch bookings for this user
            var bookings = await _context.Bookings
                .Include(b => b.TourPackage)
                .Where(b => b.CustomerEmail == userEmail)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            // Separate upcoming and past trips
            var upcoming = bookings.Where(b => b.TourPackage.StartDate >= DateTime.Now).ToList();
            var past = bookings.Where(b => b.TourPackage.StartDate < DateTime.Now).ToList();

            ViewBag.UpcomingBookings = upcoming;
            ViewBag.PastBookings = past;

            return View(bookings);
        }
    }
}
