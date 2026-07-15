using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentTrackingSystem.DAL.Models
{
    public class Parent : BaseEntity
    {
        // Removed duplicate Id property

        [Required]
        [MaxLength(35)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^(010|011|012|015)\d{8}$", ErrorMessage = "Invalid phone number.")]
        public string PhoneNo { get; set; } = string.Empty;

        // علاقة مع الطالب
        public ICollection<Student> Students { get; set; } = new List<Student>();
    }
}
