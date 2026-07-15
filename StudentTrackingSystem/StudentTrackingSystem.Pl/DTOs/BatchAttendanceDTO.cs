using System;
using System.Collections.Generic;

namespace StudentTrackingSystem.Pl.DTOs
{
    public class BatchAttendanceDTO
    {
        public string? Grade { get; set; }
        public DateTime Date { get; set; }
        public List<StudentAttendanceDTO> Students { get; set; } = new List<StudentAttendanceDTO>();
    }

    public class StudentAttendanceDTO
    {
        public int StudentId { get; set; }
        public string? StudentName { get; set; }
        public string? Grade { get; set; }
        public bool IsPresent { get; set; }
    }
} 