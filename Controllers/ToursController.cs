using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TourismMVC.Data;
using TourismMVC.Models;

namespace TourismMVC.Controllers
{
    public class ToursController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ToursController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // AGENCY SECTION (Manage Tours)
        // ==========================================

        // GET: Tours (Lists only YOUR tours) - For Agency Dashboard
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Verify if user is Agency? For now assume yes or just show their tours.
            return View(await _context.TourPackages.Where(t => t.AgencyId == userId).ToListAsync());
        }

        // GET: Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken] // Core uses this, not just ValidateInput
        [Authorize]
        public async Task<IActionResult> Create(TourPackage tourPackage)
        {
            // Assign logged-in user as owner
            tourPackage.AgencyId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (ModelState.IsValid)
            {
                _context.Add(tourPackage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tourPackage);
        }

        // GET: Edit
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var tour = await _context.TourPackages.FirstOrDefaultAsync(t => t.Id == id);
            
            if (tour == null) return NotFound();
            
            // Security: Ensure only owner can edit
            if (tour.AgencyId != userId) return Forbid();

            return View(tour);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, TourPackage tourPackage)
        {
            if (id != tourPackage.Id) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            // Security check before update (though detached entity makes this tricky, let's just check Id ownership normally or trust the bind if we re-fetch)
            // Better to fetch and update properties for security, but keeping it simple for now as per "merge" request.
            // We must ensure AgencyId is preserved or verified.
            
            var existingTour = await _context.TourPackages.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
            if (existingTour == null) return NotFound();
            if (existingTour.AgencyId != userId) return Forbid();

            // Ensure AgencyId doesn't change
            tourPackage.AgencyId = userId; 

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tourPackage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TourExists(tourPackage.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tourPackage);
        }

        // GET: Tours/AgencyBookings/5
        [Authorize]
        public async Task<IActionResult> AgencyBookings(int? id)
        {
            if (id == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var tour = await _context.TourPackages.FirstOrDefaultAsync(t => t.Id == id);

            if (tour == null) return NotFound();
            if (tour.AgencyId != userId) return Forbid(); // Ensure ownership

            // Load bookings for this tour
            var bookings = await _context.Bookings
                .Where(b => b.TourPackageId == id)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            ViewBag.TourName = tour.Name;
            return View(bookings);
        }

        // ==========================================
        // PUBLIC SECTION (Browse & Book)
        // ==========================================

        // GET: Tours/Browse (Public)
        public async Task<IActionResult> Browse(string location = "", string difficulty = "")
        {
            var tours = _context.TourPackages.AsQueryable();

            if (!string.IsNullOrEmpty(location))
                tours = tours.Where(t => t.Location.Contains(location));

            if (!string.IsNullOrEmpty(difficulty))
                tours = tours.Where(t => t.Difficulty == difficulty);

            // Populate lists for dropdowns or filters
            ViewBag.Locations = await _context.TourPackages.Select(t => t.Location).Distinct().ToListAsync();
            ViewBag.Difficulties = await _context.TourPackages.Select(t => t.Difficulty).Distinct().ToListAsync();

            return View(await tours.ToListAsync());
        }

        // GET: Tours/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var tour = await _context.TourPackages.FirstOrDefaultAsync(m => m.Id == id);
            if (tour == null) return NotFound();
            
            // Load Feedbacks for this view (using ViewBag for simplicity or could ViewModel)
            ViewBag.Feedbacks = await _context.Feedbacks
                .Where(f => f.TourPackageId == id)
                .OrderByDescending(f => f.DatePosted)
                .ToListAsync();

            return View(tour);
        }

        // GET: Tours/Book/5
        [Authorize] // Must be logged in to book
        public async Task<IActionResult> Book(int? id)
        {
            if (id == null) return NotFound();

            var tour = await _context.TourPackages.FindAsync(id);
            if (tour == null)
            {
                TempData["ErrorMessage"] = "Tour not found.";
                return RedirectToAction(nameof(Browse));
            }

            // Pre-fill booking info
            var booking = new Booking
            {
                TourPackageId = tour.Id,
                NumberOfPeople = 1,
                CustomerName = User.Identity?.Name ?? "", // Attempt to prefill
                CustomerEmail = User.Identity?.Name ?? "" 
            };
            
            // If we have a TouristProfile, we could prefill better, but keeping it simple.

            ViewBag.Tour = tour;
            return View(booking);
        }

        // POST: Tours/Book
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Book(Booking booking)
        {
            // Re-fetch tour to calc price and checking existence
             var tour = await _context.TourPackages.FindAsync(booking.TourPackageId);
            
            if (tour == null) return NotFound();

            // Manually re-validate model state for TourPackageId (if needed) or just trust flow
            // Note: ModelState might be invalid if TourPackage navigation prop is missing but required?
            // "TourPackage" nav prop is virtual, validation shouldn't block, but let's see.
            // We'll remove the Navigation Property validation error if it occurs because we don't bind it from form
            ModelState.Remove("TourPackage");

            if (ModelState.IsValid)
            {
                booking.BookingDate = DateTime.Now;
                booking.TotalPrice = tour.Price * booking.NumberOfPeople;
                booking.Status = "Confirmed";
                
                // Save
                _context.Add(booking);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Booking confirmed! Ref: #{booking.Id}";
                return RedirectToAction("MyBookings", "Bookings");
            }

            ViewBag.Tour = tour;
            return View(booking);
        }

        // GET: Tours/LeaveFeedback/5
        [Authorize]
        public async Task<IActionResult> LeaveFeedback(int? id)
        {
            if (id == null) return NotFound();

            var tour = await _context.TourPackages.FindAsync(id);
            if (tour == null) return NotFound();

            var feedback = new Feedback
            {
                TourPackageId = tour.Id,
                TouristName = User.Identity?.Name ?? "Anonymous"
            };

            ViewBag.TourName = tour.Name;
            return View(feedback);
        }

        // POST: Tours/LeaveFeedback
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> LeaveFeedback(Feedback feedback)
        {
            if (ModelState.IsValid)
            {
                feedback.DatePosted = DateTime.Now;
                _context.Add(feedback);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Thank you for your feedback!";
                return RedirectToAction("Details", new { id = feedback.TourPackageId });
            }
            
            // Reload tour name if validation fails
            var tour = await _context.TourPackages.FindAsync(feedback.TourPackageId);
            ViewBag.TourName = tour?.Name;
            
            return View(feedback);
        }

        private bool TourExists(int id)
        {
            return _context.TourPackages.Any(e => e.Id == id);
        }
    }
}
