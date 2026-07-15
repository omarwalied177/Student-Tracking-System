using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace StudentTrackingSystem.PL.DTOs
{
    public class SubmitAssignmentDTO
    {
        [Required]
        public int AssignmentId { get; set; }

        [Display(Name = "ملف الواجب")]
        public IFormFile? SubmissionFile { get; set; }
    }
} 