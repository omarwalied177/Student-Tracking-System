using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace StudentTrackingSystem.Pl.DTOs
{
    public class CreateAssignmentDTO
    {
        [Required]
        public string? Title { get; set; }

        [Required]
        public string? Description { get; set; }

        [Required]
        [Display(Name = "Grade")]
        public int GradeId { get; set; }

        [Required]
        [Display(Name = "Subject")]
        public int SubjectId { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        public int TotalMarks { get; set; }

        public IFormFile? AssignmentFile { get; set; }
    }

    public class AssignmentDetailsDTO
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Grade { get; set; }
        public string? Subject { get; set; }
        public DateTime DueDate { get; set; }
        public int TotalMarks { get; set; }
        public string? FileUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? TeacherName { get; set; }
        public bool IsSubmitted { get; set; }
        public bool IsGraded { get; set; }
        public int? ObtainedMarks { get; set; }
        public string? SubmittedFileUrl { get; set; }
        public DateTime? SubmissionDate { get; set; }
    }

    public class SubmitAssignmentDTO
    {
        [Required]
        public int AssignmentId { get; set; }

        [Required]
        public IFormFile? SubmissionFile { get; set; }
    }

    public class GradeAssignmentDTO
    {
        [Required]
        public int StudentAssignmentId { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int ObtainedMarks { get; set; }
    }
} 