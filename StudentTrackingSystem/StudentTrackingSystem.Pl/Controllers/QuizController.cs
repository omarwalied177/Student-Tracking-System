using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentTrackingSystem.DAL.Data.Contexts;
using StudentTrackingSystem.DAL.Models;
using StudentTrackingSystem.Pl.DTOs;
using System.Security.Claims;

namespace StudentTrackingSystem.Pl.Controllers
{
    [Authorize]
    public class QuizController : Controller
    {
        private readonly AppDbContext _context;

        public QuizController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> ShowForStudent()
        {

            return View();
        }


        [Authorize(Roles = "Teatcher,Admin")]
        public async Task<IActionResult> ShowForTeacher_Admin()
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

        #region MyRegion

        [HttpGet]
        [Authorize(Roles = "Teatcher,Admin")]
        public async Task<IActionResult> GetAllQuizzes()
        {
            var allQuizzes = await _context.Quizzes
                .Include(q => q.Grade)
                .Include(q => q.Subject)
                .Include(q => q.Teacher)
                .Include(q => q.Questions)
                .OrderByDescending(q => q.Id)
                .Select(q => new QuizDetailsDTO
                {
                    Id = q.Id,
                    Name = q.Name,
                    Grade = q.Grade!.Name,
                    Subject = q.Subject!.Name,
                    TotalMarks = q.TotalMarks,
                    StartTime = q.StartTime,
                    EndTime = q.EndTime,
                    IsActive = q.IsActive,
                    TeacherName = q.Teacher!.UserName ?? string.Empty,
                    // يمكنك إضافة معلومات أخرى حسب الحاجة
                })
                .ToListAsync();

            return View(allQuizzes);
        }

        // إذا كنت تريد إرجاع البيانات كـ JSON للـ API
        [HttpGet]
        [Authorize(Roles = "Teatcher,Admin")]
        public async Task<IActionResult> GetAllQuizzesJson()
        {
            var allQuizzes = await _context.Quizzes
                .Include(q => q.Grade)
                .Include(q => q.Subject)
                .Include(q => q.Teacher)
                .Include(q => q.Questions)
                .OrderByDescending(q => q.Id)
                .Select(q => new
                {
                    Id = q.Id,
                    Name = q.Name,
                    Grade = q.Grade!.Name,
                    Subject = q.Subject!.Name,
                    TotalMarks = q.TotalMarks,
                    StartTime = q.StartTime,
                    EndTime = q.EndTime,
                    IsActive = q.IsActive,
                    TeacherName = q.Teacher!.UserName ?? string.Empty,
                    QuestionsCount = q.Questions.Count(),
                    SubmissionsCount = _context.StudentQuizSubmissions.Count(s => s.QuizId == q.Id)
                })
                .ToListAsync();

            return Json(allQuizzes);
        }

        // إذا كنت تريد إضافة فلترة حسب المعايير المختلفة
        [HttpGet]
        [Authorize(Roles = "Teatcher,Admin")]
        public async Task<IActionResult> GetFilteredQuizzes(int? gradeId = null, int? subjectId = null, bool? isActive = null)
        {
            var query = _context.Quizzes
                .Include(q => q.Grade)
                .Include(q => q.Subject)
                .Include(q => q.Teacher)
                .Include(q => q.Questions)
                .AsQueryable();

            // تطبيق الفلاتر
            if (gradeId.HasValue)
                query = query.Where(q => q.GradeId == gradeId.Value);

            if (subjectId.HasValue)
                query = query.Where(q => q.SubjectId == subjectId.Value);

            if (isActive.HasValue)
                query = query.Where(q => q.IsActive == isActive.Value);

            var filteredQuizzes = await query
                .OrderByDescending(q => q.Id)
                .Select(q => new QuizDetailsDTO
                {
                    Id = q.Id,
                    Name = q.Name,
                    Grade = q.Grade!.Name,
                    Subject = q.Subject!.Name,
                    TotalMarks = q.TotalMarks,
                    StartTime = q.StartTime,
                    EndTime = q.EndTime,
                    IsActive = q.IsActive,
                    TeacherName = q.Teacher!.UserName ?? string.Empty,
                })
                .ToListAsync();

            return View(filteredQuizzes);
        }
        #endregion




        [HttpPost]
        [Authorize(Roles = "Teatcher,Admin")]
        public async Task<IActionResult> Create(CreateQuizDTO model)
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

            var quiz = new Quiz
            {
                Name = model.Name,
                GradeId = model.GradeId,
                SubjectId = model.SubjectId,
                TotalMarks = model.TotalMarks,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                TeacherId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty,
                Questions = model.Questions.Select(q => new QuizQuestion
                {
                    QuestionText = q.QuestionText,
                    QuestionType = q.QuestionType,
                    Option1 = q.Option1,
                    Option2 = q.Option2,
                    Option3 = q.Option3,
                    Option4 = q.Option4,
                    CorrectOption = q.CorrectOption
                }).ToList()
            };

            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Index()
        {
            var quizzes = await _context.Quizzes
                .Include(q => q.Grade)
                .Include(q => q.Subject)
                .Include(q => q.Teacher)
                .Select(q => new QuizDetailsDTO
                {
                    Id = q.Id,
                    Name = q.Name,
                    Grade = q.Grade!.Name,
                    Subject = q.Subject!.Name,
                    TotalMarks = q.TotalMarks,
                    StartTime = q.StartTime,
                    EndTime = q.EndTime,
                    IsActive = q.IsActive,
                    TeacherName = q.Teacher!.UserName ?? string.Empty
                })
                .ToListAsync();

            return View(quizzes);
        }

        public async Task<IActionResult> Details(int id)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Grade)
                .Include(q => q.Subject)
                .Include(q => q.Teacher)
                .Include(q => q.Questions)
                .Where(q => q.Id == id)
                .Select(q => new QuizDetailsDTO
                {
                    Id = q.Id,
                    Name = q.Name,
                    Grade = q.Grade!.Name,
                    Subject = q.Subject!.Name,
                    TotalMarks = q.TotalMarks,
                    StartTime = q.StartTime,
                    EndTime = q.EndTime,
                    IsActive = q.IsActive,
                    TeacherName = q.Teacher!.UserName ?? string.Empty,
                    Questions = q.Questions.Select(qq => new QuizQuestionDTO
                    {
                        Id = qq.Id,
                        QuestionText = qq.QuestionText,
                        QuestionType = qq.QuestionType,
                        Option1 = qq.Option1,
                        Option2 = qq.Option2,
                        Option3 = qq.Option3,
                        Option4 = qq.Option4,
                        CorrectOption = qq.CorrectOption
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (quiz == null)
            {
                return NotFound();
            }

            return View(quiz);
        }

        [HttpPost]
        [Authorize(Roles = "Teatcher,Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var quiz = await _context.Quizzes.FindAsync(id);
            if (quiz == null)
            {
                return NotFound();
            }

            _context.Quizzes.Remove(quiz);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "Teatcher,Admin")]
        public async Task<IActionResult> Submissions(int id)
        {
            var submissions = await _context.StudentQuizSubmissions
                .Include(s => s.Student)
                .Include(s => s.Quiz)
                .Where(s => s.QuizId == id)
                .Select(s => new
                {
                    s.Id,
                    s.StudentName,
                    StudentUsername = s.Student!.UserName,
                    s.SubmissionTime,
                    s.ObtainedMarks,
                    s.IsSubmitted,
                    s.IsGraded,
                    QuizTotalMarks = s.Quiz!.TotalMarks
                })
                .ToListAsync();

            ViewBag.QuizId = id;
            return View(submissions);
        }

        [HttpPost]
        [Authorize(Roles = "Teatcher,Admin")]
        public async Task<IActionResult> Grade(int submissionId, int marks)
        {
            var submission = await _context.StudentQuizSubmissions.FindAsync(submissionId);
            if (submission == null)
            {
                return NotFound();
            }

            submission.ObtainedMarks = marks;
            submission.IsGraded = true;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Submissions), new { id = submission.QuizId });
        }

        [Authorize(Roles = "Student,Teatcher,Admin")]
        public async Task<IActionResult> StudentQuizzes()
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var now = DateTime.Now;

            var availableQuizzes = await _context.Quizzes
                .Include(q => q.Grade)
                .Include(q => q.Subject)
                .Include(q => q.Teacher)
                .Where(q => q.IsActive && q.StartTime <= now && q.EndTime >= now)
                .Select(q => new QuizDetailsDTO
                {
                    Id = q.Id,
                    Name = q.Name,
                    Grade = q.Grade!.Name,
                    Subject = q.Subject!.Name,
                    TotalMarks = q.TotalMarks,
                    StartTime = q.StartTime,
                    EndTime = q.EndTime,
                    IsActive = q.IsActive,
                    TeacherName = q.Teacher!.UserName ?? string.Empty,
                    IsSubmitted = _context.StudentQuizSubmissions
                        .Any(s => s.QuizId == q.Id && s.StudentId == studentId && s.IsSubmitted)
                })
                .ToListAsync();

            return View(availableQuizzes);
        }

        [Authorize(Roles = "Student,Teatcher,Admin")]
        public async Task<IActionResult> StudentResults()
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var submissions = await _context.StudentQuizSubmissions
                .Include(s => s.Quiz)
                .ThenInclude(q => q!.Grade)
                .Include(s => s.Quiz)
                .ThenInclude(q => q!.Subject)
                .Where(s => s.StudentId == studentId && s.IsSubmitted)
                .Select(s => new
                {
                    QuizName = s.Quiz!.Name,
                    GradeName = s.Quiz.Grade!.Name,
                    SubjectName = s.Quiz.Subject!.Name,
                    TotalMarks = s.Quiz.TotalMarks,
                    s.ObtainedMarks,
                    s.SubmissionTime,
                    s.IsGraded
                })
                .ToListAsync();

            return View(submissions);
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> TakeQuiz(int id)
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var now = DateTime.Now;

            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                .FirstOrDefaultAsync(q => q.Id == id && q.IsActive && q.StartTime <= now && q.EndTime >= now);

            if (quiz == null)
            {
                return NotFound();
            }

            // Check if student has already submitted this quiz
            var existingSubmission = await _context.StudentQuizSubmissions
                .AnyAsync(s => s.QuizId == id && s.StudentId == studentId && s.IsSubmitted);

            if (existingSubmission)
            {
                TempData["Error"] = "لقد قمت بتسليم هذا الكويز من قبل";
                return RedirectToAction(nameof(StudentQuizzes));
            }

            var quizViewModel = new QuizDetailsDTO
            {
                Id = quiz.Id,
                Name = quiz.Name,
                TotalMarks = quiz.TotalMarks,
                StartTime = quiz.StartTime,
                EndTime = quiz.EndTime,
                Questions = quiz.Questions.Select(q => new QuizQuestionDTO
                {
                    Id = q.Id,
                    QuestionText = q.QuestionText,
                    QuestionType = q.QuestionType,
                    Option1 = q.Option1,
                    Option2 = q.Option2,
                    Option3 = q.Option3,
                    Option4 = q.Option4
                }).ToList()
            };

            return View(quizViewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> SubmitQuiz(int quizId, string studentName, List<StudentQuizAnswerDTO> answers)
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var now = DateTime.Now;

            var quiz = await _context.Quizzes
                .FirstOrDefaultAsync(q => q.Id == quizId && q.IsActive && q.StartTime <= now && q.EndTime >= now);

            if (quiz == null)
            {
                return NotFound();
            }

            // Create submission
            var submission = new StudentQuizSubmission
            {
                QuizId = quizId,
                StudentId = studentId ?? string.Empty,
                StudentName = studentName,
                SubmissionTime = now,
                IsSubmitted = true,
                Answers = answers.Select(a => new StudentQuizAnswer
                {
                    QuestionId = a.QuestionId,
                    SelectedOption = a.SelectedOption,
                    EssayAnswer = a.EssayAnswer
                }).ToList()
            };

            _context.StudentQuizSubmissions.Add(submission);
            await _context.SaveChangesAsync();

            TempData["Success"] = "تم تسليم الكويز بنجاح";
            return RedirectToAction(nameof(StudentQuizzes));
        }
    }
} 