using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentTrackingSystem.DAL.Models
{
    public class Attendance : BaseEntity
    {
        [ForeignKey("Student")]
        public int StudentId { get; set; }
        public virtual Student? Student { get; set; }

        public DateTime Date { get; set; }
        public bool IsPresent { get; set; }
    }
}
