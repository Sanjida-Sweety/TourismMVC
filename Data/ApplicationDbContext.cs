using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TourismMVC.Models;

namespace TourismMVC.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    // The constructor must pass options to the base class
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // These create the actual tables in your database
    public DbSet<TouristProfile> TouristProfiles { get; set; }
    public DbSet<AgencyProfile> AgencyProfiles { get; set; }
    public DbSet<TourPackage> TourPackages { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Feedback> Feedbacks { get; set; }
}

