using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourismMVC.Models
{
    public class Feedback
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TourPackageId { get; set; }

        [Required]
        public string TouristName { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5 stars.")]
        public int Rating { get; set; }

        [Required]
        [StringLength(1000)]
        public string Comment { get; set; }

        public DateTime DatePosted { get; set; } = DateTime.Now;

        [ForeignKey("TourPackageId")]
        public virtual TourPackage TourPackage { get; set; }
    }
}
