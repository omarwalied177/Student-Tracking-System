using System;
using System.ComponentModel.DataAnnotations;
using StudentTrackingSystem.DAL.Models;

namespace StudentTrackingSystem.Pl.DTOs
{
    public class CreateQuizDTO
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "Quiz Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Grade")]
        public int GradeId { get; set; }

        [Required]
        [Display(Name = "Subject")]
        public int SubjectId { get; set; }

        [Required]
        [Display(Name = "Total Marks")]
        [Range(1, 100)]
        public int TotalMarks { get; set; }

        [Required]
        [Display(Name = "Start Time")]
        public DateTime StartTime { get; set; }

        [Required]
        [Display(Name = "End Time")]
        public DateTime EndTime { get; set; }

        public List<CreateQuizQuestionDTO> Questions { get; set; } = new List<CreateQuizQuestionDTO>();
    }

    public class CreateQuizQuestionDTO
    {
        [Required]
        [Display(Name = "Question Text")]
        public string QuestionText { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Question Type")]
        public QuestionType QuestionType { get; set; }

        [Display(Name = "Option 1")]
        public string? Option1 { get; set; }

        [Display(Name = "Option 2")]
        public string? Option2 { get; set; }

        [Display(Name = "Option 3")]
        public string? Option3 { get; set; }

        [Display(Name = "Option 4")]
        public string? Option4 { get; set; }

        [Display(Name = "Correct Option")]
        public int? CorrectOption { get; set; }
    }

    public class QuizDetailsDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public int TotalMarks { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsActive { get; set; }
        public bool IsSubmitted { get; set; }
        public string TeacherName { get; set; } = string.Empty;
        public List<QuizQuestionDTO> Questions { get; set; } = new List<QuizQuestionDTO>();
    }

    public class QuizQuestionDTO
    {
        public int Id { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public QuestionType QuestionType { get; set; }
        public string? Option1 { get; set; }
        public string? Option2 { get; set; }
        public string? Option3 { get; set; }
        public string? Option4 { get; set; }
        public int? CorrectOption { get; set; }
    }

    public class StudentQuizAnswerDTO
    {
        public int QuestionId { get; set; }
        public int? SelectedOption { get; set; }
        public string? EssayAnswer { get; set; }
    }
} 