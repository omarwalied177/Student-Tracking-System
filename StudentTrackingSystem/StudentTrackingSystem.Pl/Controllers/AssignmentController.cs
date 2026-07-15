using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentTrackingSystem.DAL.Data;
using StudentTrackingSystem.DAL.Data.Models;
using StudentTrackingSystem.Pl.DTOs;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using StudentTrackingSystem.DAL.Data.Contexts;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace StudentTrackingSystem.Pl.Controllers
{
    [Authorize]
    public class AssignmentController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<AssignmentController> _logger;

        public AssignmentController(AppDbContext context, IWebHostEnvironment webHostEnvironment, ILogger<AssignmentController> logger)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }
        [Authorize(Roles = "Teatcher,Admin,Student")]
        public async Task<IActionResult> Index()
        {
           

            return View();
        }
        [Authorize(Roles = "Teatcher,Admin")]
        public async Task<IActionResult> IndexTeacher()
        {


            return View();
        }

        [Authorize(Roles = "Teatcher,Admin")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Grades = await _context.Grades
                .Select(g => new SelectListItem
                {
                    Value = g.Id.ToString(),
                    Text = g.Name
                })
                .ToListAsync();

            ViewBag.Subjects = await _context.Subjects
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                })
                .ToListAsync();

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Teatcher,Admin")]
        public async Task<IActionResult> Create(CreateAssignmentDTO model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Grades = await _context.Grades
                    .Select(g => new SelectListItem
                    {
                        Value = g.Id.ToString(),
                        Text = g.Name
                    })
                    .ToListAsync();

                ViewBag.Subjects = await _context.Subjects
                    .Select(s => new SelectListItem
                    {
                        Value = s.Id.ToString(),
                        Text = s.Name
                    })
                    .ToListAsync();

                return View(model);
            }

            var assignment = new Assignment
            {
                Title = model.Title,
                Description = model.Description,
                GradeId = model.GradeId,
                SubjectId = model.SubjectId,
                DueDate = model.DueDate,
                TotalMarks = model.TotalMarks,
                TeacherId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty
            };

            if (model.AssignmentFile != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "assignments");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.AssignmentFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.AssignmentFile.CopyToAsync(fileStream);
                }

                assignment.FileUrl = uniqueFileName;
            }

            _context.Assignments.Add(assignment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(TeacherAssignments));
        }

        [Authorize(Roles = "Teatcher,Admin")]
        public async Task<IActionResult> TeacherAssignments()
        {
            var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var assignments = await _context.Assignments
            .Include(a => a.Grade)
            .Include(a => a.Subject)
            .Where(a => a.TeacherId == teacherId)
            .Select(a => new AssignmentDetailsDTO
            {
                Id = a.Id,
                Title = a.Title,
                Description = a.Description,
                Grade = a.Grade != null ? a.Grade.Name : string.Empty,
                Subject = a.Subject != null ? a.Subject.Name : string.Empty,
                DueDate = a.DueDate,
                TotalMarks = a.TotalMarks,
                FileUrl = a.FileUrl,
                CreatedAt = a.CreatedAt
            })

                .ToListAsync();

            return View(assignments);
        }

        [Authorize(Roles = "Student,Admin,Teatcher")]
        public async Task<IActionResult> StudentAssignments()
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var assignments = await _context.Assignments
                .Include(a => a.Teacher)
                .Include(a => a.Grade)
                .Include(a => a.Subject)
                .Select(a => new AssignmentDetailsDTO
                {
                    Id = a.Id,
                    Title = a.Title,
                    Description = a.Description,
                    Grade = a.Grade != null ? a.Grade.Name : string.Empty,
                    Subject = a.Subject != null ? a.Subject.Name : string.Empty,

                    DueDate = a.DueDate,
                    TotalMarks = a.TotalMarks,
                    FileUrl = a.FileUrl,
                    CreatedAt = a.CreatedAt,
                    TeacherName = a.Teacher != null ? a.Teacher.UserName : string.Empty,
                    IsSubmitted = _context.StudentAssignments
                        .Any(sa => sa.AssignmentId == a.Id && sa.StudentId == studentId && sa.IsSubmitted),
                    IsGraded = _context.StudentAssignments
                        .Any(sa => sa.AssignmentId == a.Id && sa.StudentId == studentId && sa.IsGraded),
                    ObtainedMarks = _context.StudentAssignments
                        .Where(sa => sa.AssignmentId == a.Id && sa.StudentId == studentId)
                        .Select(sa => sa.ObtainedMarks)
                        .FirstOrDefault(),
                    SubmittedFileUrl = _context.StudentAssignments
                        .Where(sa => sa.AssignmentId == a.Id && sa.StudentId == studentId)
                        .Select(sa => sa.SubmittedFileUrl)
                        .FirstOrDefault(),
                    SubmissionDate = _context.StudentAssignments
                        .Where(sa => sa.AssignmentId == a.Id && sa.StudentId == studentId)
                        .Select(sa => sa.SubmissionDate)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return View(assignments);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Submit(SubmitAssignmentDTO model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var submission = await _context.StudentAssignments
                    .FirstOrDefaultAsync(sa => sa.AssignmentId == model.AssignmentId && sa.StudentId == studentId);

                if (submission == null)
                {
                    submission = new StudentAssignment
                    {
                        AssignmentId = model.AssignmentId,
                        StudentId = studentId ?? string.Empty,
                        SubmissionDate = DateTime.Now,
                        IsSubmitted = true
                    };
                    _context.StudentAssignments.Add(submission);
                }
                else
                {
                    submission.SubmissionDate = DateTime.Now;
                    submission.IsSubmitted = true;
                }

                if (model.SubmissionFile != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "submissions");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.SubmissionFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.SubmissionFile.CopyToAsync(fileStream);
                    }

                    submission.SubmittedFileUrl = uniqueFileName;
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "تم تسليم الواجب بنجاح";
                return RedirectToAction(nameof(StudentAssignments));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting assignment");
                ModelState.AddModelError("", "حدث خطأ أثناء تسليم الواجب");
                return View(model);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Teatcher,Admin")]
        public async Task<IActionResult> Grade(GradeAssignmentDTO model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction(nameof(TeacherAssignments));

            var submission = await _context.StudentAssignments.FindAsync(model.StudentAssignmentId);
            if (submission == null)
                return NotFound();

            submission.ObtainedMarks = model.ObtainedMarks;
            submission.IsGraded = true;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(TeacherAssignments));
        }

        [Authorize(Roles = "Teatcher,Admin")]
        public async Task<IActionResult> Submissions(int id)
        {
            var submissions = await _context.StudentAssignments
                .Include(sa => sa.Student)
                .Include(sa => sa.Assignment)
                .Where(sa => sa.AssignmentId == id)
                .ToListAsync();

            return View(submissions);
        }
    }
} 