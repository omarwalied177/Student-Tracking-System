using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StudentTrackingSystem.DAL.Models;

namespace StudentTrackingSystem.DAL.Models
{
    public class Quiz
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [ForeignKey("Grade")]
        public int GradeId { get; set; }
        public Grade? Grade { get; set; }

        [Required]
        [ForeignKey("Subject")]
        public int SubjectId { get; set; }
        public Subject? Subject { get; set; }

        [Required]
        public int TotalMarks { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        public bool IsActive { get; set; } = true;

        [Required]
        [ForeignKey("Teacher")]
        public string TeacherId { get; set; } = string.Empty;
        public AppUser? Teacher { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation property for questions
        public ICollection<QuizQuestion> Questions { get; set; } = new List<QuizQuestion>();
    }
} 