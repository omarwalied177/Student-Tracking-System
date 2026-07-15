using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StudentTrackingSystem.DAL.Models;

namespace StudentTrackingSystem.DAL.Data.Models
{
    public class StudentAssignment
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Assignment")]
        public int AssignmentId { get; set; }
        public Assignment? Assignment { get; set; }

        [ForeignKey("Student")]
        public string? StudentId { get; set; }
        public AppUser? Student { get; set; }

        public string? SubmittedFileUrl { get; set; }

        public DateTime? SubmissionDate { get; set; }

        public int? ObtainedMarks { get; set; }

        public bool IsSubmitted { get; set; } = false;

        public bool IsGraded { get; set; } = false;
    }
} 