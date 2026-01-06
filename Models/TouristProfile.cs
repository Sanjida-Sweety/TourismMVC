using System.ComponentModel.DataAnnotations;

namespace TourismMVC.Models
{
    public class TouristProfile
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        public string Nationality { get; set; }

        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        // Link to the Identity User (Login account)
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
