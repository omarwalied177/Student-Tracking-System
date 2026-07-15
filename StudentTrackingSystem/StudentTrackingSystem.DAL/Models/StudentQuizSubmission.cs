using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentTrackingSystem.DAL.Models
{
    public class StudentQuizSubmission
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Quiz")]
        public int QuizId { get; set; }
        public Quiz? Quiz { get; set; }

        [Required]
        [ForeignKey("Student")]
        public string StudentId { get; set; } = string.Empty;
        public AppUser? Student { get; set; }

        [Required]
        [MaxLength(100)]
        public string StudentName { get; set; } = string.Empty;

        public DateTime SubmissionTime { get; set; }

        public int? ObtainedMarks { get; set; }

        public bool IsSubmitted { get; set; } = false;

        public bool IsGraded { get; set; } = false;

        // Navigation property for answers
        public ICollection<StudentQuizAnswer> Answers { get; set; } = new List<StudentQuizAnswer>();
    }
} 