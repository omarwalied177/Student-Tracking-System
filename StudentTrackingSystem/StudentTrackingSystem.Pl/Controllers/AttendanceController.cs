using Microsoft.AspNetCore.Mvc;
using StudentTrackingSystem.BLL.Interfaces;
using StudentTrackingSystem.DAL.Models;
using StudentTrackingSystem.Pl.DTOs;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace StudentTrackingSystem.Pl.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly IStudentRepository _studentRepo;
        private readonly IGenaricRepository<Attendance> _attendanceRepo;
        private readonly IGradeRepository _gradeRepo;
        private readonly IUnitOfWork _unitOfWork;

        public AttendanceController(
            IStudentRepository studentRepo,
            IGenaricRepository<Attendance> attendanceRepo,
            IGradeRepository gradeRepo,
            IUnitOfWork unitOfWork)
        {
            _studentRepo = studentRepo;
            _attendanceRepo = attendanceRepo;
            _gradeRepo = gradeRepo;
            _unitOfWork = unitOfWork;
        }
        
        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);

            // Get all students
            var totalStudents = (await _studentRepo.GetAllAsync()).ToList();
            var totalStudentsCount = totalStudents.Count;

            // Get today's attendance
            var todaysAttendance = (await _attendanceRepo.GetAllAsync())
                .Where(a => a.Date.Date == today)
                .ToList();

            // Calculate today's statistics
            var presentToday = todaysAttendance.Count(a => a.IsPresent);
            var absentToday = todaysAttendance.Count(a => !a.IsPresent);
            var notMarkedToday = totalStudentsCount - (presentToday + absentToday);

            ViewBag.TodayAttendance = totalStudentsCount;
            ViewBag.PresentToday = presentToday;
            ViewBag.AbsentToday = absentToday;
            ViewBag.NotMarkedToday = notMarkedToday;

            // Calculate monthly average
            var monthlyAttendance = (await _attendanceRepo.GetAllAsync())
                .Where(a => a.Date >= startOfMonth && a.Date <= today)
                .ToList();
            
            // Calculate total possible attendance days for the month
            var schoolDays = 0;
            for (var date = startOfMonth; date <= today; date = date.AddDays(1))
            {
                // Skip weekends (Saturday and Sunday)
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                {
                    schoolDays++;
                }
            }

            var totalPossibleAttendance = totalStudentsCount * schoolDays;
            var actualAttendance = monthlyAttendance.Count(a => a.IsPresent);
            
            ViewBag.MonthlyAverage = totalPossibleAttendance > 0 
                ? Math.Round((double)actualAttendance / totalPossibleAttendance * 100, 1) 
                : 0;

            return View();
        }

        // الصفحة لاظهار الطلاب اللي هيتم تسجيل حضورهم (فقط الطلاب اللي لم يؤخذ لهم حضور اليوم)
        [Authorize(Roles = "Admin,Teatcher")]
        public async Task<IActionResult> AddAttendance(string? grade = null)
        {
            var today = DateTime.Today;

            var allStudents = await _studentRepo.GetAllAsync();

            // نجيب حضور اليوم لكل الطلاب
            var todaysAttendance = await _attendanceRepo.GetAllAsync();
            var todaysAttendanceForStudents = todaysAttendance
                .Where(a => a.Date.Date == today)
                .Select(a => a.StudentId)
                .ToList();

            // نجيب الطلاب اللي ماخدوش حضور النهاردة (يعني اللي مش موجودين في حضور اليوم)
            var studentsToTakeAttendance = allStudents
                .Where(s => !todaysAttendanceForStudents.Contains(s.Id))
                .ToList();

            if (!string.IsNullOrEmpty(grade))
            {
                studentsToTakeAttendance = studentsToTakeAttendance
                    .Where(s => s.Grade == grade)
                    .ToList();
            }

            var dtoList = studentsToTakeAttendance.Select(s => new AttendanceDTO
            {
                StudentId = s.Id,
                StudentName = s.FullName,
                Grade = s.Grade,
                Date = today
            }).ToList();

            ViewBag.Grades = (await _gradeRepo.GetAllAsync()).ToList();

            return View(dtoList);
        }
        [Authorize(Roles = "Admin,Teatcher")]
        // اضافة حضور جديد
        [HttpPost]
        public async Task<IActionResult> MarkAttendance(int studentId, bool isPresent)
        {
            var attendance = new Attendance
            {
                StudentId = studentId,
                Date = DateTime.Today,
                IsPresent = isPresent
            };

            await _attendanceRepo.AddAsync(attendance);
            await _unitOfWork.CompleteAsync();

            return Json(new { success = true, attendanceId = studentId }); // برجّع StudentId للتمييز
        }

        // عرض كل الحضور مع فلترة الدرجة
        [Authorize(Roles = "Admin,Teatcher")]
        public async Task<IActionResult> ShowAttendance(string? grade = null)
        {
            var attendances = await _attendanceRepo.GetAllAsync();
            var students = await _studentRepo.GetAllAsync();

            var attendanceDTOs = (from a in attendances
                                  join s in students on a.StudentId equals s.Id
                                  select new AttendanceDTO
                                  {
                                      Id = a.Id,
                                      StudentId = s.Id,
                                      StudentName = s.FullName,
                                      Grade = s.Grade,
                                      Date = a.Date,
                                      IsPresent = a.IsPresent
                                  }).ToList();

            if (!string.IsNullOrEmpty(grade))
            {
                attendanceDTOs = attendanceDTOs.Where(a => a.Grade == grade).ToList();
            }

            ViewBag.Grades = (await _gradeRepo.GetAllAsync()).ToList();

            return View(attendanceDTOs);
        }
        [Authorize(Roles = "Admin,Teatcher")]
        public async Task<IActionResult> BatchAttendance(string? grade = null)
        {
            var today = DateTime.Today;
            var allStudents = await _studentRepo.GetAllAsync();

            // Get today's attendance records
            var todaysAttendance = await _attendanceRepo.GetAllAsync();
            var todaysAttendanceForStudents = todaysAttendance
                .Where(a => a.Date.Date == today)
                .Select(a => a.StudentId)
                .ToList();

            // Get students who haven't had attendance marked today
            var studentsToTakeAttendance = allStudents
                .Where(s => !todaysAttendanceForStudents.Contains(s.Id))
                .ToList();

            if (!string.IsNullOrEmpty(grade))
            {
                studentsToTakeAttendance = studentsToTakeAttendance
                    .Where(s => s.Grade == grade)
                    .ToList();
            }

            var batchAttendance = new BatchAttendanceDTO
            {
                Grade = grade,
                Date = today,
                Students = studentsToTakeAttendance.Select(s => new StudentAttendanceDTO
                {
                    StudentId = s.Id,
                    StudentName = s.FullName,
                    Grade = s.Grade,
                    IsPresent = true // Default to present
                }).ToList()
            };

            ViewBag.Grades = (await _gradeRepo.GetAllAsync()).ToList();
            return View(batchAttendance);
        }
        [Authorize(Roles = "Admin,Teatcher")]
        [HttpPost]
        public async Task<IActionResult> SaveBatchAttendance(BatchAttendanceDTO model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Grades = (await _gradeRepo.GetAllAsync()).ToList();
                return View("BatchAttendance", model);
            }

            var attendances = model.Students.Select(s => new Attendance
            {
                StudentId = s.StudentId,
                Date = model.Date,
                IsPresent = s.IsPresent
            }).ToList();

            foreach (var attendance in attendances)
            {
                await _attendanceRepo.AddAsync(attendance);
            }

            await _unitOfWork.CompleteAsync();

            TempData["SuccessMessage"] = "Attendance has been saved successfully!";
            return RedirectToAction(nameof(ShowAttendance), new { grade = model.Grade });
        }
        [Authorize(Roles = "Admin,Teatcher")]
        public async Task<IActionResult> AttendanceStats(string? grade = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var students = await _studentRepo.GetAllAsync();
            var attendances = await _attendanceRepo.GetAllAsync();

            // Set default date range if not provided
            startDate ??= DateTime.Today.AddMonths(-1);
            endDate ??= DateTime.Today;

            // Filter attendances by date range
            var filteredAttendances = attendances
                .Where(a => a.Date.Date >= startDate.Value.Date && a.Date.Date <= endDate.Value.Date)
                .ToList();

            var stats = new List<StudentAttendanceStatsDTO>();

            foreach (var student in students)
            {
                if (!string.IsNullOrEmpty(grade) && student.Grade != grade)
                    continue;

                var studentAttendances = filteredAttendances
                    .Where(a => a.StudentId == student.Id)
                    .ToList();

                var presentDays = studentAttendances.Count(a => a.IsPresent);
                var absentDays = studentAttendances.Count(a => !a.IsPresent);

                stats.Add(new StudentAttendanceStatsDTO
                {
                    StudentId = student.Id,
                    StudentName = student.FullName,
                    Grade = student.Grade,
                    TotalDays = studentAttendances.Count,
                    PresentDays = presentDays,
                    AbsentDays = absentDays
                });
            }

            ViewBag.Grades = (await _gradeRepo.GetAllAsync()).ToList();
            ViewBag.StartDate = startDate.Value.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate.Value.ToString("yyyy-MM-dd");

            return View(stats);
        }
    }
}
