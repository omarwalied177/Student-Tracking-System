using System;
using System.ComponentModel.DataAnnotations;

namespace StudentTrackingSystem.DAL.Models
{
    public class Teatcher : BaseEntity
    {
        // Removed duplicate Id property

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        public string Gender { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        // Foreign Key
        public int SubjectId { get; set; }

        // Navigation Property
        public Subject? Subject { get; set; }

        public string? ImagePath { get; set; }
    }
}
