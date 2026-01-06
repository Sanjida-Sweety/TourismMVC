using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace TourismMVC.Models
{
    public class AgencyProfile
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Agency Name")]
        public string AgencyName { get; set; }

        public string LicenseNumber { get; set; }

        public string Address { get; set; }

        [EmailAddress]
        public string BusinessEmail { get; set; }

        [Display(Name = "License Document")]
        public string LicenseDocumentPath { get; set; }

        [NotMapped]
        [Display(Name = "Upload License")]
        public IFormFile LicenseFile { get; set; }

        // Link to the Identity User (Login account)
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
