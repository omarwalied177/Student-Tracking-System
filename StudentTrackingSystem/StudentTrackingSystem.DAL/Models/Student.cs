using StudentTrackingSystem.DAL.Models;
using System.ComponentModel.DataAnnotations;

public class Student : BaseEntity
{
    // Remove duplicate Id property

    [Required]
    [MaxLength(35)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public DateTime DateOfBirth { get; set; }

    [Required]
    [EmailAddress]
    public string EmailAddress { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^(010|011|012|015)\d{8}$", ErrorMessage = "Invalid phone number.")]
    public string PhoneNo { get; set; } = string.Empty;

    public string? Address { get; set; }

    [Required]
    public string Grade { get; set; } = string.Empty;

    // Parent relationship
    public int? ParentId { get; set; }
    public Parent? Parent { get; set; }

    // Subject relationship
    public int? SubjectId { get; set; }
    public Subject? Subject { get; set; }

    // Image path for student
    public string? ImagePath { get; set; }

    // Password (commented out)
    // [Required]
    // [DataType(DataType.Password)]
    // [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
    // public string Password { get; set; }

    public string Gender { get; set; } = string.Empty;
}
