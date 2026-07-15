using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StudentTrackingSystem.DAL.Models;

namespace StudentTrackingSystem.DAL.Data.Models
{
    public class Assignment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? Title { get; set; }

        [Required]
        public string? Description { get; set; }

        [Required]
        [ForeignKey("Grade")]
        public int GradeId { get; set; }
        public Grade? Grade { get; set; }

        [Required]
        [ForeignKey("Subject")]
        public int SubjectId { get; set; }
        public Subject? Subject { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        public int TotalMarks { get; set; }

        public string? FileUrl { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("Teacher")]
        public string? TeacherId { get; set; }
        public AppUser? Teacher { get; set; }
    }
} 