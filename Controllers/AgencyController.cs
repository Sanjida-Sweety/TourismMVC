using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourismMVC.Data;
using TourismMVC.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace TourismMVC.Controllers
{
    [Authorize]
    public class AgencyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _hostEnvironment;

        public AgencyController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Agency/Index (My Profile)
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var profile = await _context.AgencyProfiles
                .FirstOrDefaultAsync(p => p.ApplicationUserId == user.Id);

            if (profile == null)
            {
                return RedirectToAction(nameof(Create));
            }

            return View(profile);
        }

        // GET: Agency/Create
        public IActionResult Create()
        {
            // Check if profile already exists
            var userId = _userManager.GetUserId(User);
            if (_context.AgencyProfiles.Any(p => p.ApplicationUserId == userId))
            {
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        // POST: Agency/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AgencyName,LicenseNumber,Address,BusinessEmail")] AgencyProfile agencyProfile)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            // Enforce Agency user type
            if (user.UserType != "Agency")
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                agencyProfile.ApplicationUserId = user.Id;
                _context.Add(agencyProfile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(agencyProfile);
        }

        // GET: Agency/Edit
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var profile = await _context.AgencyProfiles
                .FirstOrDefaultAsync(p => p.ApplicationUserId == user.Id);

            if (profile == null)
            {
                return RedirectToAction(nameof(Create));
            }

            return View(profile);
        }

        // POST: Agency/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AgencyName,LicenseNumber,Address,BusinessEmail,ApplicationUserId,LicenseFile,LicenseDocumentPath")] AgencyProfile agencyProfile)
        {
            if (id != agencyProfile.Id)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            // Ensure user owns this profile
            if (agencyProfile.ApplicationUserId != user.Id)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Handle file upload
                    if (agencyProfile.LicenseFile != null)
                    {
                        string wwwRootPath = _hostEnvironment.WebRootPath;
                        string fileName = Path.GetFileNameWithoutExtension(agencyProfile.LicenseFile.FileName);
                        string extension = Path.GetExtension(agencyProfile.LicenseFile.FileName);
                        agencyProfile.LicenseDocumentPath = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                        string path = Path.Combine(wwwRootPath + "/uploads/agency_docs/", fileName);
                        
                        // Ensure directory exists
                        Directory.CreateDirectory(Path.GetDirectoryName(path));

                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await agencyProfile.LicenseFile.CopyToAsync(fileStream);
                        }
                    }

                    _context.Update(agencyProfile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.AgencyProfiles.Any(e => e.Id == agencyProfile.Id))
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
            return View(agencyProfile);
        }
    }
}
