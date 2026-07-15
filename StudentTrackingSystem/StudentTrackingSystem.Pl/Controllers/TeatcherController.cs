using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentTrackingSystem.BLL.Interfaces;
using StudentTrackingSystem.DAL.Models;
using StudentTrackingSystem.PL.DTOs;

namespace StudentTrackingSystem.PL.Controllers
{
    [Authorize]
    public class TeatcherController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TeatcherController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var teachers = await _unitOfWork.TeatcherRepository.GetAllWithSubjectAsync();

            // Add this 👇
            var subjects = await _unitOfWork.SubjectRepository.GetAllAsync();
            ViewBag.Subjects = subjects.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Name
            }).ToList();

            return View(teachers);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadDropDowns();
            return View(new TeatcherDTO());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TeatcherDTO dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropDowns();
                return View(dto);
            }

            string? imagePath = null;
            if (dto.TeacherImage != null)
            {
                var uploads = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                Directory.CreateDirectory(uploads);
                var fileName = Guid.NewGuid() + Path.GetExtension(dto.TeacherImage.FileName);
                var filePath = Path.Combine(uploads, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.TeacherImage.CopyToAsync(fileStream);
                }

                imagePath = "/uploads/" + fileName;
            }

            var teacher = new Teatcher
            {
                FullName = dto.FullName ?? string.Empty,
                Email = dto.Email ?? string.Empty,
                PhoneNumber = dto.PhoneNumber ?? string.Empty,
                SubjectId = dto.SubjectId,
                DateOfBirth = dto.DateOfBirth,
                Gender = dto.Gender ?? string.Empty,
                ImagePath = imagePath

            };

            await _unitOfWork.TeatcherRepository.AddAsync(teacher);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var teacher = await _unitOfWork.TeatcherRepository.GetAsync(id);
            if (teacher == null) return NotFound();

            var dto = new TeatcherDTO
            {
                Id = teacher.Id,
                FullName = teacher.FullName,
                Email = teacher.Email,
                PhoneNumber = teacher.PhoneNumber,
                SubjectId = teacher.SubjectId,
                DateOfBirth = teacher.DateOfBirth,
                Gender = teacher.Gender,
                ImagePath = teacher.ImagePath
            };

            await LoadDropDowns(dto.SubjectId, dto.Gender);
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TeatcherDTO dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropDowns(dto.SubjectId, dto.Gender);
                return View(dto);
            }

            var teacher = await _unitOfWork.TeatcherRepository.GetAsync(id);
            if (teacher == null) return NotFound();

            teacher.FullName = dto.FullName ?? string.Empty;
            teacher.Email = dto.Email ?? string.Empty;
            teacher.PhoneNumber = dto.PhoneNumber ?? string.Empty;
            teacher.SubjectId = dto.SubjectId;
            teacher.DateOfBirth = dto.DateOfBirth;
            teacher.Gender = dto.Gender ?? string.Empty;


            if (dto.TeacherImage != null)
            {
                var uploads = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                Directory.CreateDirectory(uploads);
                var fileName = Guid.NewGuid() + Path.GetExtension(dto.TeacherImage.FileName);
                var filePath = Path.Combine(uploads, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.TeacherImage.CopyToAsync(fileStream);
                }

                teacher.ImagePath = "/uploads/" + fileName;
            }

            _unitOfWork.TeatcherRepository.Update(teacher);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var teacher = await _unitOfWork.TeatcherRepository.GetWithSubjectAsync(id);
            if (teacher == null)
                return NotFound();

            return View(teacher);
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return BadRequest();

            var teacher = await _unitOfWork.TeatcherRepository.GetAsync(id.Value);
            if (teacher is null) return NotFound();

            // تحويل الـ Teatcher إلى TeatcherDTO
            var teacherDTO = new TeatcherDTO
            {
                Id = teacher.Id,
                FullName = teacher.FullName,
                Email = teacher.Email,
                PhoneNumber = teacher.PhoneNumber,
                SubjectId = teacher.SubjectId,
                DateOfBirth = teacher.DateOfBirth,
                Gender = teacher.Gender,
                ImagePath = teacher.ImagePath
            };

            return View("Delete", teacherDTO);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teacher = await _unitOfWork.TeatcherRepository.GetAsync(id);
            if (teacher is null) return NotFound();

            _unitOfWork.TeatcherRepository.Delete(teacher);
            await _unitOfWork.CompleteAsync();

            TempData["Message"] = "Teacher Deleted Successfully";
            return RedirectToAction(nameof(Index));
        }


        private async Task LoadDropDowns(int? selectedSubjectId = null, string? selectedGender = null)
        {
            var subjects = await _unitOfWork.SubjectRepository.GetAllAsync();
            ViewBag.Subjects = new SelectList(subjects, "Id", "Name", selectedSubjectId);

            var genders = new List<string> { "Male", "Female" };
            ViewBag.Genders = new SelectList(genders, selectedGender);
        }
    }
}