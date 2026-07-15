using System;

namespace StudentTrackingSystem.Pl.DTOs
{
    public class AttendanceDTO
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string? StudentName { get; set; }
        public string? Grade { get; set; }
        public DateTime Date { get; set; }
        public bool IsPresent { get; set; }
    }
} 