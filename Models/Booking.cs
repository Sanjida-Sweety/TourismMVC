using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourismMVC.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TourPackageId { get; set; } // Foreign Key to TourPackage

        [Required]
        public string CustomerName { get; set; }

        [Required]
        [EmailAddress]
        public string CustomerEmail { get; set; }

        [Required]
        public DateTime BookingDate { get; set; }

        [Required]
        [Range(1, 10)]
        public int NumberOfPeople { get; set; }

        [Required]
        public decimal TotalPrice { get; set; }

        [Required]
        public string Status { get; set; } = "Pending";

        // Navigation property
        [ForeignKey("TourPackageId")]
        public virtual TourPackage TourPackage { get; set; }
    }
}
