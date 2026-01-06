using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourismMVC.Data;
using TourismMVC.Models;

namespace TourismMVC.Controllers
{
    [Authorize]
    public class TouristController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TouristController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Tourist/Index (My Profile)
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var profile = await _context.TouristProfiles
                .FirstOrDefaultAsync(p => p.ApplicationUserId == user.Id);

            if (profile == null)
            {
                return RedirectToAction(nameof(Create));
            }

            return View(profile);
        }

        // GET: Tourist/Create
        public IActionResult Create()
        {
            // Check if profile already exists
            var userId = _userManager.GetUserId(User);
            if (_context.TouristProfiles.Any(p => p.ApplicationUserId == userId))
            {
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        // POST: Tourist/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FullName,Nationality,PhoneNumber")] TouristProfile touristProfile)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            // Enforce Tourist user type
            if (user.UserType != "Tourist")
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                touristProfile.ApplicationUserId = user.Id;
                _context.Add(touristProfile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(touristProfile);
        }

        // GET: Tourist/Edit
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var profile = await _context.TouristProfiles
                .FirstOrDefaultAsync(p => p.ApplicationUserId == user.Id);

            if (profile == null)
            {
                return RedirectToAction(nameof(Create));
            }

            return View(profile);
        }

        // POST: Tourist/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,Nationality,PhoneNumber,ApplicationUserId")] TouristProfile touristProfile)
        {
            if (id != touristProfile.Id)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            // Ensure user owns this profile
            if (touristProfile.ApplicationUserId != user.Id)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(touristProfile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.TouristProfiles.Any(e => e.Id == touristProfile.Id))
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
            return View(touristProfile);
        }
    }
}
