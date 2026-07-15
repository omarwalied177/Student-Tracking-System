using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace StudentTrackingSystem.DAL.Models
{
    public class Subject
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        // Navigation Property to Grade
        public int GradeId { get; set; }
        public Grade? Grade { get; set; }

        // Navigation Property
        public ICollection<Teatcher> Teatchers { get; set; } = new List<Teatcher>();
    }

}
