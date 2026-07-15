using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using StudentTrackingSystem.BLL.Interfaces;
using StudentTrackingSystem.Models;
using StudentTrackingSystem.PL.DTOs;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using StudentTrackingSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using StudentTrackingSystem.DAL.Data.Contexts;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace StudentTrackingSystem.PL.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppDbContext _Context;

        public StudentController(IUnitOfWork unitOfWork, AppDbContext appContext)
        {
            _unitOfWork = unitOfWork;
            _Context = appContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Get grades for the filter dropdown
            var grades = await _unitOfWork.GradeRepository.GetAllAsync();
            ViewBag.Grades = grades.ToList();

            // Get students with related data
            var students = await _Context.Set<Student>()
                .Include(s => s.Parent)
                .ToListAsync();

            var studentDTOs = students.Select(student => new StudentDTO
            {
                StudentId = student.Id,
                FullName = student.FullName,
                DateOfBirth = student.DateOfBirth,
                EmailAddress = student.EmailAddress,
                PhoneNo = student.PhoneNo,
                Address = student.Address,
                Grade = student.Grade,
                Gender = student.Gender,
                ImagePath = student.ImagePath,
                ParentName = student.FullName,
                ParentPhone = student.PhoneNo,
                ParentEmail = student.EmailAddress
            }).ToList();

            return View("Index", studentDTOs);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var grades = await _unitOfWork.GradeRepository.GetAllAsync();

            ViewBag.Grades = grades.Select(g => new SelectListItem
            {
                Value = g.Name,   // لإنك بتخزن الاسم
                Text = g.Name
            }).ToList();

            return View(new StudentDTO());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentDTO studentDTO, IFormFile studentImage)
        {
            if (ModelState.IsValid)
            {
                // Handle student image
                string? imagePath = null;
                if (studentImage != null && studentImage.Length > 0)
                {
                    var fileName = Path.GetFileName(studentImage.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await studentImage.CopyToAsync(stream);
                    }

                    imagePath = "/images/" + fileName;  // Store the relative path
                }

                // Create a new parent entry (if needed)
                var parent = new Parent
                {
                    FullName = studentDTO.ParentName ?? string.Empty,
                    PhoneNo = studentDTO.ParentPhone ?? string.Empty,
                    EmailAddress = studentDTO.ParentEmail ?? string.Empty,
                };

                await _unitOfWork.ParentRepository.AddAsync(parent);
                await _unitOfWork.CompleteAsync();  // Save parent first to get ParentId

                // Create the student with the parent info
                var student = new Student
                {
                    FullName = studentDTO.FullName ?? string.Empty,
                    DateOfBirth = studentDTO.DateOfBirth,
                    EmailAddress = studentDTO.EmailAddress ?? string.Empty,
                    PhoneNo = studentDTO.PhoneNo ?? string.Empty,
                    Address = studentDTO.Address,
                    Grade = studentDTO.Grade ?? string.Empty,
                    Gender = studentDTO.Gender ?? string.Empty,

                    //Password = studentDTO.Password,  // Save the password
                    ParentId = parent.Id,  // Set the ParentId for the student
                    ImagePath = imagePath   // Set the student's image path
                };

                await _unitOfWork.StudentRepository.AddAsync(student);
                int result = await _unitOfWork.CompleteAsync();

                if (result > 0)
                {
                    TempData["Message"] = "Student Added Successfully";
                    return RedirectToAction("Index");
                }
            }

            return View(studentDTO);
        }
        [Authorize(Roles = "Admin,Teatcher")]
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {

            if (id is null)
                return BadRequest();

            var student = await _Context.Students
                .Include(s => s.Parent)  // هنا بنجيب بيانات الأب المرتبط بالطالب
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student is null)
                return NotFound();

            var studentDTO = new StudentDTO
            {
                StudentId = student.Id,
                FullName = student.FullName,
                Address = student.Address,
                EmailAddress = student.EmailAddress,
                PhoneNo = student.PhoneNo,
                DateOfBirth = student.DateOfBirth,
                Grade = student.Grade,
                Gender = student.Gender,
                ImagePath = student.ImagePath,
                ParentName = student.Parent?.FullName,
                ParentEmail = student.Parent?.EmailAddress,
                ParentPhone = student.Parent?.PhoneNo
            };

            return View(studentDTO);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null) return BadRequest();

            var student = await _unitOfWork.StudentRepository.GetAsync(id.Value);
            if (student is null) return NotFound();

            // Get the parent information if available
            Parent? parent = null;
            if (student.ParentId.HasValue)
            {
                parent = await _unitOfWork.ParentRepository.GetAsync(student.ParentId.Value);
            }

            var studentDTO = new StudentDTO
            {
                StudentId = student.Id,
                FullName = student.FullName,
                DateOfBirth = student.DateOfBirth,
                EmailAddress = student.EmailAddress,
                PhoneNo = student.PhoneNo,
                Address = student.Address,
                Grade = student.Grade,
                Gender = student.Gender,
                ImagePath = student.ImagePath,
                ParentName = parent?.FullName,
                ParentPhone = parent?.PhoneNo,
                ParentEmail = parent?.EmailAddress
            };

            // ✅ تحميل قائمة الدرجات للدروب دون
            var grades = await _unitOfWork.GradeRepository.GetAllAsync();
            ViewBag.Grades = grades.Select(g => new SelectListItem
            {
                Value = g.Name,
                Text = g.Name,
                Selected = g.Name == studentDTO.Grade
            }).ToList();

            return View(studentDTO);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, StudentDTO studentDTO, IFormFile studentImage)
        {
            if (id != studentDTO.StudentId)
                return NotFound();

            // We don't want to immediately reject the form if there are model validation issues
            // Instead, we'll check specific validation issues that matter

            // Keep existing image path if no new image is uploaded
            string? imagePath = studentDTO.ImagePath;
            if (studentImage != null && studentImage.Length > 0)
            {
                var fileName = Path.GetFileName(studentImage.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await studentImage.CopyToAsync(stream);
                }

                imagePath = "/images/" + fileName;
            }

            var student = await _unitOfWork.StudentRepository.GetAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            // Update student information
            student.FullName = studentDTO.FullName ?? string.Empty;
            student.DateOfBirth = studentDTO.DateOfBirth;
            student.EmailAddress = studentDTO.EmailAddress ?? string.Empty;
            student.PhoneNo = studentDTO.PhoneNo ?? string.Empty;
            student.Address = studentDTO.Address;
            student.Grade = studentDTO.Grade ?? string.Empty;
            student.Gender = studentDTO.Gender ?? string.Empty;

            student.ImagePath = imagePath; // Use the image path we determined above

            // Handle parent information
            if (!string.IsNullOrEmpty(studentDTO.ParentName) ||
                !string.IsNullOrEmpty(studentDTO.ParentPhone) ||
                !string.IsNullOrEmpty(studentDTO.ParentEmail))
            {
                if (student.ParentId.HasValue)
                {
                    var existingParent = await _unitOfWork.ParentRepository.GetAsync(student.ParentId.Value);
                    if (existingParent != null)
                    {
                        // Update existing parent
                        existingParent.FullName = studentDTO.ParentName ?? string.Empty;
                        existingParent.PhoneNo = studentDTO.ParentPhone ?? string.Empty;
                        existingParent.EmailAddress = studentDTO.ParentEmail ?? string.Empty;


                        _unitOfWork.ParentRepository.Update(existingParent);
                    }
                    else
                    {
                        // Create new parent if no parent exists but parent ID was set
                        var newParent = new Parent
                        {
                            FullName = studentDTO.ParentName ?? string.Empty,
                            PhoneNo = studentDTO.ParentPhone ?? string.Empty,
                            EmailAddress = studentDTO.ParentEmail ?? string.Empty,

                        };

                        await _unitOfWork.ParentRepository.AddAsync(newParent);
                        await _unitOfWork.CompleteAsync();

                        student.ParentId = newParent.Id;
                    }
                }
                else
                {
                    // Create new parent if student has no parent assigned
                    var newParent = new Parent
                    {
                        FullName = studentDTO.ParentName ?? string.Empty,
                        PhoneNo = studentDTO.ParentPhone ?? string.Empty,
                        EmailAddress = studentDTO.ParentEmail ?? string.Empty,

                    };

                    await _unitOfWork.ParentRepository.AddAsync(newParent);
                    await _unitOfWork.CompleteAsync();

                    student.ParentId = newParent.Id;
                }
            }

            _unitOfWork.StudentRepository.Update(student);
            int result = await _unitOfWork.CompleteAsync();

            if (result > 0)
            {
                TempData["Message"] = "Student Updated Successfully";
                return RedirectToAction("Index");
            }

            // If we get here, something went wrong with saving
            return View(studentDTO);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return BadRequest();

            var student = await _unitOfWork.StudentRepository.GetAsync(id.Value);
            if (student is null) return NotFound();

            // تحويل الـ Student إلى StudentDTO
            var studentDTO = new StudentDTO
            {
                StudentId = student.Id,
                FullName = student.FullName,
                Address = student.Address,
                EmailAddress = student.EmailAddress,
                PhoneNo = student.PhoneNo,
                DateOfBirth = student.DateOfBirth,
                Grade = student.Grade,
                Gender = student.Gender,
                ImagePath = student.ImagePath,
                ParentName = student.Parent?.FullName,
                ParentEmail = student.Parent?.EmailAddress,
                ParentPhone = student.Parent?.PhoneNo
            };

            return View(studentDTO);
        }
        [Authorize(Roles = "Admin")]

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _unitOfWork.StudentRepository.GetAsync(id);
            if (student is null) return NotFound();

            // حذف الطالب من الـ Repository
            _unitOfWork.StudentRepository.Delete(student);
            await _unitOfWork.CompleteAsync();

            TempData["Message"] = "Student Deleted Successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}