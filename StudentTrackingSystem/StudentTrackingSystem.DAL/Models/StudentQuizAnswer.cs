using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentTrackingSystem.DAL.Models
{
    public class StudentQuizAnswer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("QuizQuestion")]
        public int QuestionId { get; set; }
        public QuizQuestion? Question { get; set; }

        [Required]
        [ForeignKey("StudentQuizSubmission")]
        public int SubmissionId { get; set; }
        public StudentQuizSubmission? Submission { get; set; }

        // For multiple choice questions
        public int? SelectedOption { get; set; }

        // For essay questions
        public string? EssayAnswer { get; set; }

        // For essay questions grading
        public int? ObtainedMarks { get; set; }
    }
} 