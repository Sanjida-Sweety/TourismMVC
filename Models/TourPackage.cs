using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourismMVC.Models
{
    public class TourPackage
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Package Name is required.")]
        [StringLength(100)]
        [Display(Name = "Tour Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please provide a description.")]
        public string Description { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        [Range(0.01, 100000.00, ErrorMessage = "Price must be greater than 0.")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Required]
        [Range(1, 365, ErrorMessage = "Duration must be at least 1 day.")]
        [Display(Name = "Duration (Days)")]
        public int DurationDays { get; set; }

        public string Difficulty { get; set; } // Easy, Medium, Hard

        // Validation: Custom check for future dates
        [Required]
        [DataType(DataType.Date)]
        [FutureDate(ErrorMessage = "Tour date cannot be in the past.")]
        public DateTime StartDate { get; set; }

        [Required]
        [Range(1, 500)]
        [Display(Name = "Max Group Size")]
        public int MaxGroupSize { get; set; }

        [Display(Name = "Image URL")]
        public string ImageUrl { get; set; }

        // Tracks which Agency owns this package (matches TourismApp logic)
        public string AgencyId { get; set; }
        
        // Navigation properties if needed
        // public virtual AgencyProfile Agency { get; set; }
    }

    // Custom Validator Logic
    public class FutureDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is DateTime dateTime)
            {
                return dateTime >= DateTime.Today;
            }
            return true;
        }
    }
}
