using System;

namespace StudentTrackingSystem.Pl.DTOs
{
    public class StudentAttendanceStatsDTO
    {
        public int StudentId { get; set; }
        public string? StudentName { get; set; }
        public string? Grade { get; set; }
        public int TotalDays { get; set; }
        public int PresentDays { get; set; }
        public int AbsentDays { get; set; }
        public double AttendancePercentage => TotalDays > 0 ? (PresentDays * 100.0 / TotalDays) : 0;
    }
} 