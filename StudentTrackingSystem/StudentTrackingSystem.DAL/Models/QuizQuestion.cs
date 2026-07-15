using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentTrackingSystem.DAL.Models
{
    public enum QuestionType
    {
        MultipleChoice,
        Essay
    }

    public class QuizQuestion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string QuestionText { get; set; } = string.Empty;

        [Required]
        public QuestionType QuestionType { get; set; }

        // For multiple choice questions
        public string? Option1 { get; set; }
        public string? Option2 { get; set; }
        public string? Option3 { get; set; }
        public string? Option4 { get; set; }
        public int? CorrectOption { get; set; } // 1-4 for multiple choice

        [Required]
        [ForeignKey("Quiz")]
        public int QuizId { get; set; }
        public Quiz? Quiz { get; set; }
    }
} 