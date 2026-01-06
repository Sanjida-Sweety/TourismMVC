using Microsoft.AspNetCore.Identity;

namespace TourismMVC.Models
{
    public class ApplicationUser : IdentityUser
    {
        // "Tourist" or "Agency"
        public string UserType { get; set; }
    }
}